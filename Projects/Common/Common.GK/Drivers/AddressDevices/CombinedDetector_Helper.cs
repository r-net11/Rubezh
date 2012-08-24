using System.Linq;
using XFiresecAPI;
using System;

namespace Common.GK
{
	public class CombinedDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x60,
				DriverType = XDriverType.CombinedDetector,
				UID = new Guid("37f13667-bc77-4742-829b-1c43fa404c1f"),
				Name = "Пожарный комбинированный извещатель ИП212/101-64-А2R1",
				ShortName = "ИП-64К",
				HasZone = true
			};
			GKDriversHelper.AddIntProprety(driver, 0x84, "Порог срабатывания по дыму", 0, 65, 0, 255);
			GKDriversHelper.AddIntProprety(driver, 0x8B, "Порог срабатывания по температуре", 0, 100, 0, 85);
			GKDriversHelper.AddIntProprety(driver, 0x8C, "Порог срабатывания по градиенту температуры", 0, 100, 0, 255);
			return driver;
		}
	}
}