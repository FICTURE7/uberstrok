using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActor
    {
        public int Cmid => View.Cmid;
        public TeamID Team => View.TeamID;

        public PlayerMovement Movement { get; set; }
        public GameActorInfoView View { get; set; }
    }
}
