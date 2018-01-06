using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class UberstrikeUserViewProxy
	{
		public static UberstrikeUserView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new UberstrikeUserView();
            
			if ((mask & 1) != 0)
				view.CmuneMemberView = MemberViewProxy.Deserialize(bytes);
			if ((mask & 2) != 0)
				view.UberstrikeMemberView = UberstrikeMemberViewProxy.Deserialize(bytes);

            return view;
		}

		public static void Serialize(Stream stream, UberstrikeUserView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.CmuneMemberView != null)
					MemberViewProxy.Serialize(bytes, instance.CmuneMemberView);
				else
					mask |= 1;
				if (instance.UberstrikeMemberView != null)
					UberstrikeMemberViewProxy.Serialize(bytes, instance.UberstrikeMemberView);
				else
					mask |= 2;

                Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
