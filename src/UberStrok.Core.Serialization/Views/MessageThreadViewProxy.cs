using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class MessageThreadViewProxy
    {
        public static void Serialize(Stream stream, MessageThreadView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                BooleanProxy.Serialize(bytes, instance.HasNewMessages);

                if (instance.LastMessagePreview != null)
                    StringProxy.Serialize(bytes, instance.LastMessagePreview);
                else
                    mask |= 1;

                DateTimeProxy.Serialize(bytes, instance.LastUpdate);
                Int32Proxy.Serialize(bytes, instance.MessageCount);
                Int32Proxy.Serialize(bytes, instance.ThreadId);

                if (instance.ThreadName != null)
                    StringProxy.Serialize(bytes, instance.ThreadName);
                else
                    mask |= 2;

                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static MessageThreadView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var view = new MessageThreadView();
            view.HasNewMessages = BooleanProxy.Deserialize(bytes);

            if ((mask & 1) != 0)
                view.LastMessagePreview = StringProxy.Deserialize(bytes);

            view.LastUpdate = DateTimeProxy.Deserialize(bytes);
            view.MessageCount = Int32Proxy.Deserialize(bytes);
            view.ThreadId = Int32Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
                view.ThreadName = StringProxy.Deserialize(bytes);

            return view;
        }
    }
}
