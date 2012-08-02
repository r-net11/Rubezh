using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class HeatDetectorHelper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.HeatDetector);
			CommonHelper.AddIntProprety(xDriver, 0x8B, "Порог срабатывания по температуре", 0, 0, 0, 85);
			CommonHelper.AddIntProprety(xDriver, 0x8C, "Порог срабатывания по градиенту температуры", 0, 0, 0, 255);
		}
	}
}