using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IShopAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> GetShop(byte[] data);

        [OperationContract]
        Task<byte[]> BuyItem(byte[] data);

        [OperationContract]
        Task<byte[]> BuyPack(byte[] data);

        [OperationContract]
        Task<byte[]> GetBundles(byte[] data);

        [OperationContract]
        Task<byte[]> BuyBundle(byte[] data);

        [OperationContract]
        Task<byte[]> BuyBundleSteam(byte[] data);

        [OperationContract]
        Task<byte[]> FinishBuyBundleSteam(byte[] data);

        [OperationContract]
        Task<byte[]> VerifyReceipt(byte[] data);

        [OperationContract]
        Task<byte[]> UseConsumableItem(byte[] data);

        [OperationContract]
        Task<byte[]> GetAllMysteryBoxs_1(byte[] data);

        [OperationContract]
        Task<byte[]> GetAllMysteryBoxs_2(byte[] data);

        [OperationContract]
        Task<byte[]> GetMysteryBox(byte[] data);

        [OperationContract]
        Task<byte[]> RollMysteryBox(byte[] data);

        [OperationContract]
        Task<byte[]> GetAllLuckyDraws_1(byte[] data);

        [OperationContract]
        Task<byte[]> GetAllLuckyDraws_2(byte[] data);

        [OperationContract]
        Task<byte[]> GetLuckyDraw(byte[] data);

        [OperationContract]
        Task<byte[]> RollLuckyDraw(byte[] data);
    }
}