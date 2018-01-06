using System;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace UberStrok.Realtime.Client
{
    internal class PhotonPeerListener : IPhotonPeerListener
    {
        public PhotonPeerListener(BasePeer peer)
        {
            _peer = peer;
            _dispatchers = new List<IEventDispatcher>();
        }

        private readonly BasePeer _peer;
        internal readonly List<IEventDispatcher> _dispatchers;

        public void DebugReturn(DebugLevel level, string message)
        {
            Console.WriteLine($"[{level.ToString()}] - {message}");
        }

        public void OnEvent(EventData eventData)
        {
            for (int i = 0; i < _dispatchers.Count; i++)
            {
                var dispatcher = _dispatchers[i];
                try
                {
                    dispatcher.OnEvent(eventData.Code, (byte[])eventData.Parameters[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while dispatching event: " + ex);
                }
            }
        }

        public void OnMessage(object messages)
        {
            // Space
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            // Space
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    _peer.OnConnect(string.Empty);
                    break;
            }
        }
    }
}