using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Client
{
    public class CommPeerOperations
    {
        public CommPeerOperations(BasePeer peer, byte id)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _id = id;
            _peer = peer;
        }

        private byte _id;
        private readonly BasePeer _peer;

        public void SendAuthenticationRequest(string authToken, string magicHash)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);
                StringProxy.Serialize(bytes, magicHash);

                var param = new Dictionary<byte, object>
                {
                    {
                        _id,
                        bytes.ToArray()
                    }
                };

                _peer._peer.OpCustom((byte)ICommPeerOperationsType.AuthenticationRequest, param, true, 0, false); 
            }
        }
    }
}
