using log4net;
using System.ServiceModel;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using System.Collections.Generic;

namespace UberStrok.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ShopWebService : BaseShopWebService
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(ShopWebService).Name);

        public ShopWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public override BuyItemResult OnBuyItem(int itemId, string authToken, UberStrikeCurrencyType currencyType, BuyingDurationType durationType, UberStrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType)
        {
            var member = Context.Users.GetMember(authToken);
            if (member == null)
            {
                Log.Error("An unidentified AuthToken was passed.");
                return BuyItemResult.InvalidData;
            }

            var cmid = member.PublicProfile.Cmid;
            var inventory = Context.Users.Db.Inventories.Load(member.PublicProfile.Cmid);

            inventory.Add(new ItemInventoryView(itemId, null, -1, cmid));

            Context.Users.Db.Inventories.Save(cmid, inventory);
            return BuyItemResult.OK;
        }

        public override UberStrikeItemShopClientView OnGetShop()
        {
            return Context.Items.GetShop();
        }

        public override List<BundleView> OnGetBundles(ChannelType channel)
        {
            return Context.Items.GetBundles();
        }
    }
}
