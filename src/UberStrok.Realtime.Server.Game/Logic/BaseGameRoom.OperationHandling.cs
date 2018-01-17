using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using System;

namespace UberStrok.Realtime.Server.Game.Logic
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
            peer.Actor.Data.Health = 100;
            peer.Actor.Data.Ping = (ushort)(peer.RoundTripTime / 2);
            peer.Actor.Data.PlayerState |= PlayerStates.Ready;

            lock (_peers)
            {
                _players.Add(peer);
                _data.ConnectedPlayers = Players.Count;
            }

            s_log.Info($"Joining team -> CMID:{peer.Actor.Cmid}:{team}:{peer.Actor.Number}");

            /*
                If the match has not started yet and there is more
                than 1 players, we start the match.
             */
            if (!_started && Players.Count > 1)
            {
                StartMatch();
                return;
            }

            /*
                If we haven't yet started the match we send the peer
                in 'waiting for players' state.
             */
            if (!_started)
            {
                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Peers)
                {
                    otherPeer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, peer.Actor.Movement);
                    otherPeer.KnownActors.Add(peer.Actor.Cmid);
                }

                peer.Events.Game.SendWaitingForPlayer();
            }
            /*
                Otherwise we send the client to random spawn for its
                team.
             */
            else
            {
                var point = _spawnManager.Get(peer.Actor.Team);
                peer.Actor.Movement.Position = point.Position;
                peer.Actor.Movement.HorizontalRotation = point.Rotation;

                peer.Events.Game.SendMatchStart(_roundNumber, _endTime);

                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Peers)
                {
                    otherPeer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, peer.Actor.Movement);
                    otherPeer.KnownActors.Add(otherPeer.Actor.Cmid);
                }

                //peer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, peer.Actor.Movement);
                peer.Events.Game.SendPlayerRespawned(peer.Actor.Cmid, point.Position, point.Rotation);
            }
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

            peer.Actor.Data.Health -= actualDamage;
            peer.Actor.Delta.Changes[GameActorInfoDeltaView.Keys.Health] = actualDamage;
        }

        protected override void OnDirectHitDamage(GamePeer peer, int target, byte bodyPart, byte bullets)
        {
            foreach (var player in Players)
            {
                if (player.Actor.Cmid == target)
                {
                    player.Actor.Data.Health -= 1;
                    player.Actor.Delta.Changes[GameActorInfoDeltaView.Keys.Health] = peer.Actor.Data.Health;
                    break;
                }
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
            peer.Actor.Data.CurrentWeaponSlot = slot;
            peer.Actor.Delta.Changes[GameActorInfoDeltaView.Keys.CurrentWeaponSlot] = slot;
        }

        protected override void OnIsFiring(GamePeer peer, bool on)
        {
            if (on)
                peer.Actor.Data.PlayerState |= PlayerStates.Shooting;
            else
                peer.Actor.Data.PlayerState &= ~PlayerStates.Shooting;

            peer.Actor.Delta.Changes[GameActorInfoDeltaView.Keys.PlayerState] = peer.Actor.Data.PlayerState;

            s_log.Debug(peer.Actor.Delta.Changes[GameActorInfoDeltaView.Keys.PlayerState]);
        }

        protected override void OnIsPaused(GamePeer peer, bool on)
        {
            if (on)
                peer.Actor.Data.PlayerState |= PlayerStates.Paused;
            else
                peer.Actor.Data.PlayerState &= ~PlayerStates.Paused;

            peer.Actor.Delta.Changes[GameActorInfoDeltaView.Keys.PlayerState] = peer.Actor.Data.PlayerState;
        }
    }
}
