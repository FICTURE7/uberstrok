using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public class PrivateMessageViewProxy
    {
        public static void Serialize(Stream stream, PrivateMessageView instance)
        {
            int mask = 0;
            using (MemoryStream bytes = new MemoryStream())
            {
                if (instance.ContentText != null)
                    StringProxy.Serialize(bytes, instance.ContentText);
                else
                    mask |= 1;

                DateTimeProxy.Serialize(bytes, instance.DateSent);
                Int32Proxy.Serialize(bytes, instance.FromCmid);

                if (instance.FromName != null)
                    StringProxy.Serialize(bytes, instance.FromName);
                else
                    mask |= 2;

                BooleanProxy.Serialize(bytes, instance.HasAttachment);
                BooleanProxy.Serialize(bytes, instance.IsDeletedByReceiver);
                BooleanProxy.Serialize(bytes, instance.IsDeletedBySender);
                BooleanProxy.Serialize(bytes, instance.IsRead);
                Int32Proxy.Serialize(bytes, instance.PrivateMessageId);
                Int32Proxy.Serialize(bytes, instance.ToCmid);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static PrivateMessageView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var instance = new PrivateMessageView();

            if ((mask & 1) != 0)
                instance.ContentText = StringProxy.Deserialize(bytes);

            instance.DateSent = DateTimeProxy.Deserialize(bytes);
            instance.FromCmid = Int32Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
                instance.FromName = StringProxy.Deserialize(bytes);

            instance.HasAttachment = BooleanProxy.Deserialize(bytes);
            instance.IsDeletedByReceiver = BooleanProxy.Deserialize(bytes);
            instance.IsDeletedBySender = BooleanProxy.Deserialize(bytes);
            instance.IsRead = BooleanProxy.Deserialize(bytes);
            instance.PrivateMessageId = Int32Proxy.Deserialize(bytes);
            instance.ToCmid = Int32Proxy.Deserialize(bytes);
            return instance;
        }
    }
}
