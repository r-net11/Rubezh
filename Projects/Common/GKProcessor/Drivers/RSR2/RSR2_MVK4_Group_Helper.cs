﻿using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_MVK4_Group_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_MVK4_Group,
				UID = new Guid("D3FD9DE2-D2E8-4DAC-B44E-82B202BE3534"),
				Name = "Модуль выходов с контролем МВК4-R2",
				ShortName = "МВК4-R2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_MVK8,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}