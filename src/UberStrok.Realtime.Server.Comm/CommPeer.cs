using log4net;
using Photon.SocketServer;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : BasePeer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeer));

        public LobbyRoom Room { get; set; }
        public CommActor Actor { get; set; }
        public CommPeerEvents Events { get; }

        public CommPeer(InitRequest request) : base(request)
        {
            Events = new CommPeerEvents(this);
            AddOperationHandler(new CommPeerOperationHandler());
        }
    }
}
