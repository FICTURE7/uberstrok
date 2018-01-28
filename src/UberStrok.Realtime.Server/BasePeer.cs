using log4net;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace UberStrok.Realtime.Server
{
    public class BasePeer : ClientPeer
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(BasePeer));

        private readonly ConcurrentDictionary<int, BaseOperationHandler> _opHandlers;

        public BasePeer(InitRequest initRequest) : base(initRequest)
        {
            /* Check the client version. */
            if (initRequest.ApplicationId != RealtimeVersion.Current)
                Disconnect();

            _opHandlers = new ConcurrentDictionary<int, BaseOperationHandler>();
        }

        public void AddOperationHandler(BaseOperationHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!_opHandlers.TryAdd(handler.Id, handler))
                throw new ArgumentException("Already contains a handler with the same handler ID.");
        }

        public void RemoveOperationHandler(int handlerId)
        {
            var handler = default(BaseOperationHandler);
            _opHandlers.TryRemove(handlerId, out handler);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            foreach (var opHandler in _opHandlers.Values)
            {
                try { opHandler.OnDisconnect(this, reasonCode, reasonDetail); }
                catch (Exception ex)
                {
                    s_log.Error($"Error while handling disconnection of peer -> {opHandler.GetType().Name}", ex);
                }
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            /* 
                OperationRequest should contain 1 parameters.
                [0] -> (Int32 - OperationHandler ID) ->> (Byte[] - Data).

                Then we use OperationRequest.OperationCode & OperationHandler ID to,
                determine how to read stuff.

                Check if we've got enough parameters.
             */
            if (operationRequest.Parameters.Count < 1)
            {
                s_log.Warn("Disconnecting client since its does not have enough parameters!");
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
                        s_log.Error($"Error while handling request {handler.GetType().Name}:{handlerId} -> OpCode: {operationRequest.OperationCode}", ex);
                    }
                }
            }
            else
            {
                s_log.Warn($"Unable to handle operation request -> not implemented operation handler: {handlerId}");
            }
        }
    }
}
