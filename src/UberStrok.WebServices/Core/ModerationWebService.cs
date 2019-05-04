using UberStrok.Core.Common;

namespace UberStrok.WebServices.Core
{
    public class ModerationWebService : BaseModerationWebService
    {
        public ModerationWebService(WebServiceContext ctx) : base(ctx)
        {

        }

        public override int OnBanCmid(string authToken, int cmid)
        {
            var member = Context.Users.GetMember(authToken);
            if (member.PublicProfile.AccessLevel < MemberAccessLevel.SeniorQA)
                return 1;

            Context.Users.Db.BanCmid(cmid);
            return 0;
        }

        public override int OnBanHwd(string authToken, string hwd)
        {
            return 1;
        }

        public override int OnBanIp(string authToken, string ip)
        {
            return 1;
        }
    }
}
