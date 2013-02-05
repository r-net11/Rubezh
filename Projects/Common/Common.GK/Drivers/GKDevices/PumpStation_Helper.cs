using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class PumpStation_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x102,
				DriverType = XDriverType.PumpStation,
				UID = new Guid("710CA314-06DD-4997-887F-53F55FA5610F"),
				Name = "Насосная станция",
				ShortName = "НС",
				HasAddress = false,
				IsDeviceOnShleif = false,
			};

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 0,
					Name = "Delay",
					Caption = "Время разновременного пуска, с",
					ToolTip = "Время разновременного пуска, с",
					Min = 2,
					Max = 10,
					Default = 2,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = false
				}
				);

			driver.Properties.Add(
				new XDriverProperty()
				{
					No = 0,
					Name = "PumpCount",
					Caption = "Количество основных насосов",
					ToolTip = "Количество основных насосов",
					Min = 1,
					Max = 100,
					Default = 1,
					DriverPropertyType = XDriverPropertyTypeEnum.IntType,
					IsAUParameter = false
				}
				);

			return driver;
		}
	}
}