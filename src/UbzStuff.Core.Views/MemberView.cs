using System;
using System.Collections.Generic;
using System.Text;

namespace UbzStuff.Core.Views
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

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("[Member view: ");
			if (this.PublicProfile != null && this.MemberWallet != null)
			{
				stringBuilder.Append(this.PublicProfile);
				stringBuilder.Append(this.MemberWallet);
				stringBuilder.Append("[items: ");
				if (this.MemberItems != null && this.MemberItems.Count > 0)
				{
					int num = this.MemberItems.Count;
					foreach (int current in this.MemberItems)
					{
						stringBuilder.Append(current);
						if (--num > 0)
						{
							stringBuilder.Append(", ");
						}
					}
				}
				else
				{
					stringBuilder.Append("No items");
				}
				stringBuilder.Append("]");
			}
			else
			{
				stringBuilder.Append("No member");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public List<int> MemberItems { get; set; }
		public MemberWalletView MemberWallet { get; set; }
		public PublicProfileView PublicProfile { get; set; }
	}
}
