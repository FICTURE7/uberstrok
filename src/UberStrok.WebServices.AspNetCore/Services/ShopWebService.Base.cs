using System;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BaseShopWebService : IShopAsyncWebServiceContract
    {
        public abstract Task<UberStrikeItemShopClientView> GetShop();
        public abstract Task<BuyItemResult> BuyItem(int itemId, string authToken, 
            UberStrikeCurrencyType currencyType, 
            BuyingDurationType durationType, 
            UberStrikeItemType itemType,
            BuyingLocationType marketLocation,
            BuyingRecommendationType recommendationType);

        async Task<byte[]> IShopAsyncWebServiceContract.BuyItem(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var itemId = Int32Proxy.Deserialize(bytes);
                var authToken = StringProxy.Deserialize(bytes);
                var currencyType = EnumProxy<UberStrikeCurrencyType>.Deserialize(bytes);
                var durationType = EnumProxy<BuyingDurationType>.Deserialize(bytes);
                var itemType = EnumProxy<UberStrikeItemType>.Deserialize(bytes);
                var marketLocation = EnumProxy<BuyingLocationType>.Deserialize(bytes);
                var recommendationType = EnumProxy<BuyingRecommendationType>.Deserialize(bytes);

                var result = await BuyItem(itemId, authToken, currencyType, durationType, itemType, marketLocation, recommendationType);
                using (var outBytes = new MemoryStream())
                {
                    EnumProxy<BuyItemResult>.Serialize(outBytes, result);
                    return outBytes.ToArray();
                }
            }
        }

        Task<byte[]> IShopAsyncWebServiceContract.BuyBundle(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.BuyBundle));

        Task<byte[]> IShopAsyncWebServiceContract.BuyBundleSteam(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.BuyBundleSteam));

        Task<byte[]> IShopAsyncWebServiceContract.BuyPack(byte[] data)
        {
            throw new NotImplementedException();
        }

        Task<byte[]> IShopAsyncWebServiceContract.FinishBuyBundleSteam(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.FinishBuyBundleSteam));

        Task<byte[]> IShopAsyncWebServiceContract.GetAllLuckyDraws_1(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.GetAllLuckyDraws_1));

        Task<byte[]> IShopAsyncWebServiceContract.GetAllLuckyDraws_2(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.GetAllLuckyDraws_2));

        Task<byte[]> IShopAsyncWebServiceContract.GetAllMysteryBoxs_1(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.GetAllMysteryBoxs_1));

        Task<byte[]> IShopAsyncWebServiceContract.GetAllMysteryBoxs_2(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.GetAllMysteryBoxs_2));

        Task<byte[]> IShopAsyncWebServiceContract.GetBundles(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.GetBundles));

        Task<byte[]> IShopAsyncWebServiceContract.GetLuckyDraw(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.GetLuckyDraw));

        Task<byte[]> IShopAsyncWebServiceContract.GetMysteryBox(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.GetMysteryBox));

        async Task<byte[]> IShopAsyncWebServiceContract.GetShop(byte[] data)
        {
            var view = await GetShop();
            if (view == null)
                return null;

            using (var outBytes = new MemoryStream())
            {
                UberStrikeItemShopClientViewProxy.Serialize(outBytes, view);
                return outBytes.ToArray();
            }
        }

        Task<byte[]> IShopAsyncWebServiceContract.RollLuckyDraw(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.RollLuckyDraw));

        Task<byte[]> IShopAsyncWebServiceContract.RollMysteryBox(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.RollMysteryBox));

        Task<byte[]> IShopAsyncWebServiceContract.UseConsumableItem(byte[] data)
        {
            throw new NotImplementedException();
        }

        Task<byte[]> IShopAsyncWebServiceContract.VerifyReceipt(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IShopAsyncWebServiceContract.VerifyReceipt));
    }
}
