using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PlayerStatisticsViewProxy
	{
		public static PlayerStatisticsView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new PlayerStatisticsView();
			view.Cmid = Int32Proxy.Deserialize(bytes);
			view.Headshots = Int32Proxy.Deserialize(bytes);
			view.Hits = Int64Proxy.Deserialize(bytes);
			view.Level = Int32Proxy.Deserialize(bytes);
			view.Nutshots = Int32Proxy.Deserialize(bytes);

            if ((mask & 1) != 0)
				view.PersonalRecord = PlayerPersonalRecordStatisticsViewProxy.Deserialize(bytes);

            view.Shots = Int64Proxy.Deserialize(bytes);
			view.Splats = Int32Proxy.Deserialize(bytes);
			view.Splatted = Int32Proxy.Deserialize(bytes);
			view.TimeSpentInGame = Int32Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
				view.WeaponStatistics = PlayerWeaponStatisticsViewProxy.Deserialize(bytes);

            view.Xp = Int32Proxy.Deserialize(bytes);

            return view;
		}

		public static void Serialize(Stream stream, PlayerStatisticsView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.Cmid);
				Int32Proxy.Serialize(bytes, instance.Headshots);
				Int64Proxy.Serialize(bytes, instance.Hits);
				Int32Proxy.Serialize(bytes, instance.Level);
				Int32Proxy.Serialize(bytes, instance.Nutshots);

                if (instance.PersonalRecord != null)
					PlayerPersonalRecordStatisticsViewProxy.Serialize(bytes, instance.PersonalRecord);
				else
					mask |= 1;

                Int64Proxy.Serialize(bytes, instance.Shots);
				Int32Proxy.Serialize(bytes, instance.Splats);
				Int32Proxy.Serialize(bytes, instance.Splatted);
				Int32Proxy.Serialize(bytes, instance.TimeSpentInGame);

                if (instance.WeaponStatistics != null)
					PlayerWeaponStatisticsViewProxy.Serialize(bytes, instance.WeaponStatistics);
				else
					mask |= 2;

                Int32Proxy.Serialize(bytes, instance.Xp);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
