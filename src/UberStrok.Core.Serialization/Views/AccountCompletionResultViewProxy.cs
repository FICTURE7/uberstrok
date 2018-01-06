using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class AccountCompletionResultViewProxy
	{
		public static AccountCompletionResultView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new AccountCompletionResultView();

            if ((mask & 1) != 0)
				view.ItemsAttributed = DictionaryProxy<int, int>.Deserialize(bytes, Int32Proxy.Deserialize, Int32Proxy.Deserialize);
			if ((mask & 2) != 0)
				view.NonDuplicateNames = ListProxy<string>.Deserialize(bytes, StringProxy.Deserialize);

			view.Result = Int32Proxy.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, AccountCompletionResultView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.ItemsAttributed != null)
					DictionaryProxy<int, int>.Serialize(bytes, instance.ItemsAttributed, Int32Proxy.Serialize, Int32Proxy.Serialize);
				else
					mask |= 1;
				if (instance.NonDuplicateNames != null)
					ListProxy<string>.Serialize(bytes, instance.NonDuplicateNames, StringProxy.Serialize);
				else
					mask |= 2;

                Int32Proxy.Serialize(bytes, instance.Result);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
