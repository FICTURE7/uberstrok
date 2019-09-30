using System;
using UberStrok.Core.Views;
using UberStrok.Core.Common;

namespace UberStrok.Core
{
    public abstract class Item
    {
        public int Id => View.ID;
        public string Name => View.Name;
        public string PrefabName => View.PrefabName;
        public UberStrikeItemType Type => View.ItemType;
        public UberStrikeItemClass Class => View.ItemClass;

        protected BaseUberStrikeItemView View { get; set; }

        public Item(BaseUberStrikeItemView view)
            => View = view ?? throw new ArgumentNullException(nameof(view));

        public BaseUberStrikeItemView GetView() 
            => View;

        public override int GetHashCode()
            => Id;

        public override string ToString()
            => $"({Id}:\"{PrefabName}\" -> \"{Name}\")";
    }

    // TODO: Turn this into a sealed class.
    public class Item<TView> : Item where TView : BaseUberStrikeItemView
    {
        public Item(TView view) : base(view)
        {
            // Space
        }

        public new TView GetView() => (TView)View;
    }
}
