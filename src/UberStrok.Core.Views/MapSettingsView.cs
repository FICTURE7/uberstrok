using System;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class MapSettingsView
	{
		public int KillsCurrent { get; set; }
		public int KillsMax { get; set; }
		public int KillsMin { get; set; }
		public int PlayersCurrent { get; set; }
		public int PlayersMax { get; set; }
		public int PlayersMin { get; set; }
		public int TimeCurrent { get; set; }
		public int TimeMax { get; set; }
		public int TimeMin { get; set; }
	}
}
