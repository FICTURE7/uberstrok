﻿using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
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

        protected override void OnJoinGame(GamePeer peer, TeamID team)
        {
            /* 
                When the client joins a game it resets its game state to 'none'.               

                Update the actor's team + other data and register the peer in the player list.
                Update the number of connected players while we're at it.
             */
            peer.Actor.Team = team;
            peer.Actor.Info.Health = 100;
            peer.Actor.Info.Ping = (ushort)(peer.RoundTripTime / 2);
            peer.Actor.Info.PlayerState |= PlayerStates.Ready;

            lock (_peers)
            {
                _players.Add(peer);
                _data.ConnectedPlayers = Players.Count;
            }

            OnPlayerJoined(new PlayerJoinedEventArgs
            {
                Player = peer,
                Team = team
            });

            s_log.Info($"Joining team -> CMID:{peer.Actor.Cmid}:{team}:{peer.Actor.Number}");
        }

        protected override void OnChatMessage(GamePeer peer, string message, ChatContext context)
        {
            var cmid = peer.Actor.Cmid;
            var playerName = peer.Actor.PlayerName;
            var accessLevel = peer.Actor.AccessLevel;

            foreach (var otherPeer in Peers)
            {
                if (otherPeer.Actor.Cmid != cmid)
                    otherPeer.Events.Game.SendChatMessage(cmid, playerName, message, accessLevel, context);
            }
        }

        protected override void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes)
        {
            /* We care only about the first operation sent. */
            if (!_powerUpManager.IsLoaded())
                _powerUpManager.Load(respawnTimes);
        }

        protected override void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            /* We care only about the first operation sent for that team ID. */
            if (!_spawnManager.IsLoaded(team))
                _spawnManager.Load(team, positions, rotations);
        }

        protected override void OnDirectDamage(GamePeer peer, ushort damage)
        {
            var actualDamage = (short)damage;
            /* THEY SHEATING */
            if (damage < 0)
                return;

            peer.Actor.Info.Health -= actualDamage;
        }

        protected override void OnDirectHitDamage(GamePeer peer, int target, byte bodyPart, byte bullets)
        {
            var weaponId = peer.Actor.Info.CurrentWeaponID;

            foreach (var player in Players)
            {
                if (player.Actor.Cmid != target)
                    continue;

                var weapon = default(UberStrikeItemWeaponView);
                if (ShopManager.WeaponItems.TryGetValue(weaponId, out weapon))
                {
                    /* TODO: Clamp value. */
                    var damage = (weapon.DamagePerProjectile * bullets);

                    /* Calculate the critical hit damage. */
                    var part = (BodyPart)bodyPart;
                    var bonus = weapon.CriticalStrikeBonus;
                    if (bonus > 0)
                    {
                        if (part == BodyPart.Head || part == BodyPart.Nuts)
                            damage = (int)Math.Round(damage + (damage * (bonus / 100f)));
                    }

                    /* Calculate the direction of the hit. */
                    var shortDamage = (short)damage;

                    var victimPos = player.Actor.Movement.Position;
                    var attackerPos = peer.Actor.Movement.Position;

                    var direction = attackerPos - victimPos;
                    var back = new Vector3(0, 0, -1);

                    var angle = Vector3.Angle(direction, back);
                    if (direction.x < 0)
                        angle = 360 - angle;

                    var byteAngle = Conversions.Angle2Byte(angle);

                    /* TODO: Find out the damage effect type & stuffs. */
                    player.Actor.Damages.Add(byteAngle, shortDamage, part, 0, 0);
                    player.Actor.Info.Health -= shortDamage;
                }
                else
                {
                    s_log.Debug($"Unable to find weapon with ID {weaponId}");
                }

                return;
            }
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
    }
}