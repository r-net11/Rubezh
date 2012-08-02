using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class AMsHelper
	{
		public static void Create()
		{
			SetProperty(XDriverType.AM_1, 0x81, 0);
			SetProperty(XDriverType.AM1_O, 0x81, 0);
			SetProperty(XDriverType.AM4, 0x81, 0);
			SetProperty(XDriverType.AM4_T, 0x81, 0);
			SetProperty(XDriverType.AM4_P, 0x81, 0);
			SetProperty(XDriverType.AM4_O, 0x81, 0);
			SetProperty(XDriverType.StopButton, 0x81, 0);
			SetProperty(XDriverType.StartButton, 0x81, 0);
			SetProperty(XDriverType.AutomaticButton, 0x81, 0);
			SetProperty(XDriverType.ShuzOnButton, 0x81, 0);
			SetProperty(XDriverType.ShuzOffButton, 0x81, 0);
			SetProperty(XDriverType.ShuzUnblockButton, 0x81, 0);
			SetProperty(XDriverType.AM1_T, 0x81, 0);
			SetProperty(XDriverType.AMT_4, 0x81, 0);
		}

		static void SetProperty(XDriverType driverType, byte no, short value)
		{
			var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == driverType);
			if (driver == null)
			{
				return;
			}
			var driverTypeMappedProperty = new XDriverTypeMappedProperty()
			{
				No = no,
				Value = value
			};
			driver.DriverTypeMappedProperties = new List<XDriverTypeMappedProperty>();
			driver.DriverTypeMappedProperties.Add(driverTypeMappedProperty);
		}
	}
}