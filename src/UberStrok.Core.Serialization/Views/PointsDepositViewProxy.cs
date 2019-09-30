using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PointsDepositViewProxy
    {
        public static void Serialize(Stream stream, PointsDepositView instance)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, instance.Cmid);
                DateTimeProxy.Serialize(bytes, instance.DepositDate);
                EnumProxy<PointsDepositType>.Serialize(bytes, instance.DepositType);
                BooleanProxy.Serialize(bytes, instance.IsAdminAction);
                Int32Proxy.Serialize(bytes, instance.PointDepositId);
                Int32Proxy.Serialize(bytes, instance.Points);
                bytes.WriteTo(stream);
            }
        }

        public static PointsDepositView Deserialize(Stream bytes)
        {
            return new PointsDepositView
            {
                Cmid = Int32Proxy.Deserialize(bytes),
                DepositDate = DateTimeProxy.Deserialize(bytes),
                DepositType = EnumProxy<PointsDepositType>.Deserialize(bytes),
                IsAdminAction = BooleanProxy.Deserialize(bytes),
                PointDepositId = Int32Proxy.Deserialize(bytes),
                Points = Int32Proxy.Deserialize(bytes)
            };
        }
    }
}
