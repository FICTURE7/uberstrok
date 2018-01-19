using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class RunningMatchState : MatchState
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(RunningMatchState));

        /* Time when the next ping update happens. */
        private DateTime _nextPingUpdate = DateTime.UtcNow.AddSeconds(1);

        public RunningMatchState(BaseGameRoom room) : base(room)
        {
            // Space
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;

            /* Calculate the time when the games ends (in system ticks). */
            Room.EndTime = Environment.TickCount + Room.Data.TimeLimit * 1000;

            foreach (var player in Room.Players)
                /* 
                    MatchStart event changes the match state of the client to match running,
                    which in turn changes the player state to playing.

                    The client does not care about the roundNumber apparently (in TeamDeathMatch atleast).
                 */
                player.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);

            /* TODO: Increment round number only when the round is over. */
            Room.RoundNumber++;
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        public override void OnUpdate()
        {
            var position = new List<PlayerMovement>(Room.Players.Count);
            foreach (var player in Room.Players)
                position.Add(player.Actor.Movement);

            var deltas = new List<GameActorInfoDeltaView>(Room.Peers.Count);
            var updatePing = DateTime.UtcNow > _nextPingUpdate;
            foreach (var player in Room.Players)
            {
                var delta = player.Actor.Info.ViewDelta;
                if (delta.Changes.Count > 0)
                {
#if DEBUG
                    var changes = delta.Id + ": \r\n";
                    foreach (var change in delta.Changes)
                        changes += change.Key + ": " + change.Value + "\r\n";

                    s_log.Debug(changes);
#endif

                    delta.UpdateMask();
                    deltas.Add(delta);
                }
            }

            if (updatePing)
                _nextPingUpdate = DateTime.UtcNow.AddSeconds(1);

            foreach (var otherPeer in Room.Peers)
            {
                otherPeer.Events.Game.SendAllPlayerDeltas(deltas);
                otherPeer.Events.Game.SendAllPlayerPositions(position, 1);
            }

            foreach (var delta in deltas)
                delta.Changes.Clear();
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
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
