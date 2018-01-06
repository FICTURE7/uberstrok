using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ApplicationConfigurationViewProxy
	{
		public static ApplicationConfigurationView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new ApplicationConfigurationView();
			view.MaxLevel = Int32Proxy.Deserialize(bytes);
			view.MaxXp = Int32Proxy.Deserialize(bytes);
			view.PointsBaseLoser = Int32Proxy.Deserialize(bytes);
			view.PointsBaseWinner = Int32Proxy.Deserialize(bytes);
			view.PointsHeadshot = Int32Proxy.Deserialize(bytes);
			view.PointsKill = Int32Proxy.Deserialize(bytes);
			view.PointsNutshot = Int32Proxy.Deserialize(bytes);
			view.PointsPerMinuteLoser = Int32Proxy.Deserialize(bytes);
			view.PointsPerMinuteWinner = Int32Proxy.Deserialize(bytes);
			view.PointsSmackdown = Int32Proxy.Deserialize(bytes);
			view.XpBaseLoser = Int32Proxy.Deserialize(bytes);
			view.XpBaseWinner = Int32Proxy.Deserialize(bytes);
			view.XpHeadshot = Int32Proxy.Deserialize(bytes);
			view.XpKill = Int32Proxy.Deserialize(bytes);
			view.XpNutshot = Int32Proxy.Deserialize(bytes);
			view.XpPerMinuteLoser = Int32Proxy.Deserialize(bytes);
			view.XpPerMinuteWinner = Int32Proxy.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.XpRequiredPerLevel = DictionaryProxy<int, int>.Deserialize(bytes, Int32Proxy.Deserialize, Int32Proxy.Deserialize);

            view.XpSmackdown = Int32Proxy.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, ApplicationConfigurationView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.MaxLevel);
				Int32Proxy.Serialize(bytes, instance.MaxXp);
				Int32Proxy.Serialize(bytes, instance.PointsBaseLoser);
				Int32Proxy.Serialize(bytes, instance.PointsBaseWinner);
				Int32Proxy.Serialize(bytes, instance.PointsHeadshot);
				Int32Proxy.Serialize(bytes, instance.PointsKill);
				Int32Proxy.Serialize(bytes, instance.PointsNutshot);
				Int32Proxy.Serialize(bytes, instance.PointsPerMinuteLoser);
				Int32Proxy.Serialize(bytes, instance.PointsPerMinuteWinner);
				Int32Proxy.Serialize(bytes, instance.PointsSmackdown);
				Int32Proxy.Serialize(bytes, instance.XpBaseLoser);
				Int32Proxy.Serialize(bytes, instance.XpBaseWinner);
				Int32Proxy.Serialize(bytes, instance.XpHeadshot);
				Int32Proxy.Serialize(bytes, instance.XpKill);
				Int32Proxy.Serialize(bytes, instance.XpNutshot);
				Int32Proxy.Serialize(bytes, instance.XpPerMinuteLoser);
				Int32Proxy.Serialize(bytes, instance.XpPerMinuteWinner);

                if (instance.XpRequiredPerLevel != null)
                    DictionaryProxy<int, int>.Serialize(bytes, instance.XpRequiredPerLevel, Int32Proxy.Serialize, Int32Proxy.Serialize);
                else
                    mask |= 1;

				Int32Proxy.Serialize(bytes, instance.XpSmackdown);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
