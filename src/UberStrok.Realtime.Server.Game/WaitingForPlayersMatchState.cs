using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class WaitingForPlayersMatchState : MatchState
    {
        public WaitingForPlayersMatchState(BaseGameRoom room) : base(room)
        {
            // Space
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;
            Room.PlayerKilled += OnPlayerKilled;
            Room.PlayerRespawned += OnPlayerRespawned;
        }

        public override void OnResume()
        {
            // Space
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
            Room.PlayerKilled -= OnPlayerKilled;
            Room.PlayerRespawned -= OnPlayerRespawned;
        }

        public override void OnUpdate()
        {
            var deltas = new List<GameActorInfoDeltaView>(Room.Peers.Count);
            var position = new List<PlayerMovement>(Room.Players.Count);
            foreach (var player in Room.Players)
            {
                position.Add(player.Actor.Movement);

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
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            var player = e.Player;

            /* If we got more than 1 player we start the countdown. */
            if (Room.Players.Count > 1)
                Room.State.Set(Id.Countdown);
            else
            {
                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Room.Peers)
                {
                    otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Info.View, player.Actor.Movement);
                    otherPeer.KnownActors.Add(player.Actor.Cmid);
                }

                /* Set the player in a 'waiting for players' state. */
                player.State.Set(PeerState.Id.WaitingForPlayers);
            }
        }

        private void OnPlayerKilled(object sender, PlayerKilledEventArgs e)
        {
            /* Let all peers know that the player has died. */
            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerKilled(e.AttackerCmid, e.VictimCmid, e.ItemClass, e.Damage, e.Part, e.Direction);
        }

        private void OnPlayerRespawned(object sender, PlayerRespawnedEventArgs e)
        {
            /* Let all peers know that the player has respawned. */
            e.Player.Actor.Info.Health = 100;
            e.Player.Actor.Info.PlayerState &= ~PlayerStates.Dead;

            var spawn = Room.SpawnManager.Get(e.Player.Actor.Team);
            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerRespawned(e.Player.Actor.Cmid, spawn.Position, spawn.Rotation);

            /* Switch to previous state which should be 'waiting for players'. */
            e.Player.State.Previous();

            Debug.Assert(e.Player.State.Current == PeerState.Id.WaitingForPlayers);
        }
    }
}
