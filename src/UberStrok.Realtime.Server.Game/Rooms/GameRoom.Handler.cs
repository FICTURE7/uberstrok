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
        /* Enqueue the work on the loop so processing of operations are serial. */
        protected override void Enqueue(Action action)
        {
            Loop.Enqueue(action);
        }

        public override void OnDisconnect(GamePeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Leave(peer);
        }

        protected override void OnJoinTeam(GameActor actor, TeamID team)
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

        protected override void OnChatMessage(GameActor actor, string message, byte context)
        {
            var cmid = actor.Cmid;
            var playerName = actor.Info.PlayerName;
            var accessLevel = actor.Info.AccessLevel;
            var chatContext = (ChatContext)context;

            if (accessLevel >= MemberAccessLevel.Moderator && message == "?end")
                State.Set(MatchState.Id.End);

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

        protected override void OnPowerUpPicked(GameActor actor, int pickupId, PickupItemType type, byte value)
        {
            PowerUps.PickUp(
                actor, 
                pickupId,
                type, 
                value
            );
        }

        protected override void OnPowerUpRespawnTimes(GameActor actor, List<ushort> respawnTimes)
        {
            /* We care only about the first operation sent. */
            if (!PowerUps.IsLoaded)
                PowerUps.Load(respawnTimes);
        }

        protected override void OnSpawnPositions(GameActor actor, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            Debug.Assert(positions.Count == rotations.Count, "Number of spawn positions given and number of rotations given is not equal.");

            /* We care only about the first operation sent for that team ID. */
            if (!Spawns.IsLoaded(team))
                Spawns.Load(team, positions, rotations);
        }

        protected override void OnRespawnRequest(GameActor actor)
        {
            actor.Loadout.Reset();
            actor.Ping.Reset();

            OnPlayerRespawned(new PlayerRespawnedEventArgs { Player = actor });
        }

        protected override void OnExplosionDamage(GameActor actor, int targetCmid, byte slot, byte distance, Vector3 force)
        {
            int weaponSlot = slot;
            if (weaponSlot < 0 || weaponSlot >= actor.Info.Weapons.Count)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage Could not find a Weapon ID in that slot {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            int weaponId = actor.Info.Weapons[weaponSlot];

            GameActor attacker = actor;
            Weapon weapon = attacker.Loadout.Weapons[weaponId];

            if (weapon == null)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage Could not find a Weapon in that slot {actor.Cmid}");
                return;
            }

            var itemClass = weapon.GetView().ItemClass;
            if (itemClass != UberStrikeItemClass.WeaponCannon && itemClass != UberStrikeItemClass.WeaponLauncher)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage ItemClass mismatch {actor.Cmid}");
                return;
            }

            /* Calculate damage amount. */
            float damage = weapon.GetView().DamagePerProjectile;
            float radius = weapon.GetView().SplashRadius / 100f;
            float damageExplosion = damage * (radius - distance) / radius;
            short shortDamage = (short)damageExplosion;

            actor.Projectiles.Explode();
            if (actor.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage False positive reached {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            foreach (var victim in Players)
            {
                if (victim.Cmid != targetCmid)
                    continue;

                if (DoDamage(victim, attacker, weapon, shortDamage, BodyPart.Body, out Vector3 direction))
                {
                    OnPlayerKilled(new PlayerKilledEventArgs
                    {
                        Attacker = attacker,
                        Victim = victim,
                        ItemClass = weapon.GetView().ItemClass,
                        Damage = (ushort)shortDamage,
                        Part = BodyPart.Body,
                        Direction = -direction
                    });
                }
            }
        }

        protected override void OnDirectHitDamage(GameActor actor, int target, byte bodyPart, byte bullets)
        {
            int currentWeaponSlot = actor.Info.CurrentWeaponSlot;
            if (currentWeaponSlot < 0 || currentWeaponSlot >= actor.Info.Weapons.Count)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage Could not find a Weapon ID in the current slot {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            int currentWeaponId = actor.Info.Weapons[currentWeaponSlot];

            GameActor attacker = actor;
            Weapon weapon = attacker.Loadout.Weapons[currentWeaponId];

            if (weapon == null)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage Could not find a Weapon in the current slot {actor.Cmid}");
                return;
            }

            Debug.Assert(currentWeaponId == weapon.GetView().ID);

            if (!attacker.Info.IsAlive)
            {
                Log.Info("Attacker is dead. Not registering direct hit");
                return;
            }

            weapon.Hit();

            if (weapon.FalsePositive >= weapon.FalsePositiveThreshold)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            if (bullets > weapon.GetView().ProjectilesPerShot)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage Fired more bullet than in stats {actor.Cmid}");
                return;
            }

            /* TODO: Clamp value. */
            int damage = weapon.GetView().DamagePerProjectile * bullets;

            /* Calculate the critical hit damage. */
            var part = (BodyPart)bodyPart;
            int bonus = weapon.GetView().CriticalStrikeBonus;
            if (bonus > 0 && (part == BodyPart.Head || part == BodyPart.Nuts))
                damage = (int)Math.Round(damage + (damage * (bonus / 100f)));
            var shortDamage = (short)damage;

            foreach (var victim in Players)
            {
                if (victim.Cmid != target)
                    continue;

                if (DoDamage(victim, attacker, weapon, shortDamage, part, out Vector3 direction))
                {
                    OnPlayerKilled(new PlayerKilledEventArgs
                    {
                        Attacker = attacker,
                        Victim = victim,
                        ItemClass = weapon.GetView().ItemClass,
                        Damage = (ushort)shortDamage,
                        Part = part,
                        Direction = -(direction.Normalized * weapon.GetView().DamageKnockback)
                    });

                    break;
                }
            }
        }

        protected override void OnDirectDamage(GameActor actor, ushort damage)
        {
            var actualDamage = (short)damage;
            if (damage < 0)
            {
                Log.Warn($"Negative damage: {damage}. Disconnecting.");
                actor.Peer.Disconnect();
                return;
            }

            actor.Info.Health -= actualDamage;

            /* Check if the player is dead. */
            if (actor.Info.Health <= 0)
            {
                actor.Info.PlayerState |= PlayerStates.Dead;
                actor.Info.Deaths++;

                OnPlayerKilled(new PlayerKilledEventArgs
                {
                    Attacker = actor,
                    Victim = actor,
                    ItemClass = UberStrikeItemClass.WeaponMachinegun,
                    Damage = (ushort)actualDamage,
                    Part = BodyPart.Body,
                    Direction = Vector3.Zero
                });
            }
        }

        protected override void OnDirectDeath(GameActor actor)
        {
            if (!actor.Info.IsAlive)
            {
                Log.Debug($"Player {actor.Cmid} DirectDeath k: {actor.Info.Kills} d: {actor.Info.Deaths}, but already dead");
                return;
            }

            int damage = actor.Info.Health;
            actor.Info.Health = 0;
            actor.Info.PlayerState |= PlayerStates.Dead;
            actor.Info.Deaths++;
            actor.Info.Kills--;

            OnPlayerKilled(new PlayerKilledEventArgs
            {
                Attacker = actor,
                Victim = actor,
                ItemClass = UberStrikeItemClass.WeaponMelee,
                Damage = (ushort)damage,
                Part = BodyPart.Body,
                Direction = Vector3.Zero
            });
        }

        protected override void OnEmitProjectile(GameActor actor, Vector3 origin, Vector3 direction, byte slot, int projectileId, bool explode)
        {
            int weaponSlot = slot - 7;
            if (weaponSlot < 0 || weaponSlot >= actor.Info.Weapons.Count)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile Could not find a Weapon ID in the current slot {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            int weaponId = actor.Info.Weapons[weaponSlot];

            GamePeer emitter = actor.Peer;
            Weapon weapon = emitter.Actor.Loadout.Weapons[weaponId];

            if (weapon == null)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile Could not find a Weapon in that slot {actor.Cmid}");
                return;
            }

            var itemClass = weapon.GetView().ItemClass;
            if (itemClass != UberStrikeItemClass.WeaponCannon && itemClass != UberStrikeItemClass.WeaponLauncher)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile ItemClass mismatch {actor.Cmid}");
                return;
            }

            weapon.Hit();

            if (weapon.FalsePositive >= weapon.FalsePositiveThreshold)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            emitter.Actor.Projectiles.Emit(projectileId);
            if (emitter.Actor.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Projectiles] OnEmitProjectile FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            var shooterCmid = actor.Cmid;
            foreach (var otherActor in Actors)
            {
                if (otherActor.Cmid != shooterCmid)
                    otherActor.Peer.Events.Game.SendEmitProjectile(shooterCmid, origin, direction, slot, projectileId, explode);
            }
        }

        protected override void OnEmitQuickItem(GameActor actor, Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileId)
        {
            var emitterCmid = actor.Cmid;
            foreach (var otherActor in Actors)
            {
                if (otherActor.Cmid != emitterCmid)
                    otherActor.Peer.Events.Game.SendEmitQuickItem(origin, direction, itemId, playerNumber, projectileId);
            }
        }

        protected override void OnRemoveProjectile(GameActor actor, int projectileId, bool explode)
        {
            actor.Projectiles.Destroy(projectileId);
            if (actor.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Projectiles] OnRemoveProjectile FalsePositive reached {actor.Cmid}");
                actor.Peer.Disconnect();
                return;
            }

            foreach (var otherActor in Actors)
                otherActor.Peer.Events.Game.SendRemoveProjectile(projectileId, explode);
        }

        protected override void OnJump(GameActor actor, Vector3 position)
        {
            foreach (var otherPeer in Actors)
            {
                if (otherPeer.Cmid != actor.Cmid)
                    otherPeer.Peer.Events.Game.SendPlayerJumped(actor.Cmid, actor.Movement.Position);
            }
        }

        protected override void OnUpdatePositionAndRotation(GameActor actor, Vector3 position, Vector3 velocity,
                                byte horizontalRotation, byte verticalRotation, byte moveState)
        {
            actor.Movement.Position = position;
            actor.Movement.Velocity = velocity;
            actor.Movement.HorizontalRotation = horizontalRotation;
            actor.Movement.VerticalRotation = verticalRotation;
            actor.Movement.MovementState = moveState;
        }

        protected override void OnSwitchWeapon(GameActor actor, byte slot)
        {
            actor.Info.CurrentWeaponSlot = slot;
        }

        protected override void OnSingleBulletFire(GameActor actor)
        {
            var weapon = actor.Loadout.Weapons[actor.Info.CurrentWeaponID];
            
            /* Send single bullet fire to all peers. */
            foreach (var otherActors in Actors)
                otherActors.Peer.Events.Game.SendSingleBulletFire(actor.Cmid);

            actor.Statistics.RecordShot(weapon.GetView().ItemClass, 1);
        }

        protected override void OnIsInSniperMode(GameActor actor, bool on)
        {
            var state = actor.Info.PlayerState;
            if (on)
                state |= PlayerStates.Sniping;
            else
                state &= ~PlayerStates.Sniping;

            actor.Info.PlayerState = state;
        }

        protected override void OnIsFiring(GameActor actor, bool on)
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

        protected override void OnIsPaused(GameActor actor, bool on)
        {
            var state = actor.Info.PlayerState;
            if (on)
                state |= PlayerStates.Paused;
            else
                state &= ~PlayerStates.Paused;

            actor.Info.PlayerState = state;
        }

        protected override void OnOpenDoor(GameActor actor, int doorId)
        {
            foreach (var otherActor in Actors)
                otherActor.Peer.Events.Game.SendDoorOpen(doorId);
        }
    }
}
