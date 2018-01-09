using Photon.SocketServer;
using System;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : BasePeer
    {
        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            AddOpHandler(new GamePeerOperationHandler(this));

            _events = new GamePeerEvents(this);
        }

        public UberstrikeUserView Member { get; set; }
        public ushort Ping { get; set; }

        public GameRoom Game
        {
            get { return _game; }
            set
            {
                _game = value;
                if (_game == null)
                {
                    // Remove the GameRoomOperationHandler thingy.
                    //TODO: Figure out something better.
                    RemoteOpHandler(0);
                }
            }
        }

        public GamePeerEvents Events => _events;

        private GameRoom _game;
        private readonly GamePeerEvents _events;
    }
}
