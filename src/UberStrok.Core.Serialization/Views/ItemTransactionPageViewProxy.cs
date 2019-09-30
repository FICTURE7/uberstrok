using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ItemTransactionPageViewProxy
    {
        public static void Serialize(Stream stream, ItemTransactionPageView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                if (instance.ItemTransactions != null)
                    ListProxy<ItemTransactionView>.Serialize(bytes, instance.ItemTransactions, ItemTransactionViewProxy.Serialize);
                else
                    mask |= 1;

                Int32Proxy.Serialize(bytes, instance.TotalCount);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static ItemTransactionPageView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var instance = new ItemTransactionPageView();

            if ((mask & 1) != 0)
                instance.ItemTransactions = ListProxy<ItemTransactionView>.Deserialize(bytes, ItemTransactionViewProxy.Deserialize);

            instance.TotalCount = Int32Proxy.Deserialize(bytes);
            return instance;
        }
    }
}
