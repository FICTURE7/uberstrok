using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeerEvents : EventSender
    {
        public GameRoomEvents Game { get; }

        public GamePeerEvents(GamePeer peer) 
            : base(peer)
        {
            Game = new GameRoomEvents(peer);
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

        public void SendFullGameList(List<GameRoomDataView> gameList)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<GameRoomDataView>.Serialize(bytes, gameList, GameRoomDataViewProxy.Serialize);
                SendEvent((byte)IGamePeerEventsType.FullGameList, bytes);
            }
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

        public void SendRequestPasswordForRoom(string server, int roomId)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, server);
                Int32Proxy.Serialize(bytes, roomId);

                SendEvent((byte)IGamePeerEventsType.RequestPasswordForRoom, bytes);
            }
        }

        public void SendRoomEnterFailed(string server, int roomId, string message)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, server);
                Int32Proxy.Serialize(bytes, roomId);
                StringProxy.Serialize(bytes, message);

                SendEvent((byte)IGamePeerEventsType.RoomEnterFailed, bytes);
            }
        }

        public void SendHeartbeatChallenge(string challengeHash)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, challengeHash);
                SendEvent((byte)IGamePeerEventsType.HeartbeatChallenge, bytes);
            }
        }

        public void SendDisconnectAndDisablePhoton(string message)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);
                SendEvent((byte)IGamePeerEventsType.DisconnectAndDisablePhoton, bytes);
            }
        }
    }
}
