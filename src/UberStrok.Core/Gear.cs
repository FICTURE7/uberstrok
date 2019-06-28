using System;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class Gear
    {
        private readonly UberStrikeItemGearView _view;

        public Gear(UberStrikeItemGearView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public UberStrikeItemGearView GetView()
        {
            return _view;
        }
    }
}
