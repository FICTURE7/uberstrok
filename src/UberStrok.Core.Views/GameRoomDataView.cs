using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class GameRoomDataView : RoomDataView
    {
        public int ConnectedPlayers { get; set; }
        public int PlayerLimit { get; set; }
        public int TimeLimit { get; set; }
        public int KillLimit { get; set; }
        public int GameFlags { get; set; }
        public int MapID { get; set; }
        public byte LevelMin { get; set; }
        public byte LevelMax { get; set; }
        public GameModeType GameMode { get; set; }
        public bool IsPermanentGame { get; set; }

        public bool IsFull => ConnectedPlayers >= PlayerLimit;
    }
}
