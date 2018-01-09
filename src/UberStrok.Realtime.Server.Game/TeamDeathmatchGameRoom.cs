using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class TeamDeathMatchGameRoom : BaseGameRoom
    {
        public TeamDeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            // Space
        }

        public List<SpawnPoint> SpawnPoints { get; set; }
        public Dictionary<TeamID, List<TimeSpan>> PickupRespawnTimes { get; set; }
    }
}
