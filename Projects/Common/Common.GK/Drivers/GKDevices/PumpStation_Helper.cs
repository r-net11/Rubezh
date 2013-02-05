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

			return driver;
		}
	}
}