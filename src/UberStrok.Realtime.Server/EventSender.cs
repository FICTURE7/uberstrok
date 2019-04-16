using log4net;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.IO;

namespace UberStrok.Realtime.Server
{
    public abstract class EventSender
    {
        private readonly static ILog Log = LogManager.GetLogger(nameof(EventSender));

        protected Peer Peer { get; }

        protected EventSender(Peer peer)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
        }

        protected SendResult SendEvent(byte opCode, MemoryStream bytes, bool unreliable)
        {
            var eventData = new EventData(opCode, new Dictionary<byte, object>
            {
                {0,  bytes.ToArray() }
            });

            var result = Peer.SendEvent(eventData, new SendParameters { Unreliable = unreliable });
            if (result != SendResult.Ok)
                Log.Error($"Send event failed {opCode} -> {result}");

            return result;
        }

        protected SendResult SendEvent(byte opCode, MemoryStream bytes) => SendEvent(opCode, bytes, false);
    }
}
