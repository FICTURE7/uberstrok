using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class GameActorInfoDeltaViewProxy
    {
        public static void Serialize(Stream stream, GameActorInfoDeltaView instance)
        {
            if (instance != null)
            {
                Int32Proxy.Serialize(stream, instance.DeltaMask);
                ByteProxy.Serialize(stream, instance.PlayerId);

                if ((instance.DeltaMask & 1) != 0)
                    EnumProxy<MemberAccessLevel>.Serialize(stream, (MemberAccessLevel)((int)instance.Changes[GameActorInfoDeltaView.Keys.AccessLevel]));
                if ((instance.DeltaMask & 2) != 0)
                    ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDeltaView.Keys.ArmorPointCapacity]);
                if ((instance.DeltaMask & 4) != 0)
                    ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDeltaView.Keys.ArmorPoints]);
                if ((instance.DeltaMask & 8) != 0)
                    EnumProxy<ChannelType>.Serialize(stream, (ChannelType)((int)instance.Changes[GameActorInfoDeltaView.Keys.Channel]));
                if ((instance.DeltaMask & 16) != 0)
                    StringProxy.Serialize(stream, (string)instance.Changes[GameActorInfoDeltaView.Keys.ClanTag]);
                if ((instance.DeltaMask & 32) != 0)
                    Int32Proxy.Serialize(stream, (int)instance.Changes[GameActorInfoDeltaView.Keys.Cmid]);
                if ((instance.DeltaMask & 64) != 0)
                    EnumProxy<FireMode>.Serialize(stream, (FireMode)((int)instance.Changes[GameActorInfoDeltaView.Keys.CurrentFiringMode]));
                if ((instance.DeltaMask & 128) != 0)
                    ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDeltaView.Keys.CurrentWeaponSlot]);
                if ((instance.DeltaMask & 256) != 0)
                    Int16Proxy.Serialize(stream, (short)instance.Changes[GameActorInfoDeltaView.Keys.Deaths]);
                if ((instance.DeltaMask & 512) != 0)
                    ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDeltaView.Keys.FunctionalItems], Int32Proxy.Serialize);
                if ((instance.DeltaMask & 1024) != 0)
                    ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDeltaView.Keys.Gear], Int32Proxy.Serialize);
                if ((instance.DeltaMask & 2048) != 0)
                    Int16Proxy.Serialize(stream, (short)instance.Changes[GameActorInfoDeltaView.Keys.Health]);
                if ((instance.DeltaMask & 4096) != 0)
                    Int16Proxy.Serialize(stream, (short)instance.Changes[GameActorInfoDeltaView.Keys.Kills]);
                if ((instance.DeltaMask & 8192) != 0)
                    Int32Proxy.Serialize(stream, (int)instance.Changes[GameActorInfoDeltaView.Keys.Level]);
                if ((instance.DeltaMask & 16384) != 0)
                    UInt16Proxy.Serialize(stream, (ushort)instance.Changes[GameActorInfoDeltaView.Keys.Ping]);
                if ((instance.DeltaMask & 32768) != 0)
                    ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDeltaView.Keys.PlayerId]);
                if ((instance.DeltaMask & 65536) != 0)
                    StringProxy.Serialize(stream, (string)instance.Changes[GameActorInfoDeltaView.Keys.PlayerName]);
                if ((instance.DeltaMask & 131072) != 0)
                    EnumProxy<PlayerStates>.Serialize(stream, (PlayerStates)((byte)instance.Changes[GameActorInfoDeltaView.Keys.PlayerState]));
                if ((instance.DeltaMask & 262144) != 0)
                    ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDeltaView.Keys.QuickItems], Int32Proxy.Serialize);
                if ((instance.DeltaMask & 524288) != 0)
                    ByteProxy.Serialize(stream, (byte)instance.Changes[GameActorInfoDeltaView.Keys.Rank]);
                if ((instance.DeltaMask & 1048576) != 0)
                    ColorProxy.Serialize(stream, (Color)instance.Changes[GameActorInfoDeltaView.Keys.SkinColor]);
                if ((instance.DeltaMask & 2097152) != 0)
                    EnumProxy<SurfaceType>.Serialize(stream, (SurfaceType)((int)instance.Changes[GameActorInfoDeltaView.Keys.StepSound]));
                if ((instance.DeltaMask & 4194304) != 0)
                    EnumProxy<TeamID>.Serialize(stream, (TeamID)((int)instance.Changes[GameActorInfoDeltaView.Keys.TeamID]));
                if ((instance.DeltaMask & 8388608) != 0)
                    ListProxy<int>.Serialize(stream, (List<int>)instance.Changes[GameActorInfoDeltaView.Keys.Weapons], Int32Proxy.Serialize);
            }
            else
            {
                Int32Proxy.Serialize(stream, 0);
            }
        }

        public static GameActorInfoDeltaView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            byte id = ByteProxy.Deserialize(bytes);

            var view = new GameActorInfoDeltaView();
            view.PlayerId = id;
            if (mask != 0)
            {
                if ((mask & 1) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.AccessLevel] = EnumProxy<MemberAccessLevel>.Deserialize(bytes);
                if ((mask & 2) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.ArmorPointCapacity] = ByteProxy.Deserialize(bytes);
                if ((mask & 4) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.ArmorPoints] = ByteProxy.Deserialize(bytes);
                if ((mask & 8) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Channel] = EnumProxy<ChannelType>.Deserialize(bytes);
                if ((mask & 16) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.ClanTag] = StringProxy.Deserialize(bytes);
                if ((mask & 32) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Cmid] = Int32Proxy.Deserialize(bytes);
                if ((mask & 64) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.CurrentFiringMode] = EnumProxy<FireMode>.Deserialize(bytes);
                if ((mask & 128) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.CurrentWeaponSlot] = ByteProxy.Deserialize(bytes);
                if ((mask & 256) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Deaths] = Int16Proxy.Deserialize(bytes);
                if ((mask & 512) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.FunctionalItems] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
                if ((mask & 1024) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Gear] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
                if ((mask & 2048) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Health] = Int16Proxy.Deserialize(bytes);
                if ((mask & 4096) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Kills] = Int16Proxy.Deserialize(bytes);
                if ((mask & 8192) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Level] = Int32Proxy.Deserialize(bytes);
                if ((mask & 16384) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Ping] = UInt16Proxy.Deserialize(bytes);
                if ((mask & 32768) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.PlayerId] = ByteProxy.Deserialize(bytes);
                if ((mask & 65536) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.PlayerName] = StringProxy.Deserialize(bytes);
                if ((mask & 131072) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.PlayerState] = EnumProxy<PlayerStates>.Deserialize(bytes);
                if ((mask & 262144) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.QuickItems] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
                if ((mask & 524288) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Rank] = ByteProxy.Deserialize(bytes);
                if ((mask & 1048576) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.SkinColor] = ColorProxy.Deserialize(bytes);
                if ((mask & 2097152) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.StepSound] = EnumProxy<SurfaceType>.Deserialize(bytes);
                if ((mask & 4194304) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.TeamID] = EnumProxy<TeamID>.Deserialize(bytes);
                if ((mask & 8388608) != 0)
                    view.Changes[GameActorInfoDeltaView.Keys.Weapons] = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
            }
            return view;
        }
    }
}
