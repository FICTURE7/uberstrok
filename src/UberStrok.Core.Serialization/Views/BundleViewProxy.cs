using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class BundleViewProxy
    {
        public static void Serialize(Stream stream, BundleView instance)
        {
            int num = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (instance.AndroidStoreUniqueId != null)
                {
                    StringProxy.Serialize(memoryStream, instance.AndroidStoreUniqueId);
                }
                else
                {
                    num |= 1;
                }
                Int32Proxy.Serialize(memoryStream, instance.ApplicationId);
                if (instance.Availability != null)
                {
                    ListProxy<ChannelType>.Serialize(memoryStream, instance.Availability, new ListProxy<ChannelType>.Serializer<ChannelType>(EnumProxy<ChannelType>.Serialize));
                }
                else
                {
                    num |= 2;
                }
                if (instance.BundleItemViews != null)
                {
                    ListProxy<BundleItemView>.Serialize(memoryStream, instance.BundleItemViews, new ListProxy<BundleItemView>.Serializer<BundleItemView>(BundleItemViewProxy.Serialize));
                }
                else
                {
                    num |= 4;
                }
                EnumProxy<BundleCategoryType>.Serialize(memoryStream, instance.Category);
                Int32Proxy.Serialize(memoryStream, instance.Credits);
                if (instance.Description != null)
                {
                    StringProxy.Serialize(memoryStream, instance.Description);
                }
                else
                {
                    num |= 8;
                }
                if (instance.IconUrl != null)
                {
                    StringProxy.Serialize(memoryStream, instance.IconUrl);
                }
                else
                {
                    num |= 16;
                }
                Int32Proxy.Serialize(memoryStream, instance.Id);
                if (instance.ImageUrl != null)
                {
                    StringProxy.Serialize(memoryStream, instance.ImageUrl);
                }
                else
                {
                    num |= 32;
                }
                if (instance.IosAppStoreUniqueId != null)
                {
                    StringProxy.Serialize(memoryStream, instance.IosAppStoreUniqueId);
                }
                else
                {
                    num |= 64;
                }
                BooleanProxy.Serialize(memoryStream, instance.IsDefault);
                BooleanProxy.Serialize(memoryStream, instance.IsOnSale);
                BooleanProxy.Serialize(memoryStream, instance.IsPromoted);
                if (instance.MacAppStoreUniqueId != null)
                {
                    StringProxy.Serialize(memoryStream, instance.MacAppStoreUniqueId);
                }
                else
                {
                    num |= 128;
                }
                if (instance.Name != null)
                {
                    StringProxy.Serialize(memoryStream, instance.Name);
                }
                else
                {
                    num |= 256;
                }
                Int32Proxy.Serialize(memoryStream, instance.Points);
                if (instance.PromotionTag != null)
                {
                    StringProxy.Serialize(memoryStream, instance.PromotionTag);
                }
                else
                {
                    num |= 512;
                }
                DecimalProxy.Serialize(memoryStream, instance.USDPrice);
                DecimalProxy.Serialize(memoryStream, instance.USDPromoPrice);
                Int32Proxy.Serialize(stream, ~num);
                memoryStream.WriteTo(stream);
            }
        }

        public static BundleView Deserialize(Stream bytes)
        {
            int num = Int32Proxy.Deserialize(bytes);
            BundleView bundleView = new BundleView();
            if ((num & 1) != 0)
            {
                bundleView.AndroidStoreUniqueId = StringProxy.Deserialize(bytes);
            }
            bundleView.ApplicationId = Int32Proxy.Deserialize(bytes);
            if ((num & 2) != 0)
            {
                bundleView.Availability = ListProxy<ChannelType>.Deserialize(bytes, new ListProxy<ChannelType>.Deserializer<ChannelType>(EnumProxy<ChannelType>.Deserialize));
            }
            if ((num & 4) != 0)
            {
                bundleView.BundleItemViews = ListProxy<BundleItemView>.Deserialize(bytes, new ListProxy<BundleItemView>.Deserializer<BundleItemView>(BundleItemViewProxy.Deserialize));
            }
            bundleView.Category = EnumProxy<BundleCategoryType>.Deserialize(bytes);
            bundleView.Credits = Int32Proxy.Deserialize(bytes);
            if ((num & 8) != 0)
            {
                bundleView.Description = StringProxy.Deserialize(bytes);
            }
            if ((num & 16) != 0)
            {
                bundleView.IconUrl = StringProxy.Deserialize(bytes);
            }
            bundleView.Id = Int32Proxy.Deserialize(bytes);
            if ((num & 32) != 0)
            {
                bundleView.ImageUrl = StringProxy.Deserialize(bytes);
            }
            if ((num & 64) != 0)
            {
                bundleView.IosAppStoreUniqueId = StringProxy.Deserialize(bytes);
            }
            bundleView.IsDefault = BooleanProxy.Deserialize(bytes);
            bundleView.IsOnSale = BooleanProxy.Deserialize(bytes);
            bundleView.IsPromoted = BooleanProxy.Deserialize(bytes);
            if ((num & 128) != 0)
            {
                bundleView.MacAppStoreUniqueId = StringProxy.Deserialize(bytes);
            }
            if ((num & 256) != 0)
            {
                bundleView.Name = StringProxy.Deserialize(bytes);
            }
            bundleView.Points = Int32Proxy.Deserialize(bytes);
            if ((num & 512) != 0)
            {
                bundleView.PromotionTag = StringProxy.Deserialize(bytes);
            }
            bundleView.USDPrice = DecimalProxy.Deserialize(bytes);
            bundleView.USDPromoPrice = DecimalProxy.Deserialize(bytes);
            return bundleView;
        }
    }
}