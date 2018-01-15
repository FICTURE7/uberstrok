using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActor
    {
        private readonly GameActorInfoView _data;
        private readonly PlayerMovement _movement;

        public GameActor(GameActorInfoView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data;
            _movement = new PlayerMovement();
        }

        public TeamID Team
        {
            get { return Data.TeamID; }
            set { Data.TeamID = value; }
        }

        public int Cmid => Data.Cmid;
        public string PlayerName => Data.PlayerName;
        public MemberAccessLevel AccessLevel => Data.AccessLevel;
        public PlayerMovement Movement => _movement;
        public GameActorInfoView Data => _data;
    }
}
