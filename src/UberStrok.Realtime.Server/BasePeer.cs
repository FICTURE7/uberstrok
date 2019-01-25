using log4net;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UberStrok.Realtime.Server
{
    public class BasePeer : ClientPeer
    {
        private readonly static ILog _log = LogManager.GetLogger(nameof(BasePeer));
        private readonly Dictionary<int, BaseOperationHandler> _opHandlers;

        public BasePeer(InitRequest initRequest) : base(initRequest)
        {
            /* Check the client version. */
            if (initRequest.ApplicationId != RealtimeVersion.Current)
                Disconnect();

            _opHandlers = new Dictionary<int, BaseOperationHandler>();
        }

        protected void AddOperationHandler(BaseOperationHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _opHandlers.Add(handler.Id, handler);
        }

        protected void RemoveOperationHandler(int handlerId) => _opHandlers.Remove(handlerId);

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            foreach (var opHandler in _opHandlers.Values)
            {
                try { opHandler.OnDisconnect(this, reasonCode, reasonDetail); }
                catch (Exception ex)
                {
                    _log.Error($"Error while handling disconnection of peer -> {opHandler.GetType().Name}", ex);
                }
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            /* 
                OperationRequest should contain 1 parameter.
                [0] -> (Int32 - OperationHandler ID) ->> (Byte[] - Data).

                Then we use OperationRequest.OperationCode & OperationHandler ID to, 
                determine how to read stuff.

                Check if we've got enough parameters.
             */
            if (operationRequest.Parameters.Count < 1)
            {
                _log.Warn("Client sent operation request without enough parameters, disconnecting.");
                Disconnect();
                return;
            }

            var handlerId = operationRequest.Parameters.Keys.First();
            var handler = default(BaseOperationHandler);
            if (_opHandlers.TryGetValue(handlerId, out handler))
            {
                var data = (byte[])operationRequest.Parameters[handlerId];
                using (var bytes = new MemoryStream(data))
                {
                    try { handler.OnOperationRequest(this, operationRequest.OperationCode, bytes); }
                    catch (Exception ex)
                    {
                        _log.Error($"Error while handling request {handler.GetType().Name}:{handlerId} -> OpCode: {operationRequest.OperationCode}", ex);
                    }
                }
            }
            else
            {
                _log.Warn($"Unable to handle operation request -> not implemented operation handler: {handlerId}");
            }
        }
    }
}
