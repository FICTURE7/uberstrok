using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoomEvents : EventSender
    {
        public LobbyRoomEvents(Peer peer) : base(peer)
        {
            // Space
        }

        public void SendFullPlayerListUpdate(List<CommActorInfoView> actors)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<CommActorInfoView>.Serialize(bytes, actors, CommActorInfoViewProxy.Serialize);
                SendEvent((byte)ILobbyRoomEventsType.FullPlayerListUpdate, bytes);
            }
        }

        public void SendLobbyChatMessage(int cmid, string name, string message)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);

                SendEvent((byte)ILobbyRoomEventsType.LobbyChatMessage, bytes);
            }
        }
    }
}
