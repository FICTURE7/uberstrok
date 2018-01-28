using log4net;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.IO;

namespace UberStrok.Realtime.Server
{
    /// <summary>
    /// Provides methods to send events.
    /// </summary>
    public abstract class BaseEventSender
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(BaseEventSender));

        private readonly BasePeer _peer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEventSender"/> class with the specified
        /// <see cref="BasePeer"/>.
        /// </summary>
        /// <param name="peer"><see cref="BasePeer"/> to use.</param>
        protected BaseEventSender(BasePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peer = peer;
        }

        protected BasePeer Peer => _peer;

        protected SendResult SendEvent(byte opCode, MemoryStream bytes, bool unreliable)
        {
            var eventData = new EventData(opCode, new Dictionary<byte, object>
            {
                {0,  bytes.ToArray() }
            });

            var result = _peer.SendEvent(eventData, new SendParameters { Unreliable = unreliable });
            if (result != SendResult.Ok)
                s_log.Error($"Send event failed {opCode} -> {result}");

            return result;
        }

        protected SendResult SendEvent(byte opCode, MemoryStream bytes) => SendEvent(opCode, bytes, false);
    }
}
