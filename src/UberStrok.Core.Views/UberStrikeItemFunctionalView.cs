using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
	public class UberStrikeItemFunctionalView : BaseUberStrikeItemView
	{
        public override UberStrikeItemType ItemType => UberStrikeItemType.Functional;
	}
}
