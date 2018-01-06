using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class PublicProfileView
	{
		public PublicProfileView()
		{
            Cmid = 0;
            Name = string.Empty;
            IsChatDisabled = false;
            AccessLevel = MemberAccessLevel.Default;
            GroupTag = string.Empty;
            LastLoginDate = DateTime.UtcNow;
            EmailAddressStatus = EmailAddressStatus.Unverified;
            FacebookId = string.Empty;
		}

		public PublicProfileView(int cmid, string name, MemberAccessLevel accesLevel, bool isChatDisabled, DateTime lastLoginDate, EmailAddressStatus emailAddressStatus, string facebookId)
		{
            SetPublicProfile(cmid, name, accesLevel, isChatDisabled, string.Empty, lastLoginDate, emailAddressStatus, facebookId);
		}

		public PublicProfileView(int cmid, string name, MemberAccessLevel accesLevel, bool isChatDisabled, string groupTag, DateTime lastLoginDate, EmailAddressStatus emailAddressStatus, string facebookId)
		{
            SetPublicProfile(cmid, name, accesLevel, isChatDisabled, groupTag, lastLoginDate, emailAddressStatus, facebookId);
		}

		private void SetPublicProfile(int cmid, string name, MemberAccessLevel accesLevel, bool isChatDisabled, string groupTag, DateTime lastLoginDate, EmailAddressStatus emailAddressStatus, string facebookId)
		{
            Cmid = cmid;
            Name = name;
            AccessLevel = accesLevel;
            IsChatDisabled = isChatDisabled;
            GroupTag = groupTag;
            LastLoginDate = lastLoginDate;
            EmailAddressStatus = emailAddressStatus;
            FacebookId = facebookId;
		}

		public override string ToString()
		{
			string text = "[Public profile: ";
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"[Member name:",
				this.Name,
				"][CMID:",
				this.Cmid,
				"][Is banned from chat: ",
				this.IsChatDisabled,
				"]"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"[Access level:",
				this.AccessLevel,
				"][Group tag: ",
				this.GroupTag,
				"][Last login date: ",
				this.LastLoginDate,
				"]]"
			});
			text2 = text;
			return string.Concat(new object[]
			{
				text2,
				"[EmailAddressStatus:",
				this.EmailAddressStatus,
				"][FacebookId: ",
				this.FacebookId,
				"]"
			});
		}

		public MemberAccessLevel AccessLevel { get; set; }
		public int Cmid { get; set; }
		public EmailAddressStatus EmailAddressStatus { get; set; }
		public string FacebookId { get; set; }
		public string GroupTag { get; set; }
		public bool IsChatDisabled { get; set; }
		public DateTime LastLoginDate { get; set; }
		public string Name { get; set; }
	}
}
