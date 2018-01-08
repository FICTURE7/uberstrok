using Photon.SocketServer;
using System;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : BasePeer
    {
        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            AddOpHandler(new GamePeerOperationHandler(this));

            _events = new GamePeerEvents(this);
        }

        public ushort Ping { get; set; }

        public GameRoom Game
        {
            get { return _game; }
            set
            {
                // Remove the GameRoomOperationHandler thingy.
                //TODO: Figure out something better.
                RemoteOpHandler(1);

                _game = value;
            }
        }

        public GamePeerEvents Events => _events;

        private GameRoom _game;
        private readonly GamePeerEvents _events;
    }
}
