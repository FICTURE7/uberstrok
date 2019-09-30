using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class MemberSessionDataViewProxy
    {
        public static void Serialize(Stream stream, MemberSessionDataView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                EnumProxy<MemberAccessLevel>.Serialize(bytes, instance.AccessLevel);

                if (instance.AuthToken != null)
                    StringProxy.Serialize(bytes, instance.AuthToken);
                else
                    mask |= 1;

                EnumProxy<ChannelType>.Serialize(bytes, instance.Channel);

                if (instance.ClanTag != null)
                    StringProxy.Serialize(bytes, instance.ClanTag);
                else
                    mask |= 2;

                Int32Proxy.Serialize(bytes, instance.Cmid);
                BooleanProxy.Serialize(bytes, instance.IsBanned);
                Int32Proxy.Serialize(bytes, instance.Level);
                DateTimeProxy.Serialize(bytes, instance.LoginDate);

                if (instance.Name != null)
                    StringProxy.Serialize(bytes, instance.Name);
                else
                    mask |= 4;

                Int32Proxy.Serialize(bytes, instance.XP);
                Int32Proxy.Serialize(stream, ~mask);

                bytes.WriteTo(stream);
            }
        }

        public static MemberSessionDataView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var view = new MemberSessionDataView();
            view.AccessLevel = EnumProxy<MemberAccessLevel>.Deserialize(bytes);

            if ((mask & 1) != 0)
                view.AuthToken = StringProxy.Deserialize(bytes);

            view.Channel = EnumProxy<ChannelType>.Deserialize(bytes);

            if ((mask & 2) != 0)
                view.ClanTag = StringProxy.Deserialize(bytes);

            view.Cmid = Int32Proxy.Deserialize(bytes);
            view.IsBanned = BooleanProxy.Deserialize(bytes);
            view.Level = Int32Proxy.Deserialize(bytes);
            view.LoginDate = DateTimeProxy.Deserialize(bytes);

            if ((mask & 4) != 0)
                view.Name = StringProxy.Deserialize(bytes);

            view.XP = Int32Proxy.Deserialize(bytes);
            return view;
        }
    }
}
