using System;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class MatchState : Core.State
    {
        private readonly BaseGameRoom _room;

        public MatchState(BaseGameRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _room = room;
        }

        protected BaseGameRoom Room => _room;

        public enum Id
        {
            None,
            WaitingForPlayers,
            Countdown,
            Running
        }
    }
}
