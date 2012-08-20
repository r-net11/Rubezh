using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace Common.GK
{
	public static class RM5Helper
	{
		public static XDriver Create()
		{
			var driver = new XDriver()
			{
				DriverType = XDriverType.RM_5,
				UID = new Guid("15164964-9C14-45D1-B92E-1E085C2EC282"),
				Name = "РМ-5",
				ShortName = "РМ-5",
				IsGroupDevice = true,
				AutoChild = XDriverType.RM_1,
				AutoChildCount = 5
			};
			return driver;
		}
	}
}