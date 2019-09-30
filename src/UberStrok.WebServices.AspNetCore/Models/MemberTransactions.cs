using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class MemberTransactions
    {
        public IList<ItemTransaction> Items { get; set; }
        public IList<PointsDeposit> Points { get; set; }
    }
}
