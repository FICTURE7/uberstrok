using Photon.SocketServer;

namespace UberStrok.Realtime.Server.Game
{
    public class GameApplication : Application
    {
        public static new GameApplication Instance => (GameApplication)ApplicationBase.Instance;

        public GameRoomManager Rooms { get; private set; }

        public int PlayerCount
        {
            get
            {
                var count = 0;
                foreach (var room in Rooms)
                    count += room.Players.Count;

                return count;
            }
        }

        protected override void OnSetup()
        {
            Rooms = new GameRoomManager();
        }

        protected override void OnTearDown()
        {
            /* Space */
        }

        protected override Peer OnCreatePeer(InitRequest initRequest)
        {
            return new GamePeer(initRequest);
        }
    }
}
