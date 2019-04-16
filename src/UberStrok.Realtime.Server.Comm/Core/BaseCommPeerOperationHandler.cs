using System;
using System.IO;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Server.Comm
{
    public abstract class BaseCommPeerOperationHandler : OperationHandler<CommPeer>
    {
        protected BaseCommPeerOperationHandler()
        {
            // Space
        }

        public override byte Id => 1;

        public abstract void OnAuthenticationRequest(CommPeer peer, string authToken, string magicHash);
        public abstract void OnSendHeartbeatResponse(CommPeer peer, string authToken, string responseHash);

        public override void OnOperationRequest(CommPeer peer, byte opCode, MemoryStream bytes)
        {
            var operation = (ICommPeerOperationsType)opCode;
            switch (operation)
            {
                case ICommPeerOperationsType.AuthenticationRequest:
                    AuthenticationRequest(peer, bytes);
                    break;

                case ICommPeerOperationsType.SendHeartbeatResponse:
                    SendHeartbeatResponse(peer, bytes);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void AuthenticationRequest(CommPeer peer, MemoryStream bytes)
        {
            var authToken = StringProxy.Deserialize(bytes);
            var magicHash = StringProxy.Deserialize(bytes);

            OnAuthenticationRequest(peer, authToken, magicHash);
        }

        private void SendHeartbeatResponse(CommPeer peer, MemoryStream bytes)
        {
            var authToken = StringProxy.Deserialize(bytes);
            var responseHash = StringProxy.Deserialize(bytes);

            OnSendHeartbeatResponse(peer, authToken, responseHash);
        }
    }
}
