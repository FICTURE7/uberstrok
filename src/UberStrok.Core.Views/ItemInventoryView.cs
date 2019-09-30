using System;
using System.Text;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class ItemInventoryView
	{
		public ItemInventoryView()
		{
            // Space
		}

		public ItemInventoryView(int itemId, DateTime? expirationDate, int amountRemaining)
            : this(itemId, expirationDate, amountRemaining, default)
		{
            // Space
		}

		public ItemInventoryView(int itemId, DateTime? expirationDate, int amountRemaining, int cmid) 
		{
            ItemId = itemId;
            ExpirationDate = expirationDate;
            AmountRemaining = amountRemaining;
            Cmid = cmid;
		}

		public override string ToString()
		{
            var builder = new StringBuilder().Append("[LiveInventoryView: ")
                .Append("[Item Id: ").Append(ItemId)
                .Append("][Expiration date: ").Append(ExpirationDate)
                .Append("][Amount remaining:").Append(AmountRemaining)
            .Append("]]");
            return builder.ToString();
		}

		public int AmountRemaining { get; set; }
		public int Cmid { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public int ItemId { get; set; }
	}
}
