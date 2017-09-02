using System;
using UbzStuff.Core.Common;

namespace UbzStuff.Core.Views
{
    [Serializable]
	public class UberStrikeItemGearView : BaseUberStrikeItemView
	{
		public int ArmorPoints { get; set; }
		public int ArmorWeight { get; set; }

        public override UberstrikeItemType ItemType => UberstrikeItemType.Gear;
	}
}
