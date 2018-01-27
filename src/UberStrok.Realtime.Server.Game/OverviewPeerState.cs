using log4net;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Game.Core;

namespace UberStrok.Realtime.Server.Game
{
    public class OverviewPeerState : PeerState
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(OverviewPeerState));

        public OverviewPeerState(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnEnter()
        {
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

            /* Sync the power ups to the server side. */

            /*
            if (Room.PowerUps.IsLoaded)
            {
                Peer.Events.Game.SendSetPowerUpState(Room.PowerUps.Respawning);
                s_log.Debug($"Respawning pick-ups: {string.Join(", ", Room.PowerUps.Respawning)}");
            }
            */
        }

        public override void OnResume()
        {
            // Space
        }

        public override void OnExit()
        {
            // Space
        }

        public override void OnUpdate()
        {
            // Space
        }
    }
}
