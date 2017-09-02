using System;
using UbzStuff.Core.Common;

namespace UbzStuff.Core.Views
{
    [Serializable]
	public class MemberAuthenticationResultView
	{
		public string AuthToken { get; set; }
		public bool IsAccountComplete { get; set; }
		public LuckyDrawUnityView LuckyDraw { get; set; }
		public MemberAuthenticationResult MemberAuthenticationResult { get; set; }
		public MemberView MemberView { get; set; }
		public PlayerStatisticsView PlayerStatisticsView { get; set; }
		public DateTime ServerTime { get; set; }
	}
}
