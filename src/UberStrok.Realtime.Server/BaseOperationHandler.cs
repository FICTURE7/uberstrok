using PhotonHostRuntimeInterfaces;
using System.IO;

namespace UberStrok.Realtime.Server
{
    public abstract class BaseOperationHandler
    {
        public abstract int Id { get; }

        public abstract void OnOperationRequest(BasePeer peer, byte opCode, MemoryStream bytes);

        public virtual void OnDisconnect(BasePeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            // Space
        }
    }

    // Just to save us from casting some stuff.
    public abstract class BaseOperationHandler<TPeer> : BaseOperationHandler where TPeer : BasePeer
    {
        public virtual void OnDisconnect(TPeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            // Space
        }

        public sealed override void OnDisconnect(BasePeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            OnDisconnect((TPeer)peer, reasonCode, reasonDetail);
        }

        public abstract void OnOperationRequest(TPeer peer, byte opCode, MemoryStream bytes);

        public sealed override void OnOperationRequest(BasePeer peer, byte opCode, MemoryStream bytes)
        {
            OnOperationRequest((TPeer)peer, opCode, bytes);
        }
    }
}
