using System;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class InventoryItem
    {
        public int ItemId { get; set; }
        public int AmountRemaining { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
