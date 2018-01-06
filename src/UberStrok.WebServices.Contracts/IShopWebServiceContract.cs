using System.ServiceModel;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IShopWebServiceContract
    {
        [OperationContract]
        byte[] GetShop(byte[] data);

        [OperationContract]
        byte[] BuyItem(byte[] data);

        [OperationContract]
        byte[] BuyPack(byte[] data);

        [OperationContract]
        byte[] GetBundles(byte[] data);

        [OperationContract]
        byte[] BuyBundle(byte[] data);

        [OperationContract]
        byte[] BuyBundleSteam(byte[] data);

        [OperationContract]
        byte[] FinishBuyBundleSteam(byte[] data);

        [OperationContract]
        byte[] VerifyReceipt(byte[] data);

        [OperationContract]
        byte[] UseConsumableItem(byte[] data);

        [OperationContract]
        byte[] GetAllMysteryBoxs_1(byte[] data);

        [OperationContract]
        byte[] GetAllMysteryBoxs_2(byte[] data);

        [OperationContract]
        byte[] GetMysteryBox(byte[] data);

        [OperationContract]
        byte[] RollMysteryBox(byte[] data);

        [OperationContract]
        byte[] GetAllLuckyDraws_1(byte[] data);

        [OperationContract]
        byte[] GetAllLuckyDraws_2(byte[] data);

        [OperationContract]
        byte[] GetLuckyDraw(byte[] data);

        [OperationContract]
        byte[] RollLuckyDraw(byte[] data);
    }
}