using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ContactRequestViewProxy
    {
        public static void Serialize(Stream stream, ContactRequestView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, instance.InitiatorCmid);

                if (instance.InitiatorMessage != null)
                    StringProxy.Serialize(bytes, instance.InitiatorMessage);
                else
                    mask |= 1;

                if (instance.InitiatorName != null)
                    StringProxy.Serialize(bytes, instance.InitiatorName);
                else
                    mask |= 2;

                Int32Proxy.Serialize(bytes, instance.ReceiverCmid);
                Int32Proxy.Serialize(bytes, instance.RequestId);
                DateTimeProxy.Serialize(bytes, instance.SentDate);
                EnumProxy<ContactRequestStatus>.Serialize(bytes, instance.Status);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static ContactRequestView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var instance = new ContactRequestView();
            instance.InitiatorCmid = Int32Proxy.Deserialize(bytes);

            if ((mask & 1) != 0)
                instance.InitiatorMessage = StringProxy.Deserialize(bytes);

            if ((mask & 2) != 0)
                instance.InitiatorName = StringProxy.Deserialize(bytes);

            instance.ReceiverCmid = Int32Proxy.Deserialize(bytes);
            instance.RequestId = Int32Proxy.Deserialize(bytes);
            instance.SentDate = DateTimeProxy.Deserialize(bytes);
            instance.Status = EnumProxy<ContactRequestStatus>.Deserialize(bytes);
            return instance;
        }
    }
}
