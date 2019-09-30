using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class MemberAuthenticationResultView
    {
        public MemberAuthenticationResultView()
        {
            /* Space. */
        }

        public MemberAuthenticationResultView(MemberAuthenticationResult result)
        {
            MemberAuthenticationResult = result;
        }

		public string AuthToken { get; set; }
		public bool IsAccountComplete { get; set; }
		public LuckyDrawUnityView LuckyDraw { get; set; }
		public MemberAuthenticationResult MemberAuthenticationResult { get; set; }
		public MemberView MemberView { get; set; }
		public PlayerStatisticsView PlayerStatisticsView { get; set; }
		public DateTime ServerTime { get; set; }
	}
}
