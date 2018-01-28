using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class GameActorInfoViewProxy
    {
        public static void Serialize(Stream stream, GameActorInfoView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                EnumProxy<MemberAccessLevel>.Serialize(bytes, instance.AccessLevel);
                ByteProxy.Serialize(bytes, instance.ArmorPointCapacity);
                ByteProxy.Serialize(bytes, instance.ArmorPoints);
                EnumProxy<ChannelType>.Serialize(bytes, instance.Channel);

                if (instance.ClanTag != null)
                    StringProxy.Serialize(bytes, instance.ClanTag);
                else
                    mask |= 1;

                Int32Proxy.Serialize(bytes, instance.Cmid);
                EnumProxy<FireMode>.Serialize(bytes, instance.CurrentFiringMode);
                ByteProxy.Serialize(bytes, instance.CurrentWeaponSlot);
                Int16Proxy.Serialize(bytes, instance.Deaths);

                if (instance.FunctionalItems != null)
                    ListProxy<int>.Serialize(bytes, instance.FunctionalItems, new ListProxy<int>.Serializer<int>(Int32Proxy.Serialize));
                else
                    mask |= 2;
                if (instance.Gear != null)
                    ListProxy<int>.Serialize(bytes, instance.Gear, new ListProxy<int>.Serializer<int>(Int32Proxy.Serialize));
                else
                    mask |= 4;

                Int16Proxy.Serialize(bytes, instance.Health);
                Int16Proxy.Serialize(bytes, instance.Kills);
                Int32Proxy.Serialize(bytes, instance.Level);
                UInt16Proxy.Serialize(bytes, instance.Ping);
                ByteProxy.Serialize(bytes, instance.PlayerId);

                if (instance.PlayerName != null)
                    StringProxy.Serialize(bytes, instance.PlayerName);
                else
                    mask |= 8;

                EnumProxy<PlayerStates>.Serialize(bytes, instance.PlayerState);

                if (instance.QuickItems != null)
                    ListProxy<int>.Serialize(bytes, instance.QuickItems, new ListProxy<int>.Serializer<int>(Int32Proxy.Serialize));
                else
                    mask |= 16;

                ByteProxy.Serialize(bytes, instance.Rank);
                ColorProxy.Serialize(bytes, instance.SkinColor);
                EnumProxy<SurfaceType>.Serialize(bytes, instance.StepSound);
                EnumProxy<TeamID>.Serialize(bytes, instance.TeamID);

                if (instance.Weapons != null)
                    ListProxy<int>.Serialize(bytes, instance.Weapons, new ListProxy<int>.Serializer<int>(Int32Proxy.Serialize));
                else
                    mask |= 32;

                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static GameActorInfoView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var view = new GameActorInfoView();
            view.AccessLevel = EnumProxy<MemberAccessLevel>.Deserialize(bytes);
            view.ArmorPointCapacity = ByteProxy.Deserialize(bytes);
            view.ArmorPoints = ByteProxy.Deserialize(bytes);
            view.Channel = EnumProxy<ChannelType>.Deserialize(bytes);

            if ((mask & 1) != 0)
                view.ClanTag = StringProxy.Deserialize(bytes);

            view.Cmid = Int32Proxy.Deserialize(bytes);
            view.CurrentFiringMode = EnumProxy<FireMode>.Deserialize(bytes);
            view.CurrentWeaponSlot = ByteProxy.Deserialize(bytes);
            view.Deaths = Int16Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
                view.FunctionalItems = ListProxy<int>.Deserialize(bytes, new ListProxy<int>.Deserializer<int>(Int32Proxy.Deserialize));
            if ((mask & 4) != 0)
                view.Gear = ListProxy<int>.Deserialize(bytes, new ListProxy<int>.Deserializer<int>(Int32Proxy.Deserialize));

            view.Health = Int16Proxy.Deserialize(bytes);
            view.Kills = Int16Proxy.Deserialize(bytes);
            view.Level = Int32Proxy.Deserialize(bytes);
            view.Ping = UInt16Proxy.Deserialize(bytes);
            view.PlayerId = ByteProxy.Deserialize(bytes);

            if ((mask & 8) != 0)
                view.PlayerName = StringProxy.Deserialize(bytes);

            view.PlayerState = EnumProxy<PlayerStates>.Deserialize(bytes);

            if ((mask & 16) != 0)
                view.QuickItems = ListProxy<int>.Deserialize(bytes, new ListProxy<int>.Deserializer<int>(Int32Proxy.Deserialize));

            view.Rank = ByteProxy.Deserialize(bytes);
            view.SkinColor = ColorProxy.Deserialize(bytes);
            view.StepSound = EnumProxy<SurfaceType>.Deserialize(bytes);
            view.TeamID = EnumProxy<TeamID>.Deserialize(bytes);

            if ((mask & 32) != 0)
                view.Weapons = ListProxy<int>.Deserialize(bytes, new ListProxy<int>.Deserializer<int>(Int32Proxy.Deserialize));

            return view;
        }
    }
}
