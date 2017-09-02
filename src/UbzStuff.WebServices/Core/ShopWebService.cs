using log4net;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;

namespace UbzStuff.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ShopWebService : BaseShopWebService
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(ShopWebService));

        public ShopWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public override BuyItemResult OnBuyItem(int itemId, string authToken, UberStrikeCurrencyType currencyType, BuyingDurationType durationType, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType)
        {
            var member = Context.Users.GetMember(authToken);
            if (member == null)
            {
                Log.Error("An unidentified AuthToken was passed.");
                return BuyItemResult.InvalidData;
            }

            //TODO: Calculate expiration date?

            var cmid = member.PublicProfile.Cmid;
            var inventory = Context.Users.Db.Inventories.Load(member.PublicProfile.Cmid);
            inventory.Add(new ItemInventoryView(itemId, null, -1, cmid));

            Context.Users.Db.Inventories.Save(cmid, inventory);
            return BuyItemResult.OK;
        }

        public override UberStrikeItemShopClientView OnGetShop()
        {
            var viewJson = File.ReadAllText("configs/items.json");
            var view = JsonConvert.DeserializeObject<UberStrikeItemShopClientView>(viewJson);
            return view;
        }
    }
}
