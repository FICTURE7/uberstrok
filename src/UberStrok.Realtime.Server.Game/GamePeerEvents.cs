using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeerEvents : BaseEventSender
    {
        public GamePeerEvents(BasePeer peer) : base(peer)
        {
            // Space
        }

        public void SendGameListUpdate(List<GameRoomDataView> updatedGames, List<int> removedGames)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<GameRoomDataView>.Serialize(bytes, updatedGames, GameRoomDataViewProxy.Serialize);
                ListProxy<int>.Serialize(bytes, removedGames, Int32Proxy.Serialize);

                SendEvent((byte)IGamePeerEventsType.GameListUpdate, bytes);
            }
        }

        public void SendGameListUpdateEnd()
        {
            using (var bytes = new MemoryStream())
                SendEvent((byte)IGamePeerEventsType.GameListUpdateEnd, bytes);
        }

        public void SendServerLoadData(PhotonServerLoadView view)
        {
            using (var bytes = new MemoryStream())
            {
                PhotonServerLoadViewProxy.Serialize(bytes, view);
                SendEvent((byte)IGamePeerEventsType.ServerLoadData, bytes);
            }
        }

        public void SendRoomEntered(GameRoomDataView view)
        {
            using (var bytes = new MemoryStream())
            {
                GameRoomDataViewProxy.Serialize(bytes, view);
                SendEvent((byte)IGamePeerEventsType.RoomEntered, bytes);
            }
        }
    }
}
