using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
	public class UberStrikeItemGearView : BaseUberStrikeItemView
	{
		public int ArmorPoints { get; set; }
		public int ArmorWeight { get; set; }

        public override UberStrikeItemType ItemType => UberStrikeItemType.Gear;
	}
}
