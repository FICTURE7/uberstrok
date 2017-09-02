using System;
using System.Collections.Generic;
using UbzStuff.Core.Common;

namespace UbzStuff.Core.Views
{
	[Serializable]
	public class MapView
	{
		public string Description { get; set; }
		public string DisplayName { get; set; }
		public bool IsBlueBox { get; set; }
		public int MapId { get; set; }
		public int MaxPlayers { get; set; }
		public int RecommendedItemId { get; set; }
		public string SceneName { get; set; }
		public Dictionary<GameModeType, MapSettingsView> Settings { get; set; }
		public int SupportedGameModes { get; set; }
		public int SupportedItemClass { get; set; }
	}
}
