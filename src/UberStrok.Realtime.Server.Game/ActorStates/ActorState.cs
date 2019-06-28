using log4net;
using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class ActorState : State
    {
        public enum Id
        {
            None,
            Overview,
            WaitingForPlayers,
            Countdown,
            Playing,
            Killed,
            End
        }

        protected ILog Log { get; }
        protected GameActor Actor { get; }
        protected GamePeer Peer => Actor.Peer;
        protected GameRoom Room => Actor.Room;

        public ActorState(GameActor actor)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            Log = LogManager.GetLogger(GetType().Name);
        }
    }
}
