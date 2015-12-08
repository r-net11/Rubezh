using System;
using System.Linq;
using GKModule.ViewModels;
using Infrastructure;
using GKModule.Plans;
using Infrastructure.Common.Windows;
using Rhino.Mocks;
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

namespace GKModuleTest
{
	[TestFixture]
	public class ZonesTest
	{
		delegate bool ShowModalWindowDelegate(WindowBaseViewModel windowBaseViewModel);
		delegate bool ShowQuestionDelegate(string message, string title = null);
		delegate void AddResourceDelegate(Assembly callerAssembly, string name);
		delegate void AddRibbonItemsDelegate1(IEnumerable<RibbonMenuItemViewModel> ribbonMenuItems);
		delegate void AddRibbonItemsDelegate2(params RibbonMenuItemViewModel[] ribbonMenuItems);

		GKDevice gkDevice1;
		GKDevice kauDevice11;
		GKDevice kauDevice12;

		GKDevice gkDevice2;
		GKDevice kauDevice21;
		GKDevice kauDevice22;

		[SetUp]
		public void CreateConfiguration()
		{
			GKManager.DeviceLibraryConfiguration = new GKDeviceLibraryConfiguration();
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

		/// <summary>
		/// В список зон можно добавить зону и она добавится в конфигурацию
		/// </summary>
		[Test]
		public void Add()
		{
			var mockRepository = new MockRepository();
			var dialogService = mockRepository.StrictMock<IDialogService>();
			Expect.Call(delegate { dialogService.ShowModalWindow(null); }).IgnoreArguments().Do(new ShowModalWindowDelegate(x =>
			{
				(x as ZoneDetailsViewModel).Name = "Test Zone";
				(x as ZoneDetailsViewModel).SaveCommand.Execute();

				return x.CloseResult.Value;
			}));
			var messageBoxService = mockRepository.StrictMock<IMessageBoxService>();
			Expect.Call(delegate { messageBoxService.ShowQuestion(null, null); }).IgnoreArguments().Do(new ShowQuestionDelegate((x, y) =>
			{
				return true;
			}));
			mockRepository.ReplayAll();

			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = dialogService;
			ServiceFactory.MessageBoxService = messageBoxService;
			ServiceFactory.MenuService = new MenuService(x => { ;});
			ServiceFactory.RibbonService = mockRepository.StrictMock<IRibbonService>();
			var gkPlanExtension = new GKPlanExtension(null, null, null, null, null, null, null, null);

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
			var mockRepository = new MockRepository();
			var dialogService = mockRepository.StrictMock<IDialogService>();
			Expect.Call(delegate { dialogService.ShowModalWindow(null); }).IgnoreArguments().Do(new ShowModalWindowDelegate(x =>
			{
				(x as ZoneDetailsViewModel).CancelCommand.Execute();
				return x.CloseResult.Value;
			}));
			var messageBoxService = mockRepository.StrictMock<IMessageBoxService>();
			Expect.Call(delegate { messageBoxService.ShowQuestion(null, null); }).IgnoreArguments().Do(new ShowQuestionDelegate((x, y) =>
			{
				return true;
			}));
			mockRepository.ReplayAll();

			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = dialogService;
			ServiceFactory.MessageBoxService = messageBoxService;
			ServiceFactory.MenuService = new MenuService(x => { ;});
			ServiceFactory.RibbonService = mockRepository.StrictMock<IRibbonService>();
			var gkPlanExtension = new GKPlanExtension(null, null, null, null, null, null, null, null);

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
			var mockRepository = new MockRepository();
			var dialogService = mockRepository.StrictMock<IDialogService>();
			var messageBoxService = mockRepository.StrictMock<IMessageBoxService>();
			mockRepository.ReplayAll();

			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = dialogService;
			ServiceFactory.MessageBoxService = messageBoxService;
			ServiceFactory.MenuService = new MenuService(x => { ;});
			ServiceFactory.RibbonService = mockRepository.StrictMock<IRibbonService>();
			var gkPlanExtension = new GKPlanExtension(null, null, null, null, null, null, null, null);

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
			var mockRepository = new MockRepository();
			var dialogService = mockRepository.StrictMock<IDialogService>();
			var messageBoxService = mockRepository.StrictMock<IMessageBoxService>();
			mockRepository.ReplayAll();

			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = dialogService;
			ServiceFactory.MessageBoxService = messageBoxService;
			ServiceFactory.MenuService = new MenuService(x => { ;});
			ServiceFactory.RibbonService = mockRepository.StrictMock<IRibbonService>();
			var gkPlanExtension = new GKPlanExtension(null, null, null, null, null, null, null, null);

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
		[STAThread]
		public void AllViewModelsTest()
		{
			var mockRepository = new MockRepository();
			var dialogService = mockRepository.StrictMock<IDialogService>();
			var messageBoxService = mockRepository.StrictMock<IMessageBoxService>();
			var resourceService = mockRepository.StrictMock<IResourceService>();
			Expect.Call(delegate { resourceService.AddResource(null, null); }).IgnoreArguments().Do(new AddResourceDelegate((x, y) =>
			{
				;
			})).Repeat.Any();
			var ribbonService = mockRepository.StrictMock<IRibbonService>();
			Expect.Call(delegate { ribbonService.AddRibbonItems((IEnumerable<RibbonMenuItemViewModel>)null); }).IgnoreArguments().Do(new AddRibbonItemsDelegate1((x) =>
			{
				;
			})).Repeat.Any();
			Expect.Call(delegate { ribbonService.AddRibbonItems((RibbonMenuItemViewModel[])null); }).IgnoreArguments().Do(new AddRibbonItemsDelegate2((x) =>
			{
				;
			})).Repeat.Any();

			Expect.Call(delegate { dialogService.ShowModalWindow(null); }).IgnoreArguments().Do(new ShowModalWindowDelegate(x =>
			{
				var newDeviceViewModel = x as NewDeviceViewModel;
				newDeviceViewModel.SelectedDriver = GKManager.Drivers.FirstOrDefault(y => y.DriverType == GKDriverType.RSR2_AM_1);
				newDeviceViewModel.SaveCommand.Execute();
				return x.CloseResult.Value;
			}));

			ServiceFactory.Initialize(null, null);
			ServiceFactory.ResourceService = resourceService;
			ServiceFactory.DialogService = dialogService;
			ServiceFactory.MessageBoxService = messageBoxService;
			ServiceFactory.MenuService = new MenuService(x => { ;});
			ServiceFactory.RibbonService = ribbonService;

			mockRepository.ReplayAll();

			//AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);

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