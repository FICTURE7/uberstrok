using System;
using System.Collections.Generic;
using System.Text;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class MemberView
	{
		public MemberView()
		{
			PublicProfile = new PublicProfileView();
            MemberWallet = new MemberWalletView();
            MemberItems = new List<int>();
		}

		public MemberView(PublicProfileView publicProfile, MemberWalletView memberWallet, List<int> memberItems)
		{
            PublicProfile = publicProfile;
            MemberWallet = memberWallet;
            MemberItems = memberItems;
		}

        public List<int> MemberItems { get; set; }
		public MemberWalletView MemberWallet { get; set; }
		public PublicProfileView PublicProfile { get; set; }
	}
}
