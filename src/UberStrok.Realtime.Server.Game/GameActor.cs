using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActor
    {
        public int Cmid => Data.Cmid;
        public string PlayerName => Data.PlayerName;
        public MemberAccessLevel AccessLevel => Data.AccessLevel;

        public TeamID Team
        {
            get { return Data.TeamID; }
            set { Data.TeamID = value; }
        }

        public PlayerMovement Movement { get; set; }
        public GameActorInfoView Data { get; set; }
    }
}
