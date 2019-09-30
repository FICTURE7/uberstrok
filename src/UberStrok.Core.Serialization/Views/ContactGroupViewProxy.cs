using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ContactGroupViewProxy
    {
        public static void Serialize(Stream stream, ContactGroupView instance)
        {
            int mask = 0;
            using (MemoryStream bytes = new MemoryStream())
            {
                if (instance.Contacts != null)
                    ListProxy<PublicProfileView>.Serialize(bytes, instance.Contacts, PublicProfileViewProxy.Serialize);
                else
                    mask |= 1;

                Int32Proxy.Serialize(bytes, instance.GroupId);

                if (instance.GroupName != null)
                    StringProxy.Serialize(bytes, instance.GroupName);
                else
                    mask |= 2;

                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static ContactGroupView Deserialize(Stream bytes)
        {
            int num = Int32Proxy.Deserialize(bytes);
            var instance = new ContactGroupView();

            if ((num & 1) != 0)
                instance.Contacts = ListProxy<PublicProfileView>.Deserialize(bytes, PublicProfileViewProxy.Deserialize);

            instance.GroupId = Int32Proxy.Deserialize(bytes);

            if ((num & 2) != 0)
                instance.GroupName = StringProxy.Deserialize(bytes);

            return instance;
        }
    }
}
