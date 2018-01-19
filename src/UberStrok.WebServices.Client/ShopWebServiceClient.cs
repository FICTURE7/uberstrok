using System.IO;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Client
{
    public class ShopWebServiceClient : BaseWebServiceClient<IShopWebServiceContract>
    {
        public ShopWebServiceClient(string endPoint) : base(endPoint, "ShopWebService")
        {
            // Space
        }

        public UberStrikeItemShopClientView GetShop()
        {
            using (var bytes = new MemoryStream())
            {
                var data = Channel.GetShop(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return UberStrikeItemShopClientViewProxy.Deserialize(inBytes);
            }
        }
    }
}
