using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract partial class BaseGameRoom : BaseGameRoomOperationHandler, IRoom<GamePeer>
    {
        public override void OnDisconnect(GamePeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Leave(peer);
        }

        protected override void Enqueue(Action action)
        {
            /* Enqueue the work on the loop so processing of operations are serial. */
            Loop.Enqueue(action);
        }

        protected override void OnJoinGame(GamePeer peer, TeamID team)
        {
            /* 
             * When the client joins a game it resets its match state to 'none'.               
             *
             * Update the actor's team + other data and register the peer in the player list.
             * Update the number of connected players while we're at it.
             */
            peer.Actor.Team = team;
            peer.Actor.Info.Health = 100;
            peer.Actor.Info.ArmorPoints = peer.Actor.Info.ArmorPointCapacity;
            peer.Actor.Info.Ping = (ushort)(peer.RoundTripTime / 2);
            peer.Actor.Info.PlayerState = PlayerStates.Ready;
            peer.Actor.Weapons.Reset();

            _players.Add(peer);
            View.ConnectedPlayers = Players.Count;

            OnPlayerJoined(new PlayerJoinedEventArgs
            {
                Player = peer,
                Team = team
            });

            Log.Info($"Joining team -> CMID:{peer.Actor.Cmid}:{team}:{peer.Actor.Number}");
        }

        protected override void OnChatMessage(GamePeer peer, string message, byte context)
        {
            Actions.ChatMessage(peer, message, (ChatContext)context);
        }

        protected override void OnPowerUpPicked(GamePeer peer, int pickupId, byte type, byte value)
        {
            PowerUps.PickUp(peer, pickupId, (PickupItemType)type, value);
        }

        protected override void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes)
        {
            /* We care only about the first operation sent. */
            if (!PowerUps.IsLoaded)
                PowerUps.Load(respawnTimes);
        }

        protected override void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            Debug.Assert(positions.Count == rotations.Count, "Number of spawn positions given and number of rotations given is not equal.");

            /* We care only about the first operation sent for that team ID. */
            if (!Spawns.IsLoaded(team))
                Spawns.Load(team, positions, rotations);
        }

        protected override void OnRespawnRequest(GamePeer peer)
        {
            peer.Actor.Weapons.Reset();
            OnPlayerRespawned(new PlayerRespawnedEventArgs { Player = peer });
        }

        protected override void OnExplosionDamage(GamePeer peer, int targetCmid, byte slot, byte distance, Vector3 force)
        {
            int weaponSlot = slot;
            if (weaponSlot < 0 || weaponSlot >= peer.Actor.Info.Weapons.Count)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage Could not find a Weapon ID in that slot {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            int weaponId = peer.Actor.Info.Weapons[weaponSlot];

            GamePeer attacker = peer;
            Weapon weapon = attacker.Actor.Weapons[weaponId];

            if (weapon == null)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage Could not find a Weapon in that slot {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            var itemClass = weapon.View.ItemClass;
            if (itemClass != UberStrikeItemClass.WeaponCannon && itemClass != UberStrikeItemClass.WeaponLauncher)
            {
                ReportLog.Warn($"[Weapon] OnExplosionDamage ItemClass mismatch {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            /* Calculate damage amount. */
            float damage = weapon.View.DamagePerProjectile;
            float radius = weapon.View.SplashRadius / 100f;
            float damageExplosion = damage * (radius - distance) / radius;
            short shortDamage = (short)damageExplosion;

            foreach (var victim in Players)
            {
                if (victim.Actor.Cmid != targetCmid)
                    continue;

                peer.Actor.Projectiles.Explode();
                if (peer.Actor.Projectiles.FalsePositive >= 10)
                {
                    ReportLog.Warn($"[Weapon] OnExplosionDamage False positive reached {peer.Actor.Cmid}");
                    peer.Disconnect();
                    return;
                }

                if (DoDamage(victim, attacker, shortDamage, BodyPart.Body, out Vector3 direction))
                {
                    OnPlayerKilled(new PlayerKilledEventArgs
                    {
                        Attacker = attacker,
                        Victim = victim,
                        ItemClass = weapon.View.ItemClass,
                        Damage = (ushort)shortDamage,
                        Part = BodyPart.Body,
                        Direction = -direction
                    });
                }
            }
        }

        protected override void OnDirectHitDamage(GamePeer peer, int target, byte bodyPart, byte bullets)
        {
            int currentWeaponSlot = peer.Actor.Info.CurrentWeaponSlot;
            if (currentWeaponSlot < 0 || currentWeaponSlot >= peer.Actor.Info.Weapons.Count)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage Could not find a Weapon ID in the current slot {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            int currentWeaponId = peer.Actor.Info.Weapons[currentWeaponSlot];

            GamePeer attacker = peer;
            Weapon weapon = attacker.Actor.Weapons[currentWeaponId];

            if (weapon == null)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage Could not find a Weapon in the current slot {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            Debug.Assert(currentWeaponId == weapon.View.ID);

            if (!attacker.Actor.Info.View.IsAlive)
            {
                Log.Info("Attacker is dead. Not registering direct hit");
                return;
            }

            weapon.Trigger();

            if (weapon.FalsePositive >= 5)
            {
                ReportLog.Warn($"[Weapon] OnDirectHitDamage FalsePositive reached {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            /* TODO: Clamp value. */
            int damage = weapon.View.DamagePerProjectile * bullets;

            /* Calculate the critical hit damage. */
            var part = (BodyPart)bodyPart;
            int bonus = weapon.View.CriticalStrikeBonus;
            if (bonus > 0 && (part == BodyPart.Head || part == BodyPart.Nuts))
                damage = (int)Math.Round(damage + (damage * (bonus / 100f)));
            var shortDamage = (short)damage;

            foreach (var victim in Players)
            {
                if (victim.Actor.Cmid != target)
                    continue;

                if (DoDamage(victim, attacker, shortDamage, part, out Vector3 direction))
                {
                    OnPlayerKilled(new PlayerKilledEventArgs
                    {
                        Attacker = attacker,
                        Victim = victim,
                        ItemClass = weapon.View.ItemClass,
                        Damage = (ushort)shortDamage,
                        Part = part,
                        Direction = -(direction.Normalized * weapon.View.DamageKnockback)
                    });
                }
            }
        }

        protected override void OnDirectDamage(GamePeer peer, ushort damage)
        {
            var actualDamage = (short)damage;
            if (damage < 0)
            {
                Log.Warn($"Negative damage: {damage}. Disconnecting.");
                peer.Disconnect();
                return;
            }

            peer.Actor.Info.Health -= actualDamage;

            /* Check if the player is dead. */
            if (peer.Actor.Info.Health <= 0)
            {
                peer.Actor.Info.PlayerState |= PlayerStates.Dead;
                peer.Actor.Info.Deaths++;

                peer.State.Set(PeerState.Id.Killed);
                OnPlayerKilled(new PlayerKilledEventArgs
                {
                    Attacker = peer,
                    Victim = peer,
                    ItemClass = UberStrikeItemClass.WeaponMachinegun,
                    Damage = (ushort)actualDamage,
                    Part = BodyPart.Body,
                    Direction = Vector3.Zero
                });
            }
        }

        protected override void OnDirectDeath(GamePeer peer)
        {
            if (!peer.Actor.Info.View.IsAlive)
            {
                Log.Debug($"Player {peer.Actor.Cmid} DirectDeath k: {peer.Actor.Info.Kills} d: {peer.Actor.Info.Deaths}, but already dead");
                return;
            }

            int damage = peer.Actor.Info.Health;
            peer.Actor.Info.Health = 0;
            peer.Actor.Info.PlayerState |= PlayerStates.Dead;
            peer.Actor.Info.Deaths++;
            peer.Actor.Info.Kills--;

            peer.State.Set(PeerState.Id.Killed);
            OnPlayerKilled(new PlayerKilledEventArgs
            {
                Attacker = peer,
                Victim = peer,
                ItemClass = UberStrikeItemClass.WeaponMelee,
                Damage = (ushort)damage,
                Part = BodyPart.Body,
                Direction = Vector3.Zero
            });
        }

        protected override void OnEmitProjectile(GamePeer peer, Vector3 origin, Vector3 direction, byte slot, int projectileId, bool explode)
        {
            int weaponSlot = slot - 7;
            if (weaponSlot < 0 || weaponSlot >= peer.Actor.Info.Weapons.Count)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile Could not find a Weapon ID in the current slot {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            int weaponId = peer.Actor.Info.Weapons[weaponSlot];

            GamePeer emitter = peer;
            Weapon weapon = emitter.Actor.Weapons[weaponId];

            if (weapon == null)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile Could not find a Weapon in that slot {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            var itemClass = weapon.View.ItemClass;
            if (itemClass != UberStrikeItemClass.WeaponCannon && itemClass != UberStrikeItemClass.WeaponLauncher)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile ItemClass mismatch {peer.Actor.Cmid}");
                emitter.Disconnect();
                return;
            }

            weapon.Trigger();

            if (weapon.FalsePositive >= 5)
            {
                ReportLog.Warn($"[Weapon] OnEmitProjectile FalsePositive reached {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            emitter.Actor.Projectiles.Emit(projectileId);
            if (emitter.Actor.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Projectiles] OnEmitProjectile FalsePositive reached {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            var shooterCmid = peer.Actor.Cmid;
            foreach (var otherPeer in Peers)
            {
                if (otherPeer.Actor.Cmid != shooterCmid)
                    otherPeer.Events.Game.SendEmitProjectile(shooterCmid, origin, direction, slot, projectileId, explode);
            }
        }

        protected override void OnEmitQuickItem(GamePeer peer, Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileId)
        {
            var emitterCmid = peer.Actor.Cmid;
            foreach (var otherPeer in Peers)
            {
                if (otherPeer.Actor.Cmid != emitterCmid)
                    otherPeer.Events.Game.SendEmitQuickItem(origin, direction, itemId, playerNumber, projectileId);
            }
        }

        protected override void OnRemoveProjectile(GamePeer peer, int projectileId, bool explode)
        {
            peer.Actor.Projectiles.Destroy(projectileId);
            if (peer.Actor.Projectiles.FalsePositive >= 10)
            {
                ReportLog.Warn($"[Projectiles] OnRemoveProjectile FalsePositive reached {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            foreach (var otherPeer in Peers)
                otherPeer.Events.Game.SendRemoveProjectile(projectileId, explode);
        }

        protected override void OnJump(GamePeer peer, Vector3 position)
        {
            foreach (var otherPeer in Peers)
            {
                if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                    otherPeer.Events.Game.SendPlayerJumped(peer.Actor.Cmid, peer.Actor.Movement.Position);
            }
        }

        protected override void OnUpdatePositionAndRotation(GamePeer peer, Vector3 position, Vector3 velocity, byte horizontalRotation, byte verticalRotation, byte moveState)
        {
            peer.Actor.Movement.Position = position;
            peer.Actor.Movement.Velocity = velocity;
            peer.Actor.Movement.HorizontalRotation = horizontalRotation;
            peer.Actor.Movement.VerticalRotation = verticalRotation;
            peer.Actor.Movement.MovementState = moveState;
        }

        protected override void OnSwitchWeapon(GamePeer peer, byte slot)
        {
            peer.Actor.Info.CurrentWeaponSlot = slot;
        }

        protected override void OnSingleBulletFire(GamePeer peer)
        {
            /* Send single bullet fire to all peers. */
            foreach (var otherPeer in Peers)
                otherPeer.Events.Game.SendSingleBulletFire(peer.Actor.Cmid);
        }

        protected override void OnIsInSniperMode(GamePeer peer, bool on)
        {
            var state = peer.Actor.Info.PlayerState;
            if (on)
                state |= PlayerStates.Sniping;
            else
                state &= ~PlayerStates.Sniping;

            peer.Actor.Info.PlayerState = state;
        }

        protected override void OnIsFiring(GamePeer peer, bool on)
        {
            var state = peer.Actor.Info.PlayerState;
            if (on)
                state |= PlayerStates.Shooting;
            else
                state &= ~PlayerStates.Shooting;

            peer.Actor.Info.PlayerState = state;
        }

        protected override void OnIsPaused(GamePeer peer, bool on)
        {
            var state = peer.Actor.Info.PlayerState;
            if (on)
                state |= PlayerStates.Paused;
            else
                state &= ~PlayerStates.Paused;

            peer.Actor.Info.PlayerState = state;
        }

        protected override void OnOpenDoor(GamePeer peer, int doorId)
        {
            foreach (var otherPeer in Peers)
                otherPeer.Events.Game.SendDoorOpen(doorId);
        }
    }
}
