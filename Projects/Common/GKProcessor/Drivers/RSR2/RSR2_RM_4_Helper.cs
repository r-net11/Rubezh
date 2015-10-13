﻿using System;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class RSR2_RM_4_Helper
	{
		public static GKDriver Create()
		{
			var driver = new GKDriver()
			{
				DriverType = GKDriverType.RSR2_RM_4,
				UID = new Guid("E3001BBB-18DA-437D-AB73-56C701F070D4"),
				Name = "Модуль релейный РМ4-R2",
				ShortName = "РМ4-R2",
				IsGroupDevice = true,
				GroupDeviceChildType = GKDriverType.RSR2_RM_1,
				GroupDeviceChildrenCount = 4
			};
			return driver;
		}
	}
}