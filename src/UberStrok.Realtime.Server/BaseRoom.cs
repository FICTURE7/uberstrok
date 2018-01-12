using System;
using System.Collections.Generic;

namespace UberStrok.Realtime.Server
{
    public interface IRoom<T> where T : BasePeer
    {
        IReadOnlyCollection<T> Peers { get; }

        void OnJoin(T peer);

        void OnLeave(T peer);
    }

    [Obsolete]
    public abstract class BaseRoom<T> where T : BasePeer
    {
        private readonly List<T> _peers;
        private readonly IReadOnlyCollection<T> _peersReadOnly;

        public BaseRoom()
        {
            _peers = new List<T>();
            _peersReadOnly = _peers.AsReadOnly();
        }

        public IReadOnlyCollection<T> Peers => _peersReadOnly;

        public virtual void OnJoin(T peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peers.Add(peer);
        }

        public virtual void OnLeave(T peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peers.Remove(peer);
        }
    }
}
