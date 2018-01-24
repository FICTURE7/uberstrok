using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Game.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class RunningMatchState : MatchState
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(RunningMatchState));

        public RunningMatchState(GameRoom room) : base(room)
        {
            // Space
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;
            Room.PlayerRespawned += OnPlayerRespawned;
            Room.PlayerKilled += OnPlayerKilled;

            /* Calculate the time when the games ends (in system ticks). */
            Room.EndTime = Environment.TickCount + Room.Data.TimeLimit * 1000;

            foreach (var player in Room.Players)
                player.State.Set(PeerState.Id.Playing);

            /* TODO: Increment round number only when the round is over. */
            Room.RoundNumber++;
        }

        public override void OnResume()
        {
            // Space
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
            Room.PlayerRespawned -= OnPlayerRespawned;
            Room.PlayerKilled -= OnPlayerKilled;
        }

        public override void OnUpdate()
        {
            var deltas = new List<GameActorInfoDeltaView>(Room.Peers.Count);
            var position = new List<PlayerMovement>(Room.Players.Count);
            foreach (var player in Room.Players)
            {
                position.Add(player.Actor.Movement);

                /* If the player has any damage events, we sent them. */
                if (player.Actor.Damages.Count > 0)
                {
                    player.Events.Game.SendDamageEvent(player.Actor.Damages);
                    player.Actor.Damages.Clear();
                }

                /* If the player has changed something since the last tick. */
                var delta = player.Actor.Info.ViewDelta;
                if (delta.Changes.Count > 0)
                {
                    delta.UpdateMask();
                    deltas.Add(delta);
                }

                /* Tick the player state. */
                player.State.Update();
            }

            foreach (var otherPeer in Room.Peers)
            {
                otherPeer.Events.Game.SendAllPlayerDeltas(deltas);
                otherPeer.Events.Game.SendAllPlayerPositions(position, 0);
            }

            foreach (var delta in deltas)
                delta.Changes.Clear();

            foreach (var player in Room.Players)
            {
                if (player.Actor.Info.ShootingTick > 0)
                {
                    player.Actor.Info.ShootingTick -= 1;

                    if (player.Actor.Info.ShootingTick <= 0)
                    {
                        player.Actor.Info.ShootingTick = 0;
                        player.Actor.Info.PlayerState &= ~PlayerStates.Shooting;
                    }
                }
            }
        }

        private void OnPlayerKilled(object sender, PlayerKilledEventArgs e)
        {
            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerKilled(e.AttackerCmid, e.VictimCmid, e.ItemClass, e.Damage, e.Part, e.Direction);
        }

        private void OnPlayerRespawned(object sender, PlayerRespawnedEventArgs e)
        {
            e.Player.Actor.Info.Health = 100;
            e.Player.Actor.Info.PlayerState &= ~PlayerStates.Dead;

            var spawn = Room.SpawnManager.Get(e.Player.Actor.Team);
            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerRespawned(e.Player.Actor.Cmid, spawn.Position, spawn.Rotation);

            /* Switch to previous state which should be 'playing state'. */
            e.Player.State.Previous();

            Debug.Assert(e.Player.State.Current == PeerState.Id.Playing);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            /* Spawn the player in the room. */
            var player = e.Player;
            var point = Room.SpawnManager.Get(player.Actor.Team);
            player.Actor.Movement.Position = point.Position;
            player.Actor.Movement.HorizontalRotation = point.Rotation;

            player.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);

            /* Let all peers know that the client has joined. */
            foreach (var otherPeer in Room.Peers)
            {
                Debug.Assert(!otherPeer.KnownActors.Contains(player.Actor.Cmid));

                otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Info.View, player.Actor.Movement);
                otherPeer.KnownActors.Add(player.Actor.Cmid);
            }

            player.Events.Game.SendPlayerRespawned(player.Actor.Cmid, point.Position, point.Rotation);
        }
    }
}
