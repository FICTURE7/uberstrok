using System.IO;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;

namespace UbzStuff.Core.Serialization.Views
{
    public static class PublicProfileViewProxy
	{
		public static PublicProfileView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new PublicProfileView();
			view.AccessLevel = EnumProxy<MemberAccessLevel>.Deserialize(bytes);
			view.Cmid = Int32Proxy.Deserialize(bytes);
			view.EmailAddressStatus = EnumProxy<EmailAddressStatus>.Deserialize(bytes);
			if ((mask & 1) != 0)
				view.FacebookId = StringProxy.Deserialize(bytes);
			if ((mask & 2) != 0)
				view.GroupTag = StringProxy.Deserialize(bytes);

            view.IsChatDisabled = BooleanProxy.Deserialize(bytes);
			view.LastLoginDate = DateTimeProxy.Deserialize(bytes);
			if ((mask & 4) != 0)
				view.Name = StringProxy.Deserialize(bytes);

            return view;
		}

		public static void Serialize(Stream stream, PublicProfileView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				EnumProxy<MemberAccessLevel>.Serialize(bytes, instance.AccessLevel);
				Int32Proxy.Serialize(bytes, instance.Cmid);
				EnumProxy<EmailAddressStatus>.Serialize(bytes, instance.EmailAddressStatus);

                if (instance.FacebookId != null)
					StringProxy.Serialize(bytes, instance.FacebookId);
				else
					mask |= 1;
				if (instance.GroupTag != null)
					StringProxy.Serialize(bytes, instance.GroupTag);
				else
					mask |= 2;

                BooleanProxy.Serialize(bytes, instance.IsChatDisabled);
				DateTimeProxy.Serialize(bytes, instance.LastLoginDate);

                if (instance.Name != null)
					StringProxy.Serialize(bytes, instance.Name);
				else
					mask |= 4;

                Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
