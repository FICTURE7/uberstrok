using System;

namespace UberStrok.Core.Views
{
    [Serializable]
	public class UberstrikeUserView
	{
		public MemberView CmuneMemberView { get; set; }
		public UberstrikeMemberView UberstrikeMemberView { get; set; }
	}
}
