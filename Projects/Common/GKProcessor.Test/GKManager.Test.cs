﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKProcessor.Test
{
	public class GKManagerTest
	{
		GKDevice gkDevice1;
		GKDevice kauDevice11;
		GKDevice kauDevice12;

		GKDevice gkDevice2;
		GKDevice kauDevice21;
		GKDevice kauDevice22;

		[TestInitialize]
		public void CreateConfiguration()
		{
			GKManager.DeviceConfiguration = new GKDeviceConfiguration();
			GKDriversCreator.Create();
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice { DriverUID = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System).UID };
			gkDevice1 = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice11 = GKManager.AddChild(gkDevice1, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice12 = GKManager.AddChild(gkDevice1, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);

			gkDevice2 = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice21 = GKManager.AddChild(gkDevice2, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice22 = GKManager.AddChild(gkDevice2, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);

			GKManager.UpdateConfiguration();
			ClientManager.PlansConfiguration = new PlansConfiguration();
			ClientManager.PlansConfiguration.AllPlans = new List<Plan>();
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddChild(device.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}

		//[TestMethod]
		public void RemoveDeviceTest()
		{
			var enterDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
		}
	}
}