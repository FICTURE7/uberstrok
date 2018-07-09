using log4net;
using Photon.SocketServer;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : BasePeer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeer));

        public CommPeer(InitRequest initRequest) : base(initRequest)
        {
            // LobbyRoom adds a LobbyRoomOperationHandler to this CommPeer on
            // initialization.
            _lobby = new LobbyRoom(this);
            _events = new CommPeerEvents(this);

            //AddOperationHandler(new CommPeerOperationHandler(this));
        }

        public CommActor Actor { get; set; }

        public CommPeerEvents Events => _events;
        public LobbyRoom Lobby => _lobby;

        private readonly LobbyRoom _lobby;
        private readonly CommPeerEvents _events;
    }
}
