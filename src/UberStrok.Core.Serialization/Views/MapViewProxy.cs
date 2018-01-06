using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class MapViewProxy
	{
		public static MapView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new MapView();

			if ((mask & 1) != 0)
				view.Description = StringProxy.Deserialize(bytes);
			if ((mask & 2) != 0)
				view.DisplayName = StringProxy.Deserialize(bytes);

			view.IsBlueBox = BooleanProxy.Deserialize(bytes);
			view.MapId = Int32Proxy.Deserialize(bytes);
			view.MaxPlayers = Int32Proxy.Deserialize(bytes);
			view.RecommendedItemId = Int32Proxy.Deserialize(bytes);

            if ((mask & 4) != 0)
				view.SceneName = StringProxy.Deserialize(bytes);
			if ((mask & 8) != 0)
				view.Settings = DictionaryProxy<GameModeType, MapSettingsView>.Deserialize(bytes, EnumProxy<GameModeType>.Deserialize, MapSettingsViewProxy.Deserialize);

			view.SupportedGameModes = Int32Proxy.Deserialize(bytes);
			view.SupportedItemClass = Int32Proxy.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, MapView instance)
		{
			var mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.Description != null)
					StringProxy.Serialize(bytes, instance.Description);
				else
					mask |= 1;
				if (instance.DisplayName != null)
					StringProxy.Serialize(bytes, instance.DisplayName);
				else
					mask |= 2;

				BooleanProxy.Serialize(bytes, instance.IsBlueBox);
				Int32Proxy.Serialize(bytes, instance.MapId);
				Int32Proxy.Serialize(bytes, instance.MaxPlayers);
				Int32Proxy.Serialize(bytes, instance.RecommendedItemId);

                if (instance.SceneName != null)
					StringProxy.Serialize(bytes, instance.SceneName);
				else
					mask |= 4;
                if (instance.Settings != null)
					DictionaryProxy<GameModeType, MapSettingsView>.Serialize(bytes, instance.Settings, EnumProxy<GameModeType>.Serialize, MapSettingsViewProxy.Serialize);
				else
					mask |= 8;

				Int32Proxy.Serialize(bytes, instance.SupportedGameModes);
				Int32Proxy.Serialize(bytes, instance.SupportedItemClass);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
