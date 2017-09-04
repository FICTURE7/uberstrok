using log4net;
using System;
using System.IO;
using UbzStuff.Core.Common;
using UbzStuff.Core.Serialization;
using UbzStuff.Core.Serialization.Views;
using UbzStuff.Core.Views;
using UbzStuff.WebServices.Contracts;

namespace UbzStuff.WebServices.Core
{
    public abstract class BaseShopWebService : BaseWebService, IShopWebServiceContract
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(BaseShopWebService).Name);

        protected BaseShopWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public abstract BuyItemResult OnBuyItem(int itemId, string authToken, UberStrikeCurrencyType currencyType, BuyingDurationType durationType, UberstrikeItemType itemType, BuyingLocationType marketLocation, BuyingRecommendationType recommendationType);
        public abstract UberStrikeItemShopClientView OnGetShop();

        byte[] IShopWebServiceContract.BuyBundle(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyBundle request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.BuyBundleSteam(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyBundleSteam request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.BuyItem(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var itemId = Int32Proxy.Deserialize(bytes);
                    var authToken = StringProxy.Deserialize(bytes);
                    var currencyType = EnumProxy<UberStrikeCurrencyType>.Deserialize(bytes);
                    var durationType = EnumProxy<BuyingDurationType>.Deserialize(bytes);
                    var itemType = EnumProxy<UberstrikeItemType>.Deserialize(bytes);
                    var marketLocation = EnumProxy<BuyingLocationType>.Deserialize(bytes);
                    var recommendationType = EnumProxy<BuyingRecommendationType>.Deserialize(bytes);

                    BuyItemResult result = OnBuyItem(itemId, authToken, currencyType, durationType, itemType, marketLocation, recommendationType);
                    using (var outBytes = new MemoryStream())
                    {
                        EnumProxy<BuyItemResult>.Serialize(outBytes, result);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyItem request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.BuyPack(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle BuyPack request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.FinishBuyBundleSteam(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle FinishBuyBundleSteam request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetAllLuckyDraws_1(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllLuckyDraws_1 request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetAllLuckyDraws_2(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllLuckyDraws_2 request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetAllMysteryBoxs_1(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllMysteryBoxs_1 request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetAllMysteryBoxs_2(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetAllMysteryBoxs_2 request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetBundles(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetBundles request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetLuckyDraw(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetLuckyDraw request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetMysteryBox(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMysteryBox request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.GetShop(byte[] data)
        {
            try
            {
                var view = OnGetShop();
                using (var outBytes = new MemoryStream())
                {
                    UberStrikeItemShopClientViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetShop request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.RollLuckyDraw(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle RollLuckyDraw request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.RollMysteryBox(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle RollMysteryBox request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.UseConsumableItem(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle UseConsumableItem request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IShopWebServiceContract.VerifyReceipt(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle VerifyReceipt request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
