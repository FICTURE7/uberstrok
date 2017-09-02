using System.Collections.Generic;
using UbzStuff.Core.Views;

namespace UbzStuff.Realtime.Server.Comm
{
    public class LobbyRoomManager
    {
        public static LobbyRoomManager Instance => s_instance;
        private static LobbyRoomManager s_instance = new LobbyRoomManager();

        private LobbyRoomManager()
        {
            _actors = new List<CommActorInfoView>();
        }

        public List<CommActorInfoView> Actors => _actors;
        private readonly List<CommActorInfoView> _actors;
    }
}
