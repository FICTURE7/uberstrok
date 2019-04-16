using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UberStrok.Realtime.Server
{
    public class OperationHandlerCollection : IEnumerable<OperationHandler>
    {
        private readonly ConcurrentDictionary<byte, OperationHandler> _opHandlers;

        public OperationHandler this[byte id]
        {
            get
            {
                _opHandlers.TryGetValue(id, out OperationHandler handler);
                return handler;
            }
        }

        public int Count { get => _opHandlers.Count; }

        public OperationHandlerCollection()
        {
            _opHandlers = new ConcurrentDictionary<byte, OperationHandler>();
        }

        public void Add(OperationHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!_opHandlers.TryAdd(handler.Id, handler))
                throw new InvalidOperationException("Already contains an OperationHandler with the same ID");
        }

        public bool Remove(byte id)
        {
            return _opHandlers.TryRemove(id, out OperationHandler handler);
        }

        public IEnumerator<OperationHandler> GetEnumerator()
        {
            return _opHandlers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _opHandlers.Values.GetEnumerator();
        }
    }
}
