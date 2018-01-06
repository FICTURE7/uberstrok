using System;

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
		{
            ItemId = itemId;
            ExpirationDate = expirationDate;
            AmountRemaining = amountRemaining;
		}

		public ItemInventoryView(int itemId, DateTime? expirationDate, int amountRemaining, int cmid) : this(itemId, expirationDate, amountRemaining)
		{
            Cmid = cmid;
		}

		public override string ToString()
		{
			string text = "[LiveInventoryView: ";
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"[Item Id: ",
				this.ItemId,
				"]"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"[Expiration date: ",
				this.ExpirationDate,
				"]"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"[Amount remaining:",
				this.AmountRemaining,
				"]"
			});
			return text + "]";
		}

		public int AmountRemaining { get; set; }
		public int Cmid { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public int ItemId { get; set; }
	}
}
