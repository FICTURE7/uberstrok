using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class MemberViewProxy
    {
        public static MemberView Deserialize(Stream bytes)
        {
            var mask = Int32Proxy.Deserialize(bytes);
            var view = new MemberView();
            if ((mask & 1) != 0)
                view.MemberItems = ListProxy<int>.Deserialize(bytes, new ListProxy<int>.Deserializer<int>(Int32Proxy.Deserialize));
            if ((mask & 2) != 0)
                view.MemberWallet = MemberWalletViewProxy.Deserialize(bytes);
            if ((mask & 4) != 0)
                view.PublicProfile = PublicProfileViewProxy.Deserialize(bytes);

            return view;
        }

        public static void Serialize(Stream stream, MemberView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                if (instance.MemberItems != null)
                    ListProxy<int>.Serialize(bytes, instance.MemberItems, new ListProxy<int>.Serializer<int>(Int32Proxy.Serialize));
                else
                    mask |= 1;
                if (instance.MemberWallet != null)
                    MemberWalletViewProxy.Serialize(bytes, instance.MemberWallet);
                else
                    mask |= 2;
                if (instance.PublicProfile != null)
                    PublicProfileViewProxy.Serialize(bytes, instance.PublicProfile);
                else
                    mask |= 4;

                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }
    }
}
