using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class AMsHelper
	{
		public static void Create()
		{
			SetProperty(XDriverType.AM_1, 0x81, 0);
		}

		static void SetProperty(XDriverType driverType, byte no, ushort value)
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