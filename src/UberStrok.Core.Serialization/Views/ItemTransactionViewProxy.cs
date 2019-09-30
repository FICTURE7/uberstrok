using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ItemTransactionViewProxy
    {
        public static void Serialize(Stream stream, ItemTransactionView instance)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, instance.Cmid);
                Int32Proxy.Serialize(bytes, instance.Credits);
                EnumProxy<BuyingDurationType>.Serialize(bytes, instance.Duration);
                BooleanProxy.Serialize(bytes, instance.IsAdminAction);
                Int32Proxy.Serialize(bytes, instance.ItemId);
                Int32Proxy.Serialize(bytes, instance.Points);
                DateTimeProxy.Serialize(bytes, instance.WithdrawalDate);
                Int32Proxy.Serialize(bytes, instance.WithdrawalId);
                bytes.WriteTo(stream);
            }
        }

        public static ItemTransactionView Deserialize(Stream bytes)
        {
            return new ItemTransactionView
            {
                Cmid = Int32Proxy.Deserialize(bytes),
                Credits = Int32Proxy.Deserialize(bytes),
                Duration = EnumProxy<BuyingDurationType>.Deserialize(bytes),
                IsAdminAction = BooleanProxy.Deserialize(bytes),
                ItemId = Int32Proxy.Deserialize(bytes),
                Points = Int32Proxy.Deserialize(bytes),
                WithdrawalDate = DateTimeProxy.Deserialize(bytes),
                WithdrawalId = Int32Proxy.Deserialize(bytes)
            };
        }
    }
}
