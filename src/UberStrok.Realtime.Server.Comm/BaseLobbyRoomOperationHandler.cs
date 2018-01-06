using System.IO;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Server.Comm
{
    public abstract class BaseLobbyRoomOperationHandler : BaseOperationHandler<CommPeer>
    {
        public BaseLobbyRoomOperationHandler(CommPeer peer) : base(peer)
        {
            // Space
        }

        public override int Id => 0;

        public abstract void OnChatMessageToAll(string message);

        public override void OnOperationRequest(byte opCode, MemoryStream bytes)
        {
            var operation = (ILobbyRoomOperationsType)opCode;
            switch (operation)
            {
                case ILobbyRoomOperationsType.ChatMessageToAll:
                    ChatMessageToAll(bytes);
                    break;
            }
        }

        private void ChatMessageToAll(MemoryStream bytes)
        {
            var message = StringProxy.Deserialize(bytes);

            OnChatMessageToAll(message);
        }
    }
}