using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Client
{
    public class LobbyRoomOperations
    {
        public LobbyRoomOperations(BasePeer peer, byte id)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _id = id;
            _peer = peer;
        }

        private byte _id;
        private readonly BasePeer _peer;

        public void SendChatToAll(string message)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);

                var parameter = new Dictionary<byte, object>
                {
                    {_id, bytes.ToArray() }
                };
                _peer._peer.OpCustom((byte)ILobbyRoomOperationsType.ChatMessageToAll, parameter, true);
            }
        }
    }
}
