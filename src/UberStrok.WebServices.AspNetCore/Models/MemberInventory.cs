using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class MemberInventory
    {
        public IDictionary<int, InventoryItem> Items { get; set; }
    }
}
