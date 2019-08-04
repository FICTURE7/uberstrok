using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public abstract partial class GameRoom :  BaseGameRoomOperationHandler
    {
        /* 
         * Enqueue the work on the loop so processing of operations are serial
         * and synchronous.
         */
        protected sealed override void Enqueue(Action action)
        {
            Loop.Enqueue(action);
        }

        public sealed override void OnDisconnect(GamePeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Leave(peer);
        }

        protected sealed override void OnJoinTeam(GameActor actor, TeamID team)
        {
            if (!CanJoin(actor, team))
            {
                actor.Peer.Events.Game.SendJoinGameFailed("Room or team is full.");
            }
            else
            {
                /* 
                 * When the client joins a game it resets (pops) its match state
                 * to 'none'.
                 */
                actor.Reset();

                actor.Info.TeamID = team;
                actor.Info.PlayerState = PlayerStates.Ready;
                actor.PlayerId = _nextPlayer++;

                /* 
                 * Ignore these changes, since we'll send the player to the others
                 * through the PlayerJoined event.
                 */
                actor.Info.GetViewDelta().Reset();

                OnPlayerJoined(new PlayerJoinedEventArgs
                {
                    Player = actor,
                    Team = team
                });
            }
        }

        protected sealed override void OnChatMessage(GameActor actor, string message, byte context)
        {
            var cmid = actor.Cmid;
            var playerName = actor.Info.PlayerName;
            var accessLevel = actor.Info.AccessLevel;
            var chatContext = (ChatContext)context;

            if (accessLevel >= MemberAccessLevel.Moderator && message == "?end")
                State.Set(RoomState.Id.End);

            foreach (var otherActor in Actors)
            {
                if (otherActor.Cmid != cmid)
                {
                    otherActor.Peer.Events.Game.SendChatMessage(
                        cmid,
                        playerName,
                        message,
                        accessLevel,
                        chatContext
                    );
                }
            }
        }

        protected sealed override void OnPowerUpPicked(GameActor actor, int pickupId, PickupItemType type, byte value)
        {
            PowerUps.PickUp(
                actor, 
                pickupId,
                type, 
                value
            );
        }

        protected sealed override void OnPowerUpRespawnTimes(GameActor actor, List<ushort> respawnTimes)
        {
            /* We care only about the first operation sent. */
            if (!PowerUps.IsLoaded)
                PowerUps.Load(respawnTimes);
        }

        protected sealed override void OnSpawnPositions(GameActor actor, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            Debug.Assert(positions.Count == rotations.Count, "Number of spawn positions given and number of rotations given is not equal.");

            /* We care only about the first operation sent for that team ID. */
            if (!Spawns.IsLoaded(team))
                Spawns.Load(team, positions, rotations);
        }

        protected sealed override void OnRespawnRequest(GameActor actor)
        {
            actor.Loadout.Reset();
            actor.Ping.Reset();

            OnPlayerRespawned(new PlayerRespawnedEventArgs { Player = actor });
        }

        protected sealed override void OnExplosionDamage(GameActor actor, int targetCmid, byte slot, byte distance, Vector3 force)
        {
            GameActor attacker = actor;

            int weaponSlot = slot;
            if (weaponSlot >= attacker.Info.Weapons.Count)
                return;

            int weaponId = attacker.Info.Weapons[weaponSlot];
            var weapon = attacker.Loadout.Weapons[weaponId];
            if (weapon == null)
                return;

            var itemClass = weapon.GetView().ItemClass;
            if (itemClass != UberStrikeItemClass.WeaponCannon && itemClass != UberStrikeItemClass.WeaponLauncher)
                return;

            /* Calculate damage amount. */
            float floatDamage = weapon.GetView().DamagePerProjectile;
            float radius = weapon.GetView().SplashRadius / 100f;
            float damageExplosion = floatDamage * (radius - distance) / radius;
            short damage = (short)Math.Min(damageExplosion, short.MaxValue);

            actor.Projectiles.Explode();

            if (actor.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage False positive reached {actor.Cmid}");
                actor.Peer.Disconnect();
            }
            else
            {
                foreach (var victim in Players)
                {
                    if (victim.Cmid != targetCmid)
                        continue;

                    if (DoDamage(victim, attacker, weapon, damage, BodyPart.Body, out Vector3 direction))
                    {
                        OnPlayerKilled(new PlayerKilledEventArgs
                        {
                            Attacker = attacker,
                            Victim = victim,
                            ItemClass = weapon.GetView().ItemClass,
                            Damage = (ushort)damage,
                            Part = BodyPart.Body,
                            Direction = -direction
                        });

                        break;
                    }
                }
            }
        }

        protected sealed override void OnDirectHitDamage(GameActor actor, int target, byte bodyPart, byte bullets)
        {
            GameActor attacker = actor;

            int currentWeaponSlot = attacker.Info.CurrentWeaponSlot;
            if (currentWeaponSlot >= attacker.Info.Weapons.Count)
                return;

            int currentWeaponId = attacker.Info.Weapons[currentWeaponSlot];
            var weapon = attacker.Loadout.Weapons[currentWeaponId];
            if (weapon == null)
                return;

            Debug.Assert(currentWeaponId == weapon.GetView().ID);

            if (bullets > weapon.GetView().ProjectilesPerShot)
                return;
            if (!attacker.Info.IsAlive)
                return;

            weapon.Hit();

            if (weapon.FalsePositive >= weapon.FalsePositiveThreshold)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
            }
            else
            {
                int intDamage = weapon.GetView().DamagePerProjectile * bullets;

                /* Calculate the critical hit damage. */
                var part = (BodyPart)bodyPart;
                int bonus = weapon.GetView().CriticalStrikeBonus;
                if (bonus > 0 && (part == BodyPart.Head || part == BodyPart.Nuts))
                    intDamage += (int)Math.Truncate(bonus / 100f * intDamage);

                var damage = (short)Math.Min(intDamage, short.MaxValue);

                foreach (var victim in Players)
                {
                    if (victim.Cmid != target)
                        continue;

                    if (DoDamage(victim, attacker, weapon, damage, part, out Vector3 direction))
                    {
                        OnPlayerKilled(new PlayerKilledEventArgs
                        {
                            Attacker = attacker,
                            Victim = victim,
                            ItemClass = weapon.GetView().ItemClass,
                            Damage = (ushort)damage,
                            Part = part,
                            Direction = -(direction.Normalized * weapon.GetView().DamageKnockback)
                        });

                        break;
                    }
                }
            }
        }

        protected sealed override void OnDirectDamage(GameActor actor, ushort udamage)
        {
            var damage = (short)udamage;
            if (damage < 0)
            {
                Log.Warn($"Negative damage: {damage}; Disconnecting.");
                actor.Peer.Disconnect();
            }
            else
            {
                if (actor.Info.IsAlive && (actor.Info.Health -= damage) <= 0)
                {
                    actor.Info.Deaths++;
                    actor.Info.Kills--;
                    actor.Statistics.RecordSuicide();

                    OnPlayerKilled(new PlayerKilledEventArgs
                    {
                        Attacker = actor,
                        Victim = actor,
                        ItemClass = UberStrikeItemClass.WeaponMachinegun,
                        Damage = (ushort)damage,
                        Part = BodyPart.Body,
                        Direction = Vector3.Zero
                    });
                }
            }
        }

        protected sealed override void OnDirectDeath(GameActor actor)
        {
            if (actor.Info.IsAlive)
            {
                var damage = (ushort)actor.Info.Health;

                actor.Info.Health = 0;
                actor.Info.Deaths++;
                actor.Info.Kills--;
                actor.Statistics.RecordSuicide();

                OnPlayerKilled(new PlayerKilledEventArgs
                {
                    Attacker = actor,
                    Victim = actor,
                    ItemClass = UberStrikeItemClass.WeaponMelee,
                    Damage = damage,
                    Part = BodyPart.Body,
                    Direction = Vector3.Zero
                });
            }
        }

        protected sealed override void OnEmitProjectile(GameActor actor, Vector3 origin, Vector3 direction, byte slot, int projectileId, bool explode)
        {
            GameActor emitter = actor;

            int weaponSlot = slot - 7;
            if (weaponSlot < 0 || weaponSlot >= emitter.Info.Weapons.Count)
                return;

            int weaponId = emitter.Info.Weapons[weaponSlot];
            var weapon = emitter.Loadout.Weapons[weaponId];
            if (weapon == null)
                return;

            var itemClass = weapon.GetView().ItemClass;
            if (itemClass != UberStrikeItemClass.WeaponCannon && itemClass != UberStrikeItemClass.WeaponLauncher)
                return;

            weapon.Hit();

            if (weapon.FalsePositive >= weapon.FalsePositiveThreshold)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            emitter.Projectiles.Emit(projectileId);

            if (emitter.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Projectiles] OnEmitProjectile FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
            }
            else
            {
                var emitterCmid = emitter.Cmid;
                foreach (var otherActor in Actors)
                {
                    if (otherActor.Cmid != emitterCmid)
                        otherActor.Peer.Events.Game.SendEmitProjectile(emitterCmid, origin, direction, slot, projectileId, explode);
                }
            }
        }

        protected sealed override void OnEmitQuickItem(GameActor actor, Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileId)
        {
            var emitterCmid = actor.Cmid;
            foreach (var otherActor in Actors)
            {
                if (otherActor.Cmid != emitterCmid)
                    otherActor.Peer.Events.Game.SendEmitQuickItem(origin, direction, itemId, playerNumber, projectileId);
            }
        }

        protected sealed override void OnRemoveProjectile(GameActor actor, int projectileId, bool explode)
        {
            actor.Projectiles.Destroy(projectileId);
            if (actor.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Projectiles] OnRemoveProjectile FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
            }
            else
            {
                foreach (var otherActor in Actors)
                    otherActor.Peer.Events.Game.SendRemoveProjectile(projectileId, explode);
            }
        }

        protected sealed override void OnJump(GameActor actor, Vector3 position)
        {
            foreach (var otherPeer in Actors)
            {
                if (otherPeer.Cmid != actor.Cmid)
                    otherPeer.Peer.Events.Game.SendPlayerJumped(actor.Cmid, actor.Movement.Position);
            }
        }

        protected sealed override void OnUpdatePositionAndRotation(GameActor actor, Vector3 position, Vector3 velocity,
                                byte horizontalRotation, byte verticalRotation, byte moveState)
        {
            actor.Movement.Position = position;
            actor.Movement.Velocity = velocity;
            actor.Movement.HorizontalRotation = horizontalRotation;
            actor.Movement.VerticalRotation = verticalRotation;
            actor.Movement.MovementState = moveState;
        }

        protected sealed override void OnSwitchWeapon(GameActor actor, byte slot)
        {
            actor.Info.CurrentWeaponSlot = slot;
        }

        protected sealed override void OnSingleBulletFire(GameActor actor)
        {
            var weapon = actor.Loadout.Weapons[actor.Info.CurrentWeaponID];
            
            /* Send single bullet fire to all peers. */
            foreach (var otherActors in Actors)
                otherActors.Peer.Events.Game.SendSingleBulletFire(actor.Cmid);

            actor.Statistics.RecordShot(weapon.GetView().ItemClass, 1);
        }

        protected sealed override void OnIsInSniperMode(GameActor actor, bool on)
        {
            var state = actor.Info.PlayerState;
            if (on)
                state |= PlayerStates.Sniping;
            else
                state &= ~PlayerStates.Sniping;

            actor.Info.PlayerState = state;
        }

        protected sealed override void OnIsFiring(GameActor actor, bool on)
        {
            var weapon = actor.Loadout.Weapons[actor.Info.CurrentWeaponID];
            var state = actor.Info.PlayerState;

            if (on)
            {
                state |= PlayerStates.Shooting;
                weapon.StartFire();
            }
            else
            {
                state &= ~PlayerStates.Shooting;

                int count = weapon.StopFire();
                actor.Statistics.RecordShot(weapon.GetView().ItemClass, count);
            }

            actor.Info.PlayerState = state;
        }

        protected sealed override void OnIsPaused(GameActor actor, bool on)
        {
            var state = actor.Info.PlayerState;
            if (on)
                state |= PlayerStates.Paused;
            else
                state &= ~PlayerStates.Paused;

            actor.Info.PlayerState = state;
        }

        protected sealed override void OnOpenDoor(GameActor actor, int doorId)
        {
            foreach (var otherActor in Actors)
                otherActor.Peer.Events.Game.SendDoorOpen(doorId);
        }
    }
}
