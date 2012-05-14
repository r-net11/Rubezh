using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converter
{
	public class CombinedDetectorHelper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.CombinedDetector);

			CommonHelper.AddIntProprety(xDriver, 0x84, "Порог срабатывания по дыму", 0, 0, 0, 255);
			CommonHelper.AddIntProprety(xDriver, 0x8B, "Порог срабатывания по температуре", 0, 0, 0, 85);
			CommonHelper.AddIntProprety(xDriver, 0x8C, "Порог срабатывания по градиенту температуры", 0, 0, 0, 255);
		}
	}
}