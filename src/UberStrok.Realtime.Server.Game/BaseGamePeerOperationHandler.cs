using System;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGamePeerOperationHandler : BaseOperationHandler<GamePeer>
    {
        public BaseGamePeerOperationHandler(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override int Id => 1;

        protected abstract void OnGetGameListUpdates();
        protected abstract void OnGetServerLoad();
        protected abstract void OnCreateRoom(GameRoomDataView roomData, string password, string clientVersion, string authToken, string magicHash);
        protected abstract void OnJoinRoom(int roomId, string password, string clientVersion, string authToken, string magicHash);
        protected abstract void OnLeaveRoom();
        protected abstract void OnUpdatePing(ushort ping);

        public override void OnOperationRequest(byte opCode, MemoryStream bytes)
        {
            var operation = (IGamePeerOperationsType)opCode;
            switch (operation)
            {
                case IGamePeerOperationsType.GetGameListUpdates:
                    GetGameListUpdates(bytes);
                    break;

                case IGamePeerOperationsType.GetServerLoad:
                    GetServerLoad(bytes);
                    break;

                case IGamePeerOperationsType.CreateRoom:
                    CreateRoom(bytes);
                    break;

                case IGamePeerOperationsType.EnterRoom:
                    EnterRoom(bytes);
                    break;

                case IGamePeerOperationsType.LeaveRoom:
                    LeaveRoom(bytes);
                    break;

                case IGamePeerOperationsType.UpdatePing:
                    UpdatePing(bytes);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void GetGameListUpdates(MemoryStream bytes)
        {
            OnGetGameListUpdates();
        }

        private void GetServerLoad(MemoryStream bytes)
        {
            OnGetServerLoad();
        }

        private void CreateRoom(MemoryStream bytes)
        {
            var roomData = GameRoomDataViewProxy.Deserialize(bytes);
            var password = StringProxy.Deserialize(bytes);
            var clientVersion = StringProxy.Deserialize(bytes);
            var authToken = StringProxy.Deserialize(bytes);
            var magicHash = StringProxy.Deserialize(bytes);

            OnCreateRoom(roomData, password, clientVersion, authToken, magicHash);
        }

        private void EnterRoom(MemoryStream bytes)
        {
            var roomId = Int32Proxy.Deserialize(bytes);
            var password = StringProxy.Deserialize(bytes);
            var clientVersion = StringProxy.Deserialize(bytes);
            var authToken = StringProxy.Deserialize(bytes);
            var magicHash = StringProxy.Deserialize(bytes);

            OnJoinRoom(roomId, password, clientVersion, authToken, magicHash);
        }

        private void LeaveRoom(MemoryStream bytes)
        {
            OnLeaveRoom();
        }

        private void UpdatePing(MemoryStream bytes)
        {
            var ping = UInt16Proxy.Deserialize(bytes);

            OnUpdatePing(ping);
        }
    }
}
