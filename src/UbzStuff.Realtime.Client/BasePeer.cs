using ExitGames.Client.Photon;
using System;
using System.Threading;

namespace UbzStuff.Realtime.Client
{
    // Client -> Server peer.
    public abstract class BasePeer : IDisposable
    {
        protected BasePeer()
        {
            _listener = new PhotonPeerListener(this);
            _peer = new PhotonPeer(_listener, ConnectionProtocol.Udp);

            StartWorkerThread();
        }

        private bool _disposed;
        private bool _active;

        private readonly PhotonPeerListener _listener;
        internal readonly PhotonPeer _peer;

        private Thread _workerThread;

        public void Connect(string endPoint)
        {
            if (!_peer.Connect(endPoint, RealtimeVersion.Current))
                OnConnectFailed(endPoint);
        }

        public void Disconnect()
        {
            _peer.SendOutgoingCommands();
            _peer.Disconnect();
        }

        protected virtual void OnConnectFailed(string endPoint) { }

        protected internal abstract void OnConnect(string endPoint);

        protected internal abstract void OnError(string message);

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                StopWorkerThread();
                Disconnect();
            }

            _disposed = true;
        }

        internal void AddDispatcher(IEventDispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            _listener._dispatchers.Add(dispatcher);
        }

        internal void RemoveDispatcher(IEventDispatcher dispatcher)
        {
            _listener._dispatchers.Remove(dispatcher);
        }

        private void StartWorkerThread()
        {
            // Don't start thread more than once.
            if (_active)
                return;

            _active = true;

            _workerThread = new Thread(DoWork);
            _workerThread.Start();
        }

        private void StopWorkerThread()
        {
            _active = false;
        }

        private void DoWork()
        {
            while (_active)
            {
                _peer.SendAcksOnly();
                _peer.Service();

                Thread.Sleep(100);
            }
        }
    }
}
