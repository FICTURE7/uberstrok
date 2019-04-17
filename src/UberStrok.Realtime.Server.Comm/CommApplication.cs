using Photon.SocketServer;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommApplication : Application
    {
        public static new CommApplication Instance => (CommApplication)Application.Instance;

        public LobbyRoomManager Rooms { get; private set; }

        protected override void OnSetup()
        {
            Rooms = new LobbyRoomManager();
        }

        protected override void OnTearDown()
        {
            /* Space */
        }

        protected override Peer OnCreatePeer(InitRequest initRequest)
        {
            return new CommPeer(initRequest);
        }
    }
}
