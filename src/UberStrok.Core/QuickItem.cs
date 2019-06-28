using System;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class QuickItem
    {
        private readonly UberStrikeItemQuickView _view;

        public QuickItem(UberStrikeItemQuickView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public UberStrikeItemQuickView GetView()
        {
            return _view;
        }

        public void Reset()
        {
            /* TODO: Implement */
        }
    }
}
