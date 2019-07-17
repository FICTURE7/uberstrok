using Photon.SocketServer;
using System;

namespace UberStrok.Realtime.Server.Game
{
    public class GameApplication : Application
    {
        public static new GameApplication Instance => (GameApplication)ApplicationBase.Instance;

        public GameLobby Lobby { get; private set; }
        public GameRoomManager Rooms => Lobby.Rooms;

        public int PlayerCount
        {
            get
            {
                var count = 0;
                foreach (var room in Lobby.Rooms)
                    count += room.Players.Count;
                return count;
            }
        }

        protected override void OnSetup()
        {
            Lobby = new GameLobby();
        }

        protected override void OnTearDown()
        {
            Lobby.Dispose();
        }

        protected override Peer OnCreatePeer(InitRequest initRequest)
        {
            try
            {
                return new GamePeer(initRequest);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to create GamePeer instance", ex);
                return null;
            }
        }
    }
}
