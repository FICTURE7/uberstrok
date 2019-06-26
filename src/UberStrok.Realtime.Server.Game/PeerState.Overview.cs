using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class OverviewPeerState : PeerState
    {
        private Countdown _disconnectCountdown;

        public OverviewPeerState(GamePeer peer) : base(peer)
        {
            /* Space */
        }

        public sealed override void OnEnter()
        {
            _disconnectCountdown = new Countdown(Room.Loop, 60, 0);
            _disconnectCountdown.Counted += OnDisconnectCounted;
            _disconnectCountdown.Completed += OnDisconnectCompleted;
            _disconnectCountdown.Start();

            var players = Room.Players;

            /* Let the client know about the other players in the room, if there is any. */
            if (players.Count > 0)
            {
                var allPlayers = new List<GameActorInfoView>(players.Count);
                var allPositions = new List<PlayerMovement>(players.Count);

                foreach (var player in players)
                {
                    allPlayers.Add(player.Actor.Info.View);
                    allPositions.Add(player.Actor.Movement);

                    Debug.Assert(player.Actor.Info.PlayerId == player.Actor.Movement.Number);

                    Peer.KnownActors.Add(player.Actor.Cmid);
                }

                Peer.Events.Game.SendAllPlayers(allPlayers, allPositions, 0);
            }
        }

        public sealed override void OnUpdate()
        {
            _disconnectCountdown.Update();
        }

        private void OnDisconnectCounted(int count)
        {
            if (count <= 10)
                Peer.Events.Game.SendDisconnectCountdown(count);
        }

        private void OnDisconnectCompleted()
        {
            Peer.Disconnect();
        }
    }
}
