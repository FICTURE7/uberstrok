using System.Collections.Generic;
using System.IO;
using UbzStuff.Core.Serialization;
using UbzStuff.Core.Serialization.Views;
using UbzStuff.Core.Views;

namespace UbzStuff.Realtime.Server.Comm
{
    public class LobbyRoomEvents : BaseEventSender
    {
        public LobbyRoomEvents(BasePeer peer) : base(peer)
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
