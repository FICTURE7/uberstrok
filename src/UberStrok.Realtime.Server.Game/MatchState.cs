using log4net;
using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class MatchState : State
    {
        public MatchState(BaseGameRoom room)
        {
            Room = room ?? throw new ArgumentNullException(nameof(room));
            Log = LogManager.GetLogger(GetType().Name);
        }

        protected ILog Log { get; }
        protected BaseGameRoom Room { get; }

        public enum Id
        {
            None,
            WaitingForPlayers,
            Countdown,
            Running
        }
    }
}
