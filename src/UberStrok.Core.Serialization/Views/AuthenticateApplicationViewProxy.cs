using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class AuthenticateApplicationViewProxy
	{
		public static AuthenticateApplicationView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new AuthenticateApplicationView();
			if ((mask & 1) != 0)
				view.CommServer = PhotonViewProxy.Deserialize(bytes);
			if ((mask & 2) != 0)
				view.EncryptionInitVector = StringProxy.Deserialize(bytes);
			if ((mask & 4) != 0)
				view.EncryptionPassPhrase = StringProxy.Deserialize(bytes);
			if ((mask & 8) != 0)
				view.GameServers = ListProxy<PhotonView>.Deserialize(bytes, PhotonViewProxy.Deserialize);

			view.IsEnabled = BooleanProxy.Deserialize(bytes);
			view.WarnPlayer = BooleanProxy.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, AuthenticateApplicationView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.CommServer != null)
					PhotonViewProxy.Serialize(bytes, instance.CommServer);
				else
					mask |= 1;
				if (instance.EncryptionInitVector != null)
					StringProxy.Serialize(bytes, instance.EncryptionInitVector);
				else
					mask |= 2;
				if (instance.EncryptionPassPhrase != null)
					StringProxy.Serialize(bytes, instance.EncryptionPassPhrase);
				else
					mask |= 4;
				if (instance.GameServers != null)
					ListProxy<PhotonView>.Serialize(bytes, instance.GameServers, PhotonViewProxy.Serialize);
				else
					mask |= 8;

				BooleanProxy.Serialize(bytes, instance.IsEnabled);
				BooleanProxy.Serialize(bytes, instance.WarnPlayer);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
