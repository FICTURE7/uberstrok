using System.IO;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Server.Comm
{
    public abstract class BaseCommPeerOperationHandler /*: BaseOperationHandler<CommPeer> */
    {
        /*
        protected BaseCommPeerOperationHandler(CommPeer peer) : base(peer)
        {
            // Space
        }

        public override int Id => 1;

        public abstract void OnAuthenticationRequest(string authToken, string magicHash);
        public abstract void OnSendHeartbeatResponse(string authToken, string responseHash);

        public override void OnOperationRequest(byte opCode, MemoryStream bytes)
        {
            var operation = (ICommPeerOperationsType)opCode;
            switch (operation)
            {
                case ICommPeerOperationsType.AuthenticationRequest:
                    AuthenticationRequest(bytes);
                    break;

                case ICommPeerOperationsType.SendHeartbeatResponse:
                    SendHeartbeatResponse(bytes);
                    break;
            }
        }

        private void AuthenticationRequest(MemoryStream bytes)
        {
            var authToken = StringProxy.Deserialize(bytes);
            var magicHash = StringProxy.Deserialize(bytes);

            OnAuthenticationRequest(authToken, magicHash);
        }

        private void SendHeartbeatResponse(MemoryStream bytes)
        {
            var authToken = StringProxy.Deserialize(bytes);
            var responseHash = StringProxy.Deserialize(bytes);

            OnSendHeartbeatResponse(authToken, responseHash);
        }
        */
    }
}
