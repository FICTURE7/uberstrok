using System;
using UberStrok.Core.Common;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class PointsDeposit
    {
        public DateTime Date { get; set; }
        public int Points { get; set; }
        public PointsDepositType Type { get; set; }
    }
}
