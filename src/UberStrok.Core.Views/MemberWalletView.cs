using System;
using System.Text;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class MemberWalletView
	{
		public MemberWalletView()
		{
            CreditsExpiration = DateTime.Today;
            PointsExpiration = DateTime.Today;
		}

		public MemberWalletView(int cmid, int? credits, int? points, DateTime? creditsExpiration, DateTime? pointsExpiration)
		{
			if (!credits.HasValue)
				credits = new int?(0);
			if (!points.HasValue)
				points = new int?(0);
			if (!creditsExpiration.HasValue)
				creditsExpiration = new DateTime?(DateTime.MinValue);
			if (!pointsExpiration.HasValue)
				pointsExpiration = new DateTime?(DateTime.MinValue);

            SetMemberWallet(cmid, credits.Value, points.Value, creditsExpiration.Value, pointsExpiration.Value);
		}

		public MemberWalletView(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
		{
            SetMemberWallet(cmid, credits, points, creditsExpiration, pointsExpiration);
		}

		private void SetMemberWallet(int cmid, int credits, int points, DateTime creditsExpiration, DateTime pointsExpiration)
		{
            Cmid = cmid;
            Credits = credits;
            Points = points;
            CreditsExpiration = creditsExpiration;
            PointsExpiration = pointsExpiration;
		}

		public override string ToString()
		{
            var builder = new StringBuilder().Append("[Wallet:")
                .Append("[CMID:").Append(Cmid)
                .Append("][Credits:").Append(Credits)
                .Append("][Credits Expiration:").Append(CreditsExpiration)
                .Append("][Points:").Append(Points)
                .Append("][Points Expiration:").Append(PointsExpiration)
                .Append("]]");
            return builder.ToString();
		}

		public int Cmid { get; set; }
		public int Credits { get; set; }
		public DateTime CreditsExpiration { get; set; }
		public int Points { get; set; }
		public DateTime PointsExpiration { get; set; }
	}
}
