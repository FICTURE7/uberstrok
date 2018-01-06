using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Client
{
    public abstract class BaseLobbyRoom : IEventDispatcher
    {
        public BaseLobbyRoom(BasePeer peer)
        {
            _operations = new LobbyRoomOperations(peer, 0);

            peer.AddDispatcher(this);
        }

        public LobbyRoomOperations Operations => _operations;
        private readonly LobbyRoomOperations _operations;

        void IEventDispatcher.OnEvent(byte opCode, byte[] data)
        {
            var op = (ILobbyRoomEventsType)opCode;
            switch (op)
            {
                case ILobbyRoomEventsType.FullPlayerListUpdate:
                    FullPlayerListUpdate(data);
                    break;

                case ILobbyRoomEventsType.LobbyChatMessage:
                    LobbyChatMessage(data);
                    break;
            }
        }

        private void FullPlayerListUpdate(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var players = ListProxy<CommActorInfoView>.Deserialize(bytes, CommActorInfoViewProxy.Deserialize);
                OnFullPlayerListUpdate(players);
            }
        }

        private void LobbyChatMessage(byte[] data)
        { 
            using (var bytes = new MemoryStream(data))
            {
                var cmid = Int32Proxy.Deserialize(bytes);
                var name = StringProxy.Deserialize(bytes);
                var message = StringProxy.Deserialize(bytes);

                OnLobbyChatMessage(cmid, name, message);
            }
        }

        public abstract void OnFullPlayerListUpdate(List<CommActorInfoView> players);

        public abstract void OnLobbyChatMessage(int cmid, string name, string message);
    }
}
