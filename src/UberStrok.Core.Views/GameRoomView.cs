using System;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class GameRoomView
	{
		public int MapId { get; set; }
		public int Number { get; set; }
        public ConnectionAddressView Server { get; set; }
    }
}
