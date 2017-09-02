using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UbzStuff.Realtime.Server
{
    // Server -> Client peer.
    public class BasePeer : ClientPeer
    {
        public BasePeer(InitRequest initRequest) : base(initRequest)
        {
            // Check the client version.
            if (initRequest.ApplicationId != RealtimeVersion.Current)
                Disconnect();

            _opHandlers = new Dictionary<int, BaseOperationHandler>();
        }

        private readonly Dictionary<int, BaseOperationHandler> _opHandlers;

        protected void AddOpHandler(BaseOperationHandler handler) 
        {
            _opHandlers.Add(handler.Id, handler);
        }

        protected void RemoteOpHandler(int id)
        {
            _opHandlers.Remove(id);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            foreach (var opHandler in _opHandlers.Values)
                opHandler.OnDisconnect(reasonCode, reasonDetail);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // Check if we've got enough parameters.
            if (operationRequest.Parameters.Count < 1)
            {
                Disconnect();
                return;
            }

            var opHandlerId = operationRequest.Parameters.Keys.First();
            var handler = default(BaseOperationHandler);
            if (_opHandlers.TryGetValue(opHandlerId, out handler))
            {
                var data = (byte[])operationRequest.Parameters[opHandlerId];
                using (var bytes = new MemoryStream(data))
                    handler.OnOperationRequest(operationRequest.OperationCode, bytes);
            }
        }
    }
}
