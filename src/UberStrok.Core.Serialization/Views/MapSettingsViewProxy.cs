using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class MapSettingsViewProxy
	{
		public static MapSettingsView Deserialize(Stream bytes)
		{
			return new MapSettingsView
            {
				KillsCurrent = Int32Proxy.Deserialize(bytes),
				KillsMax = Int32Proxy.Deserialize(bytes),
				KillsMin = Int32Proxy.Deserialize(bytes),
				PlayersCurrent = Int32Proxy.Deserialize(bytes),
				PlayersMax = Int32Proxy.Deserialize(bytes),
				PlayersMin = Int32Proxy.Deserialize(bytes),
				TimeCurrent = Int32Proxy.Deserialize(bytes),
				TimeMax = Int32Proxy.Deserialize(bytes),
				TimeMin = Int32Proxy.Deserialize(bytes)
			};
		}

		public static void Serialize(Stream stream, MapSettingsView instance)
		{
			using (MemoryStream bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.KillsCurrent);
				Int32Proxy.Serialize(bytes, instance.KillsMax);
				Int32Proxy.Serialize(bytes, instance.KillsMin);
				Int32Proxy.Serialize(bytes, instance.PlayersCurrent);
				Int32Proxy.Serialize(bytes, instance.PlayersMax);
				Int32Proxy.Serialize(bytes, instance.PlayersMin);
				Int32Proxy.Serialize(bytes, instance.TimeCurrent);
				Int32Proxy.Serialize(bytes, instance.TimeMax);
				Int32Proxy.Serialize(bytes, instance.TimeMin);
				bytes.WriteTo(stream);
			}
		}
	}
}
