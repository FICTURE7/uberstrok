using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PointsDepositPageViewProxy
    {
        public static void Serialize(Stream stream, PointsDepositPageView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                if (instance.PointDeposits != null)
                    ListProxy<PointsDepositView>.Serialize(bytes, instance.PointDeposits, PointsDepositViewProxy.Serialize);
                else
                    mask |= 1;

                Int32Proxy.Serialize(bytes, instance.TotalCount);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static PointsDepositPageView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var instance = new PointsDepositPageView();

            if ((mask & 1) != 0)
                instance.PointDeposits = ListProxy<PointsDepositView>.Deserialize(bytes, PointsDepositViewProxy.Deserialize);

            instance.TotalCount = Int32Proxy.Deserialize(bytes);
            return instance;
        }
    }
}
