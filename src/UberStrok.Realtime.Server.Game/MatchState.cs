using System;
using UberStrok.Realtime.Server.Game.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class MatchState : State
    {
        private readonly GameRoom _room;

        public MatchState(GameRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _room = room;
        }

        protected GameRoom Room => _room;

        public enum Id
        {
            None,
            WaitingForPlayers,
            Countdown,
            Running
        }
    }
}
