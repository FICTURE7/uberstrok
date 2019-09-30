using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
	public class PublicProfileView
	{
		public PublicProfileView()
		{
            // Space
		}

        [Obsolete]
		public PublicProfileView(
            int cmid,
            string name,
            MemberAccessLevel accesLevel, 
            bool isChatDisabled,
            DateTime lastLoginDate,
            EmailAddressStatus emailAddressStatus,
            string facebookId)
            => SetPublicProfile(cmid, name, accesLevel, isChatDisabled, string.Empty, lastLoginDate, emailAddressStatus, facebookId);

		private void SetPublicProfile(
            int cmid,
            string name,
            MemberAccessLevel accessLevel,
            bool isChatDisabled,
            string groupTag,
            DateTime lastLoginDate,
            EmailAddressStatus emailAddressStatus,
            string facebookId)
		{
            Cmid = cmid;
            Name = name;
            AccessLevel = accessLevel;
            IsChatDisabled = isChatDisabled;
            GroupTag = groupTag;
            LastLoginDate = lastLoginDate;
            EmailAddressStatus = emailAddressStatus;
            FacebookId = facebookId;
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
