using Photon.SocketServer;
using System.Collections.Generic;
using System.IO;

namespace UbzStuff.Realtime.Server
{
    public abstract class BaseEventSender
    {
        public BaseEventSender(BasePeer peer)
        {
            _peer = peer;
        }

        private readonly BasePeer _peer;
        protected BasePeer Peer => _peer;

        protected void SendEvent(byte opCode, MemoryStream bytes)
        {
            var eventData = new EventData(opCode, new Dictionary<byte, object>
            {
                {0,  bytes.ToArray() }
            });
            _peer.SendEvent(eventData, new SendParameters());
        }
    }
}
