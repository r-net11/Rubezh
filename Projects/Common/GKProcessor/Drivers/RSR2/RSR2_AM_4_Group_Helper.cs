using System;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class RSR2_AM_4_Group_Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RSR2_AM_4,
				UID = new Guid("79EAC50A-D534-4775-A102-BE4872877400"),
				Name = "МЕТКА АДРЕСНАЯ АМ4-R2",
				ShortName = "АМ4-R2",
				IsGroupDevice = true,
				GroupDeviceChildType = XDriverType.RSR2_AM_1,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}