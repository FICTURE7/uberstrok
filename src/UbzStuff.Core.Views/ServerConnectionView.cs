using System;
using UbzStuff.Core.Common;

namespace UbzStuff.Core.Views
{
    [Serializable]
	public class ServerConnectionView
	{
		public MemberAccessLevel AccessLevel { get; set; }
		public string ApiVersion { get; set; }
        public ChannelType Channel { get; set; }
        public int Cmid { get; set; }
    }
}
