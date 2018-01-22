using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game.Core
{
    public class GameActor
    {
        private readonly GameActorInfo _info;
        private readonly PlayerMovement _movement;
        private readonly DamageEventView _damages;

        public GameActor(GameActorInfoView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _info = new GameActorInfo(data);
            _movement = new PlayerMovement();
            _damages = new DamageEventView();

            
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

        public DamageEventView Damages => _damages;
        public PlayerMovement Movement => _movement;
        public GameActorInfo Info => _info;
    }
}
