using PhotonHostRuntimeInterfaces;
using System;
using System.IO;

namespace UbzStuff.Realtime.Server
{
    public abstract class BaseOperationHandler
    {
        public abstract int Id { get; }

        public abstract void OnOperationRequest(byte opCode, MemoryStream bytes);

        public virtual void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            // Space
        }
    }

    // Just to save us from casting some stuff.
    public abstract class BaseOperationHandler<TPeer> : BaseOperationHandler where TPeer : BasePeer
    {
        public BaseOperationHandler(TPeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peer = peer;
        }

        protected TPeer Peer => _peer;

        private readonly TPeer _peer;
    }
}
