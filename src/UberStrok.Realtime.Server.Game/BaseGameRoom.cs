using System;
using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoom : GameRoomOperationHandler, IRoom<GamePeer>
    {
        private string _password;
        private readonly GameRoomDataView _data;
        private readonly List<GamePeer> _peers;
        private readonly IReadOnlyCollection<GamePeer> _peersReadOnly;

        public BaseGameRoom(GameRoomDataView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _peers = new List<GamePeer>();
            _peersReadOnly = _peers.AsReadOnly();
            _data = data;
        }

        public IReadOnlyCollection<GamePeer> Peers => _peersReadOnly;

        public int Number
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

        public void OnJoin(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peers.Add(peer);

            peer.Room = this;
            peer.Events.SendRoomEntered(Data);

            peer.AddOperationHandler(this);

            //TODO: Count players who are playing and not spectating and stuff.
            _data.ConnectedPlayers = Peers.Count;
        }

        public void OnLeave(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peers.Remove(peer);

            peer.Room = null;
            peer.RemoveOperationHandler(Id);

            _data.ConnectedPlayers = Peers.Count;
        }
    }
}
