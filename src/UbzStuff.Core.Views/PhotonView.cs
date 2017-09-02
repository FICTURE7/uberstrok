using System;
using UbzStuff.Core.Common;

namespace UbzStuff.Core.Views
{
    [Serializable]
	public class PhotonView
	{
		public string IP { get; set; }
		public int MinLatency { get; set; }
		public string Name { get; set; }
		public int PhotonId { get; set; }
		public int Port { get; set; }
		public RegionType Region { get; set; }
		public PhotonUsageType UsageType { get; set; }
	}
}
