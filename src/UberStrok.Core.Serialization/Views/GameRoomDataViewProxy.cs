using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class GameRoomDataViewProxy
    {
        public static void Serialize(Stream stream, GameRoomDataView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, instance.ConnectedPlayers);
                Int32Proxy.Serialize(bytes, instance.GameFlags);
                EnumProxy<GameModeType>.Serialize(bytes, instance.GameMode);

                if (instance.Guid != null)
                    StringProxy.Serialize(bytes, instance.Guid);
                else
                    mask |= 1;

                BooleanProxy.Serialize(bytes, instance.IsPasswordProtected);
                BooleanProxy.Serialize(bytes, instance.IsPermanentGame);
                Int32Proxy.Serialize(bytes, instance.KillLimit);
                ByteProxy.Serialize(bytes, instance.LevelMax);
                ByteProxy.Serialize(bytes, instance.LevelMin);
                Int32Proxy.Serialize(bytes, instance.MapID);

                if (instance.Name != null)
                    StringProxy.Serialize(bytes, instance.Name);
                else
                    mask |= 2;

                Int32Proxy.Serialize(bytes, instance.RoomId);
                Int32Proxy.Serialize(bytes, instance.PlayerLimit);

                if (instance.Server != null)
                    ConnectionAddressViewProxy.Serialize(bytes, instance.Server);
                else
                    mask |= 4;

                Int32Proxy.Serialize(bytes, instance.TimeLimit);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static GameRoomDataView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var view = new GameRoomDataView();
            view.ConnectedPlayers = Int32Proxy.Deserialize(bytes);
            view.GameFlags = Int32Proxy.Deserialize(bytes);
            view.GameMode = EnumProxy<GameModeType>.Deserialize(bytes);

            if ((mask & 1) != 0)
                view.Guid = StringProxy.Deserialize(bytes);

            view.IsPasswordProtected = BooleanProxy.Deserialize(bytes);
            view.IsPermanentGame = BooleanProxy.Deserialize(bytes);
            view.KillLimit = Int32Proxy.Deserialize(bytes);
            view.LevelMax = ByteProxy.Deserialize(bytes);
            view.LevelMin = ByteProxy.Deserialize(bytes);
            view.MapID = Int32Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
                view.Name = StringProxy.Deserialize(bytes);

            view.RoomId = Int32Proxy.Deserialize(bytes);
            view.PlayerLimit = Int32Proxy.Deserialize(bytes);

            if ((mask & 4) != 0)
                view.Server = ConnectionAddressViewProxy.Deserialize(bytes);

            view.TimeLimit = Int32Proxy.Deserialize(bytes);
            return view;
        }
    }
}
