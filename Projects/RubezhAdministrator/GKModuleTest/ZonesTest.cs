using System;
using System.Linq;
using GKModule.ViewModels;
using Infrastructure;
using GKModule.Plans;
using Infrastructure.Common.Windows;
using Infrastructure.Services;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System.Collections.Generic;
using GKProcessor;
using Infrastructure.Common.Services.Ribbon;
using NUnit.Framework;
using System.Windows;
using GKModule;
using Infrastructure.Common;
using System.Reflection;
using Infrastructure.Common.Ribbon;
using RubezhAPI;
using Infrastructure.Common.Services;

namespace GKModuleTest
{
	[TestFixture]
	public class ZonesTest
	{
		GKDevice gkDevice1;
		GKDevice kauDevice11;
		GKDevice kauDevice12;

		GKDevice gkDevice2;
		GKDevice kauDevice21;
		GKDevice kauDevice22;

		GroupControllerModule GroupControllerModule;
		MockDialogService MockDialogService;
		MockMessageBoxService MockMessageBoxService;

		[SetUp]
		public void CreateConfiguration()
		{
			GKManager.DeviceLibraryConfiguration = new GKDeviceLibraryConfiguration();
			GKManager.DeviceConfiguration = new GKDeviceConfiguration();
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

			ServiceFactory.Initialize(null, null);
			ServiceFactory.ResourceService = new MockResourceService();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			ServiceFactory.MessageBoxService = MockMessageBoxService = new MockMessageBoxService();
			ServiceFactory.MenuService = new MenuService(x => { ;});
			ServiceFactory.RibbonService = new MockRibbonService();

			CreateGroupControllerModule();
		}

		void CreateGroupControllerModule()
		{
			GroupControllerModule = new GroupControllerModule();
			GroupControllerModule.CreateViewModels();
			GroupControllerModule.Initialize();
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddDevice(device.Children[1], GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}

		/// <summary>
		/// В список зон можно добавить зону и она добавится в конфигурацию
		/// </summary>
		[Test]
		public void Add()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as ZoneDetailsViewModel).Name = "Test Zone";
				(x as ZoneDetailsViewModel).SaveCommand.Execute();

			};

			MockMessageBoxService.ShowConfirmationResult = true;

			var zonesViewModel = new ZonesViewModel();
			zonesViewModel.Initialize();
			zonesViewModel.OnShow();

			Assert.IsFalse(zonesViewModel.DeleteCommand.CanExecute(null));
			Assert.IsTrue(zonesViewModel.Zones.Count == 0);
			Assert.IsTrue(zonesViewModel.SelectedZone == null);
			zonesViewModel.AddCommand.Execute();
			Assert.IsTrue(zonesViewModel.Zones.Count == 1);
			Assert.IsTrue(zonesViewModel.SelectedZone != null);
			Assert.IsTrue(zonesViewModel.Zones.FirstOrDefault().Zone.Name == "Test Zone");

			Assert.IsTrue(zonesViewModel.DeleteCommand.CanExecute(null));
			zonesViewModel.DeleteCommand.Execute();
			Assert.IsTrue(zonesViewModel.Zones.Count == 0);
			Assert.IsTrue(GKManager.Zones.Count == 0);
		}

		/// <summary>
		/// При отмене создания зоны она не должна добавиться в список зон
		/// </summary>
		[Test]
		public void AddCancel()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as ZoneDetailsViewModel).CancelCommand.Execute();

			};

			var zonesViewModel = new ZonesViewModel();
			zonesViewModel.Initialize();
			zonesViewModel.OnShow();

			Assert.IsTrue(zonesViewModel.Zones.Count == 0);
			Assert.IsTrue(zonesViewModel.SelectedZone == null);
			zonesViewModel.AddCommand.Execute();
			Assert.IsTrue(zonesViewModel.Zones.Count == 0);
			Assert.IsTrue(zonesViewModel.SelectedZone == null);
		}

		/// <summary>
		/// Если нет ни одной зоны, то список доступных устройств пуст
		/// </summary>
		[Test]
		public void AddDeviceToNoZone()
		{
			AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			GKManager.UpdateConfiguration();

			var zonesViewModel = new ZonesViewModel();
			zonesViewModel.Initialize();
			zonesViewModel.OnShow();
			Assert.IsTrue(zonesViewModel.ZoneDevices.Devices.Count == 0);
			Assert.IsTrue(zonesViewModel.ZoneDevices.AvailableDevices.Count == 0);
		}

		/// <summary>
		/// Если есть зона и извещательные устройства, по устройство можно добавить в зону и при этом изменится конфигурация
		/// </summary>
		[Test]
		public void AddDeviceToZone()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var zone = new GKZone();
			GKManager.AddZone(zone);
			GKManager.UpdateConfiguration();

			var zonesViewModel = new ZonesViewModel();
			zonesViewModel.Initialize();
			zonesViewModel.OnShow();
			Assert.IsTrue(zonesViewModel.ZoneDevices.Devices.Count == 0);
			Assert.IsTrue(zonesViewModel.ZoneDevices.AvailableDevices.Count == 3);
			zonesViewModel.ZoneDevices.AddCommand.Execute(zonesViewModel.ZoneDevices.AvailableDevices.Where(x => x.Device.UID == device1.UID).ToList());
			Assert.IsTrue(zonesViewModel.ZoneDevices.Devices.Count == 1);
			Assert.IsTrue(zonesViewModel.ZoneDevices.AvailableDevices.Count == 2);
			Assert.IsTrue(zone.Devices.Count == 1);
			Assert.IsTrue(device1.Zones.Count == 1);
		}

		/// <summary>
		/// Тест на создание всех вьюмоделей
		/// </summary>
		[Test]
		public void AllViewModelsTest()
		{
			MockDialogService.OnShowModal += x =>
			{
				var newDeviceViewModel = x as NewDeviceViewModel;
				newDeviceViewModel.SelectedDriver.Driver = GKManager.Drivers.FirstOrDefault(y => y.DriverType == GKDriverType.RSR2_AM_1);
				newDeviceViewModel.SaveCommand.Execute();
			};

			var groupControllerModule = new GroupControllerModule();
			groupControllerModule.CreateViewModels();
			groupControllerModule.Initialize();


			var devicesViewModel = groupControllerModule.DevicesViewModel;
			devicesViewModel.OnShow();
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.That(selectedDevice.Children.Count(), Is.EqualTo(1));
		}
	}
}