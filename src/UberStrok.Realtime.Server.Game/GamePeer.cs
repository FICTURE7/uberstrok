using Photon.SocketServer;
using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : BasePeer
    {
        private readonly GamePeerEvents _events;

        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            KnownActors = new List<int>(16);
            _events = new GamePeerEvents(this);

            /* Could make GamePeerOperationHandler a singleton but what ever. */
            AddOperationHandler(new GamePeerOperationHandler());
        }

        public string AuthToken { get; set; }
        public ushort Ping { get; set; }
        public GameActor Actor { get; set; }
        public List<int> KnownActors { get; set; }
        public BaseGameRoom Room { get; set; }
        public UberstrikeUserView Member { get; set; }

        public GamePeerEvents Events => _events;
    }
}
