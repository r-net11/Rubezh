using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Commom.GK
{
	public class SmokeDetectorHelper
	{
		public static void Create()
		{
			var xDriver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.SmokeDetector);
			CommonHelper.AddIntProprety(xDriver, 0x84, "Порог срабатывания по дыму", 0, 0, 0, 255);
		}
	}
}