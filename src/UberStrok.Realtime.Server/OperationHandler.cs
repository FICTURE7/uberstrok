using log4net;
using PhotonHostRuntimeInterfaces;
using System;
using System.IO;

namespace UberStrok.Realtime.Server
{
    public abstract class OperationHandler
    {
        public abstract byte Id { get; }

        protected ILog Log { get; }

        protected OperationHandler()
        {
            Log = LogManager.GetLogger(GetType().Name);
        }

        public abstract void OnOperationRequest(Peer peer, byte opCode, MemoryStream bytes);

        public virtual void OnDisconnect(Peer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            // Space
        }

        protected virtual void Enqueue(Action action)
        {
            action();
        }
    }

    public abstract class OperationHandler<TPeer> : OperationHandler where TPeer : Peer
    {
        public virtual void OnDisconnect(TPeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            // Space
        }

        public sealed override void OnDisconnect(Peer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            OnDisconnect((TPeer)peer, reasonCode, reasonDetail);
        }

        public abstract void OnOperationRequest(TPeer peer, byte opCode, MemoryStream bytes);

        public sealed override void OnOperationRequest(Peer peer, byte opCode, MemoryStream bytes)
        {
            OnOperationRequest((TPeer)peer, opCode, bytes);
        }
    }
}
