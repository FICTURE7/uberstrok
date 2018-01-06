using log4net;
using Photon.SocketServer;
using System.Collections.Generic;
using System.IO;

namespace UberStrok.Realtime.Server
{
    public abstract class BaseEventSender
    {
        private static ILog Log = LogManager.GetLogger(typeof(BaseEventSender));

        public BaseEventSender(BasePeer peer)
        {
            _peer = peer;
        }

        private readonly BasePeer _peer;
        protected BasePeer Peer => _peer;

        protected SendResult SendEvent(byte opCode, MemoryStream bytes)
        {
            var eventData = new EventData(opCode, new Dictionary<byte, object>
            {
                {0,  bytes.ToArray() }
            });

            var result =  _peer.SendEvent(eventData, new SendParameters());
            if (result != SendResult.Ok)
                Log.Error($"Send event failed {opCode} -> {result}");
            return result;
        }
    }
}
