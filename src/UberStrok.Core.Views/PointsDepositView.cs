using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    public class PointsDepositView
    {
        public int PointDepositId { get; set; }
        public DateTime DepositDate { get; set; }
        public int Points { get; set; }
        public int Cmid { get; set; }
        public bool IsAdminAction { get; set; }
        public PointsDepositType DepositType { get; set; }
    }
}
