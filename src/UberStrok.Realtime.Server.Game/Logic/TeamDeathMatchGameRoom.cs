using log4net;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game.Logic
{
    public class TeamDeathMatchGameRoom : BaseGameRoom
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(TeamDeathMatchGameRoom));

        public TeamDeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            // Space
        } 
    }
}
