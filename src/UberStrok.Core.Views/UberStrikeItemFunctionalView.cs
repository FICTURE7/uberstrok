using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    //NOTE: There is another class named UberStrikeItemFunctionalView but under a different namespace.
    //      This one is from UberStrike.Core.Models.Views
    [Serializable]
	public class UberStrikeItemFunctionalView : BaseUberStrikeItemView
	{
        public override UberStrikeItemType ItemType => UberStrikeItemType.Functional;
	}
}
