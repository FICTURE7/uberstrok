using System.Collections.Generic;

namespace UberStrok.Realtime.Server
{
    public interface IRoom<T> where T : BasePeer
    {
        IReadOnlyList<T> Peers { get; }

        void Join(T peer);

        void Leave(T peer);
    }
}
