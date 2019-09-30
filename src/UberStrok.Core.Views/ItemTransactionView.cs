using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    public class ItemTransactionView
    {
        public int WithdrawalId { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public int Points { get; set; }
        public int Credits { get; set; }
        public int Cmid { get; set; }
        public bool IsAdminAction { get; set; }
        public int ItemId { get; set; }
        public BuyingDurationType Duration { get; set; }
    }
}
