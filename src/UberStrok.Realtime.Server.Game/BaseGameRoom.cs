using System;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoom : BaseRoom<GamePeer>
    {
        public BaseGameRoom(GameRoomDataView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _data = data;
            _handler = new GameRoomOperationHandler(this);
        }

        public int Id
        {
            get { return _data.Number; }
            set { _data.Number = value; }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _data.IsPasswordProtected = !string.IsNullOrEmpty(value);
                _password = value;
            }
        }

        public GameRoomDataView Data => _data;

        private string _password;
        private readonly GameRoomDataView _data;
        private readonly GameRoomOperationHandler _handler;

        public override void OnJoin(GamePeer peer)
        {
            base.OnJoin(peer);

            peer.Room = this;
            peer.Events.SendRoomEntered(Data);

            peer.AddOpHandler(_handler);

            //TODO: Count players who are playing and not spectating and stuff.
            _data.ConnectedPlayers = Peers.Count;
        }

        public override void OnLeave(GamePeer peer)
        {
            base.OnLeave(peer);

            peer.Room = null;
            peer.RemoveOpHandler(_handler.Id);

            _data.ConnectedPlayers = Peers.Count;
        }
    }
}
