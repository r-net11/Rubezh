using System.Collections.Generic;
using System.Linq;
using GKModule.Validation;
using Infrastructure.Common.Validation;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;

namespace GKProcessor.Test
{
	[TestClass]
	public partial class ValidationTest
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
			var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			Assert.IsNotNull(systemDriver);
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice { Driver = systemDriver, DriverUID = systemDriver.UID };
			gkDevice1 = GKManager.AddDevice(systemDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice11 = GKManager.AddDevice(gkDevice1, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice12 = GKManager.AddDevice(gkDevice1, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);

			gkDevice2 = GKManager.AddDevice(systemDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice21 = GKManager.AddDevice(gkDevice2, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice22 = GKManager.AddDevice(gkDevice2, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);

			GKManager.UpdateConfiguration();
			ClientManager.PlansConfiguration = new PlansConfiguration();
			ClientManager.PlansConfiguration.AllPlans = new List<Plan>();
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddDevice(device.Children[1], GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}

		List<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
	}
}