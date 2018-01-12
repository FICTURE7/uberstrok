using Photon.SocketServer;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : BasePeer
    {
        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            _events = new GamePeerEvents(this);

            /* Could make GamePeerOperationHandler a singleton but what ever. */
            AddOperationHandler(new GamePeerOperationHandler());
        }

        public string AuthToken { get; set; }
        public ushort Ping { get; set; }
        public BaseGameRoom Room { get; set; }
        public UberstrikeUserView Member { get; set; }

        public GamePeerEvents Events => _events;

        private readonly GamePeerEvents _events;
    }
}
