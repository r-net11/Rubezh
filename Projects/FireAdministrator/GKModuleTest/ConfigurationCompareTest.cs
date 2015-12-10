using GKModule.ViewModels;
using GKProcessor;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using System.Linq;

namespace GKModuleTest
{
	[TestFixture]
	public class ConfigurationCompareTest
	{
		GKDeviceConfiguration LocalConfiguration { get; set; }
		GKDeviceConfiguration RemoteConfiguration { get; set; }
		GKDevice GkDevice { get; set; }
		void Initialize()
		{
			LocalConfiguration = CreateConfiguration();
			RemoteConfiguration = CreateConfiguration();
			GkDevice = LocalConfiguration.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
		}
		[Test]
		public void CompareSKDZones()
		{
			Initialize();
			var viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, GkDevice);

			var x1 = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			var x2 = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);

			Assert.IsTrue(true);
		}
		[Test]
		public void ComparePumpStations()
		{
			Initialize();

			var viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, GkDevice);
			var localHasDifferences = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			var remoteHasDifferences = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);
			Assert.IsFalse(localHasDifferences);
			Assert.IsFalse(remoteHasDifferences);

			LocalConfiguration.PumpStations.Clear();
			viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, GkDevice);
			localHasDifferences = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			remoteHasDifferences = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);
			Assert.IsTrue(localHasDifferences);
			Assert.IsTrue(remoteHasDifferences);
			var localPumpStation = viewModel.LocalObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			var remotePumpStation = viewModel.RemoteObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			Assert.IsTrue(localPumpStation.DifferenceDiscription == "Отсутствует в локальной конфигурации");
			Assert.IsTrue(localPumpStation.IsAbsent && !localPumpStation.IsPresent);
			Assert.IsTrue(remotePumpStation.DifferenceDiscription == "Отсутствует в локальной конфигурации");
			Assert.IsTrue(!remotePumpStation.IsAbsent && remotePumpStation.IsPresent);

			Initialize();
			RemoteConfiguration.PumpStations.Clear();
			viewModel = new ConfigurationCompareViewModel(LocalConfiguration, RemoteConfiguration, GkDevice);
			localHasDifferences = viewModel.LocalObjectsViewModel.Objects.Any(x => x.HasDifferences);
			remoteHasDifferences = viewModel.RemoteObjectsViewModel.Objects.Any(x => x.HasDifferences);
			Assert.IsTrue(localHasDifferences);
			Assert.IsTrue(remoteHasDifferences);
			localPumpStation = viewModel.LocalObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			remotePumpStation = viewModel.RemoteObjectsViewModel.Objects.FirstOrDefault(x => x.ObjectType == ObjectType.PumpStation);
			Assert.IsTrue(localPumpStation.DifferenceDiscription == "Отсутствует в конфигурации прибора");
			Assert.IsTrue(localPumpStation.IsAbsent == false && localPumpStation.IsPresent == true);
			Assert.IsTrue(remotePumpStation.DifferenceDiscription == "Отсутствует в конфигурации прибора");
			Assert.IsTrue(remotePumpStation.IsAbsent == true && remotePumpStation.IsPresent == false);
		}
		GKDeviceConfiguration CreateConfiguration()
		{
			var newConfiguration = new GKDeviceConfiguration();
			GKDriversCreator.Create();
			var drivers = GKManager.Drivers;
			var systemDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			var gkDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			var kauDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU);
			var alsDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			var am1Driver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_AM_1);
			var smokeDetectorDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_SmokeDetector);
			var handDetectorDriver = drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector);
			var bushDrenazhDriver = drivers.FirstOrDefault(x=>x.DriverType == GKDriverType.RSR2_Bush_Drenazh);

			var am1Device = new GKDevice { DriverUID = am1Driver.UID };
			var smokeDetectorDevice = new GKDevice { DriverUID = smokeDetectorDriver.UID };
			var handDetectorDevice = new GKDevice { DriverUID = handDetectorDriver.UID };
			var bushDrenazhDevice = new GKDevice { DriverUID = bushDrenazhDriver.UID };

			var alsDevice = new GKDevice { DriverUID = alsDriver.UID };
			alsDevice.Children.Add(am1Device);
			alsDevice.Children.Add(smokeDetectorDevice);
			alsDevice.Children.Add(handDetectorDevice);
			alsDevice.Children.Add(bushDrenazhDevice);

			var kauDevice = new GKDevice { DriverUID = kauDriver.UID };
			kauDevice.Children.Add(alsDevice);

			var gkDevice = new GKDevice { DriverUID = gkDriver.UID };
			gkDevice.Children.Add(kauDevice);

			var clause = new GKClause() { ClauseOperationType = ClauseOperationType.AnyDevice, StateType = GKStateBit.Fire1 };
			clause.Devices.Add(am1Device);
			clause.DeviceUIDs.Add(am1Device.UID);
			var clauseGroup = new GKClauseGroup();
			clauseGroup.Clauses.Add(clause);
			var logic = new GKLogic();
			logic.OnClausesGroup = clauseGroup;

			var pumpStation = new GKPumpStation
			{
				Name = "Насосная станция",
				No = 1,
				StartLogic = logic,
				StopLogic = logic,
				AutomaticOffLogic = logic
			};
			pumpStation.NSDevices.Add(bushDrenazhDevice);
			pumpStation.NSDeviceUIDs.Add(bushDrenazhDevice.UID);

			newConfiguration.PumpStations.Add(pumpStation);
			newConfiguration.RootDevice = new GKDevice { DriverUID = systemDriver.UID };
			newConfiguration.RootDevice.Children.Add(gkDevice);  
			newConfiguration.UpdateConfiguration();
			return newConfiguration;
		}
	}
}