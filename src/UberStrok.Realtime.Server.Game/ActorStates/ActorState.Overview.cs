using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class OverviewActorState : ActorState
    {
        private readonly Countdown _disconnectCountdown;

        public OverviewActorState(GameActor actor) 
            : base(actor)
        {
            _disconnectCountdown = new Countdown(Room.Loop, 60 * 3, 0);
            _disconnectCountdown.Counted += OnDisconnectCounted;
            _disconnectCountdown.Completed += OnDisconnectCompleted;
        }

        public override void OnEnter()
        {
            _disconnectCountdown.Restart();

            if (Room.Players.Count > 0)
            {
                /* Send all actors in the room.*/
                var allPlayers = new List<GameActorInfoView>(Room.Actors.Count);
                var allPositions = new List<PlayerMovement>(Room.Actors.Count);
                foreach (var player in Room.Players)
                {
                    Debug.Assert(player.Info.PlayerId == player.Movement.PlayerId);

                    allPlayers.Add(player.Info.GetView());
                    allPositions.Add(player.Movement);
                }

                /* 
                 * The parameter `gameFrame` is not used by the client, therefore 
                 * we just set it to the default (0).
                 */
                Actor.Peer.Events.Game.SendAllPlayers(allPlayers, allPositions, default);
            }
        }

        public override void OnTick()
        {
            _disconnectCountdown.Tick();
        }

        private void OnDisconnectCounted(int count)
        {
            if (count <= 30)
                Peer.Events.Game.SendDisconnectCountdown(count);
        }

        private void OnDisconnectCompleted()
        {
            Peer.Disconnect();
        }
    }
}
