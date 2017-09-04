using log4net;
using System.ServiceModel;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;
using System.Collections.Generic;

namespace UbzStuff.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class UserWebService : BaseUserWebService
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(UserWebService).Name);

        public UserWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public override List<ItemInventoryView> OnGetInventory(string authToken)
        {
            var member = Context.Users.GetMember(authToken);
            if (member == null)
            {
                Log.Error("An unidentified AuthToken was passed.");
                return null;
            }

            var view = Context.Users.Db.Inventories.Load(member.PublicProfile.Cmid);
            return view;
        }

        public override LoadoutView OnGetLoadout(string authToken)
        {
            var member = Context.Users.GetMember(authToken);
            if (member == null)
            {
                Log.Error("An unidentified AuthToken was passed.");
                return null;
            }

            var view = Context.Users.Db.Loadouts.Load(member.PublicProfile.Cmid);
            return view;
        }

        public override UberstrikeUserView OnGetMember(string authToken)
        {
            // Get loaded member in memory using the auth token.
            var member = Context.Users.GetMember(authToken);
            if (member == null)
            {
                Log.Error("An unidentified AuthToken was passed.");
                return null;
            }

            var view = new UberstrikeUserView
            {
                CmuneMemberView = member,
                UberstrikeMemberView = new UberstrikeMemberView
                {
                    PlayerStatisticsView = new PlayerStatisticsView
                    {
                        Cmid = member.PublicProfile.Cmid,
                        PersonalRecord = new PlayerPersonalRecordStatisticsView(),
                        WeaponStatistics = new PlayerWeaponStatisticsView()
                    }
                }
            };
            return view;
        }

        public override bool OnIsDuplicateMemberName(string username)
        {
            return false;
        }

        public override MemberOperationResult OnSetLoaduout(string authToken, LoadoutView loadoutView)
        {
            var member = Context.Users.GetMember(authToken);
            if (member == null)
            {
                Log.Error("An unidentified AuthToken was passed.");
                return MemberOperationResult.InvalidData;
            }

            // Save straight up because we don't really care if the client is hacking.
            // Items at least.
            Context.Users.Db.Loadouts.Save(loadoutView);
            return MemberOperationResult.Ok;
        }
    }
}
