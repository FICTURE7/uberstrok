using System;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class EndActorState : ActorState
    {
        public EndActorState(GameActor actor) 
            : base(actor)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            var view = new EndOfMatchDataView
            {
                PlayerStatsTotal = Actor.Statistics.Total,
                PlayerStatsBestPerLife = Actor.Statistics.Best,

                /* Not used by the client. */
                MostEffecientWeaponId = 0,
                /* Not used by the client. */
                PlayerXpEarned = null,

                MostValuablePlayers = Room.GetMvps(),

                MatchGuid = Room.GetView().Guid,
                HasWonMatch = Room.Winner == Actor.Info.TeamID,
                TimeInGameMinutes = (int)Math.Ceiling((Room.Loop.Time - Actor.DateJoined).TotalSeconds),
            };

            /* 
             * This sets the client's match and player state to `end of match`
             * state which is the equivalent of End state.
             */
            Peer.Events.Game.SendMatchEnd(view);

            Actor.State.Reset();
            Actor.State.Set(Id.Overview);
        }
    }
}
