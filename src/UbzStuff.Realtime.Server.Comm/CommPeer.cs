using log4net;
using Photon.SocketServer;

namespace UbzStuff.Realtime.Server.Comm
{
    public class CommPeer : BasePeer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeer));

        public CommPeer(InitRequest initRequest) : base(initRequest)
        {
            _events = new CommPeerEvents(this);
            _lobby = new LobbyRoom(this);

            AddOpHandler(new CommPeerOperationHandler(this));
            //AddOpHandler(new LobbyRoomOperationHandler(this));
        }

        public CommPeerEvents Events => _events;
        public LobbyRoom Lobby => _lobby;

        private readonly LobbyRoom _lobby;
        private readonly CommPeerEvents _events;
    }
}
