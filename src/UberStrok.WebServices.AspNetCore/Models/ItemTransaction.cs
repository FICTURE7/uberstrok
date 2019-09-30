using System;
using UberStrok.Core.Common;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class ItemTransaction
    {
        public int ItemId { get; set; }
        public DateTime Date { get; set; }
        public int Points { get; set; }
        public int Credits { get; set; }
        public BuyingDurationType Duration { get; set; }
    }
}
