using System.Collections.Generic;

namespace UberStrok.Realtime.Server
{
    public interface IRoom<TPeer> where TPeer : Peer
    {
        IReadOnlyList<TPeer> Peers { get; }

        void Join(TPeer peer);
        void Leave(TPeer peer);
    }
}
