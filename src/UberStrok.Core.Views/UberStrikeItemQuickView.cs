using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class UberStrikeItemQuickView : BaseUberStrikeItemView
    {
        public QuickItemLogic BehaviourType { get; set; }
        public int CoolDownTime { get; set; }
        public int MaxOwnableAmount { get; set; }
        public int UsesPerGame { get; set; }
        public int UsesPerLife { get; set; }
        public int UsesPerRound { get; set; }
        public int WarmUpTime { get; set; }

        public override UberstrikeItemType ItemType => UberstrikeItemType.QuickUse;
    }
}
