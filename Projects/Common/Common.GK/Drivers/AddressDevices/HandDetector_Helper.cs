using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class HandDetector_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverTypeNo = 0x55,
				DriverType = XDriverType.HandDetector,
				UID = new Guid("641fa899-faa0-455b-b626-646e5fbe785a"),
				Name = "Ручной извещатель ИПР513-11",
				ShortName = "РПИ",
				HasZone = true
			};
			return driver;
		}
	}
}