using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActor
    {
        private readonly GameActorInfo _info;
        private readonly PlayerMovement _movement;

        public GameActor(GameActorInfoView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _info = new GameActorInfo(data);
            _movement = new PlayerMovement();
        }

        public TeamID Team
        {
            get { return Info.TeamID; }
            set { Info.TeamID = value; }
        }

        public int Number
        {
            get { return Info.PlayerId; }
            set
            {
                Info.PlayerId = (byte)value;
                Movement.Number = (byte)value;
            }
        }

        public int Cmid => Info.Cmid;
        public string PlayerName => Info.PlayerName;
        public MemberAccessLevel AccessLevel => Info.AccessLevel;

        public PlayerMovement Movement => _movement;
        public GameActorInfo Info => _info;
    }
}
