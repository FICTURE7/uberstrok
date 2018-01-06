using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class CommActorInfoViewProxy
	{
		public static CommActorInfoView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new CommActorInfoView();
			view.AccessLevel = EnumProxy<MemberAccessLevel>.Deserialize(bytes);
			view.Channel = EnumProxy<ChannelType>.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.ClanTag = StringProxy.Deserialize(bytes);

			view.Cmid = Int32Proxy.Deserialize(bytes);

			if ((mask & 2) != 0)
				view.CurrentRoom = GameRoomViewProxy.Deserialize(bytes);

			view.ModerationFlag = ByteProxy.Deserialize(bytes);

			if ((mask & 4) != 0)
				view.ModInformation = StringProxy.Deserialize(bytes);
			if ((mask & 8) != 0)
				view.PlayerName = StringProxy.Deserialize(bytes);

			return view;
		}

		public static void Serialize(Stream stream, CommActorInfoView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				EnumProxy<MemberAccessLevel>.Serialize(bytes, instance.AccessLevel);
				EnumProxy<ChannelType>.Serialize(bytes, instance.Channel);

				if (instance.ClanTag != null)
					StringProxy.Serialize(bytes, instance.ClanTag);
				else
					mask |= 1;
                
				Int32Proxy.Serialize(bytes, instance.Cmid);

				if (instance.CurrentRoom != null)
					GameRoomViewProxy.Serialize(bytes, instance.CurrentRoom);
				else
					mask |= 2;

				ByteProxy.Serialize(bytes, instance.ModerationFlag);

				if (instance.ModInformation != null)
					StringProxy.Serialize(bytes, instance.ModInformation);
				else
					mask |= 4;
				if (instance.PlayerName != null)
					StringProxy.Serialize(bytes, instance.PlayerName);
				else
					mask |= 8;

				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
