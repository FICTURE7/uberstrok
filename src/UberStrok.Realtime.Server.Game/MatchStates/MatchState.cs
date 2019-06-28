using log4net;
using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class MatchState : State
    {
        public enum Id
        {
            None,
            WaitingForPlayers,
            Countdown,
            Running,
            End
        }

        protected ILog Log { get; }
        protected GameRoom Room { get; }

        public MatchState(GameRoom room)
        {
            Room = room ?? throw new ArgumentNullException(nameof(room));
            Log = LogManager.GetLogger(GetType().Name);
        }
    }
}
