using GKModule;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Services;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System.Collections.Generic;
using System.Linq;
namespace GKModuleTest
{
	[TestFixture]
	public class AllViewModelsTest
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
		public void Initialize()
		{
			GKManager.DeviceLibraryConfiguration = new GKDeviceLibraryConfiguration();
			GKManager.DeviceConfiguration = new GKDeviceConfiguration();
			GKDriversCreator.Create();
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice { DriverUID = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System).UID };
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
			ServiceFactory.MenuService = new MenuService(x => {; });
			ServiceFactory.RibbonService = new MockRibbonService();

			CreateGroupControllerModule();
		}

		void CreateGroupControllerModule()
		{
			GroupControllerModule = new GroupControllerModule();
			GroupControllerModule.CreateViewModels();
			GroupControllerModule.Initialize();
		}

		[Test]
		public void AddDevice()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 1);
			Assert.IsTrue(selectedDevice.Device.Children.All(x => x.DriverType == GKDriverType.RSR2_AM_1));
			Assert.IsTrue(selectedDevice.Device.Children.All(x => x.AllParents.Count == 4));
			Assert.IsTrue(selectedDevice.Device.Children.All(x => x.Parent == selectedDevice.Device));
			Assert.IsTrue(selectedDevice.Device.Children[0].IntAddress == 1);
		}

		[Test]
		public void AddDeviceInStartList()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_4, addInStartList: true);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_AM_4).IntAddress == 1);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault(x => x.DriverType == GKDriverType.RSR2_AM_1).IntAddress == 5);
		}
		[Test]
		public void InsertGroupDevice()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_4);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_AM_1);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 0);
			Assert.IsTrue(selectedDevice.Device.IntAddress == 1);
			Assert.IsTrue(selectedDevice.Device.Parent.Children.LastOrDefault().IntAddress == 2);
			Assert.IsTrue(selectedDevice.Device.Parent.Children.LastOrDefault().Parent.DriverType == GKDriverType.RSR2_KAU_Shleif);
			Assert.IsTrue(selectedDevice.Device.Parent.Children.LastOrDefault().Children.All(x => x.Parent.DriverType == GKDriverType.RSR2_AM_4));
		}

		[Test]
		public void InsertDevice()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_4);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_AM_4);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 4);
			Assert.IsTrue(selectedDevice.Device.IntAddress == 1);
			Assert.IsTrue(selectedDevice.Device.Children[0].IntAddress == 1);
			Assert.IsTrue(selectedDevice.Device.Children[1].IntAddress == 2);
			Assert.IsTrue(selectedDevice.Device.Children[2].IntAddress == 3);
			Assert.IsTrue(selectedDevice.Device.Children[3].IntAddress == 4);
			Assert.IsTrue(selectedDevice.Device.Parent.Children.LastOrDefault().IntAddress == 5);
			Assert.IsTrue(selectedDevice.Device.Parent.Children.LastOrDefault().Parent.DriverType == GKDriverType.RSR2_KAU_Shleif);
			Assert.IsTrue(selectedDevice.Device.Parent.Children.LastOrDefault().Children.All(x => x.Parent.DriverType == GKDriverType.RSR2_KAU_Shleif));
		}

		[TestCase(GKDriverType.RSR2_AM_1)]
		//[TestCase(GKDriverType.RSR2_AM_4)]
		public void AddDevicePositiveValueForRSR(GKDriverType driverType)
		{
			SetMokForNewDeviceViewModel(driverType, 255);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 255);
		}

		[Test]
		public void AddGroupDevicePositiveValueForRSR2()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1, 253);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_2);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Count() == 256);
			Assert.IsTrue(selectedDevice.Device.Children.Count() == 254);
		}

		[Test]
		public void AddDeviceNegativeValueForRSR2()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1, 256);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 0);
		}

		[Test]
		public void AddGroupDeviceNegativeValueForRSR2()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1, 254);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_2);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Count() == 254);
			Assert.IsTrue(selectedDevice.Device.Children.Count() == 254);
		}

		[Test]
		public void AddDevicePositiveValueForGK()
		{
			SetMokForNewDeviceViewModel(GKDriverType.GKMirror, 123);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 127);
		}

		[Test]
		public void AddDeviceNegativeValueForGK()
		{
			SetMokForNewDeviceViewModel(GKDriverType.GKMirror, 128);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.Count() == 4);
		}


		[Test]
		public void AddGroupDevice()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_4);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 1);
			Assert.IsTrue(selectedDevice.Device.AllChildren.Max(x => x.IntAddress) == 4);
			Assert.IsTrue(selectedDevice.Device.Children[0].DriverType == GKDriverType.RSR2_AM_4);
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.Count == 4);
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.All(x => x.DriverType == GKDriverType.RSR2_AM_1));
			Assert.IsTrue(selectedDevice.Device.Children.All(x => x.AllParents.Count == 4));
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.All(x => x.AllParents.Count == 5));
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.All(x => selectedDevice.Children.All(y => y.Device == x.Parent)));
		}

		[Test]
		public void AddGK()
		{
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.System);
			int countGK = selectedDevice.Children.Count();
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() > countGK);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Count() == 2);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[0].DriverType == GKDriverType.GKIndicatorsGroup);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[1].DriverType == GKDriverType.GKRelaysGroup);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[0].Children.All(x => x.DriverType == GKDriverType.GKIndicator));
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[1].Children.All(x => x.DriverType == GKDriverType.GKRele));
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[0].Children.Count == 16);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[1].Children.Count == 5);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.AllParents.Count() == 1);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.Parent == selectedDevice.Device);
		}

		[Test]
		public void AddGroupDeviceOPSZ()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_OPSZ);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			devicesViewModel.OnShow();
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 1);
			Assert.IsTrue(selectedDevice.Device.Children[0].DriverType == GKDriverType.RSR2_OPSZ);
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.Count() == 2);
			Assert.IsTrue(selectedDevice.Device.Children[0].Children[0].DriverType == GKDriverType.RSR2_OPKS);
			Assert.IsTrue(selectedDevice.Device.Children[0].Children[1].DriverType == GKDriverType.RSR2_OPKZ);
		}

		[Test]
		public void AddKAU()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_KAU);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			devicesViewModel.OnShow();
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			int count = selectedDevice.Device.Children.Count();
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.Count() > count);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Count() == 9);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Where(x => x.Driver.DriverType == GKDriverType.KAUIndicator).Count() == 1);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Where(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif).Count() == 8);
		}
		[Test]
		public void AddMirror()
		{
			SetMokForNewDeviceViewModel(GKDriverType.GKMirror);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			devicesViewModel.OnShow();
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			int count = selectedDevice.Device.Children.Count();
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.Count() > count);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Count() == 2);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.Children[0].DriverType == GKDriverType.GKIndicatorsGroup);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.Children[1].DriverType == GKDriverType.GKRelaysGroup);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[0].Children.All(x => x.DriverType == GKDriverType.GKIndicator));
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[1].Children.All(x => x.DriverType == GKDriverType.GKRele));
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[0].Children.Count == 16);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[1].Children.Count == 5);
			Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().IntAddress - selectedDevice.Device.Children[count - 1].IntAddress == 1);
		}
		[Test]
		public void AddMirrorItem()
		{
			SetMokForNewDeviceViewModel(GKDriverType.GKMirror);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.FireZonesMirror, 2);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GKMirror);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.Count(x => x.Driver.HasMirror == true) == 2);
			Assert.IsTrue(selectedDevice.Device.Children.Where(x => x.Driver.HasMirror == true).FirstOrDefault().IntAddress == 1);
			Assert.IsTrue(selectedDevice.Device.Children.Where(x => x.Driver.HasMirror == true).LastOrDefault().IntAddress == 2);
		}
		[Test]
		public void AddPositivMVP()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_4, 63);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Max(x => x.IntAddress) == 252);
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_MVP);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Max(x => x.IntAddress) == 253);
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_2);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_MVP_Part);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Count == 3);
			Assert.IsTrue(devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif).Device.AllChildren.Max(x => x.IntAddress) == 255);
		}

		[Test]
		public void AddNegativMVP()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_4, 63);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Max(x => x.IntAddress) == 252);
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_MVP);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Max(x => x.IntAddress) == 253);
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_4);
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_MVP_Part);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.AllChildren.Count == 0);
			Assert.IsTrue(devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif).Device.AllChildren.Max(x => x.IntAddress) == 253);
		}

		[Test]
		public void ChangeGroupDeviceWithZoneTest()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1, 3);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			var zone = new GKZone();
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(selectedDevice.Device.Children[0], zone);
			selectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Device == selectedDevice.Device.Children[0]);
			selectedDevice.Driver = RSR2_AM_4_Group_Helper.Create();
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			Assert.IsTrue(selectedDevice.Device.Children.Count(x => x.DriverType == GKDriverType.RSR2_AM_4) == 1);
			Assert.IsTrue(selectedDevice.Device.Children[1].IntAddress == 5);
			Assert.IsTrue(zone.Devices.Count == 0);
			Assert.IsTrue(zone.OutputDependentElements.Count == 0);
			Assert.IsTrue(selectedDevice.Device.Children[0].InputDependentElements.Count == 0);
			Assert.IsTrue(selectedDevice.Device.Children.All(x => x.InputDependentElements.Count == 0));
			Assert.IsTrue(selectedDevice.Device.Children[0].Driver.IsGroupDevice);
			Assert.IsTrue(selectedDevice.Device.Children[0].Driver.GroupDeviceChildrenCount == 4);
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.Count == 4);
		}
		[Test]
		public void ChangeGroupDeviceWithLogicTest()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_MDU);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			GKManager.UpdateConfiguration();
			var delay = new GKDelay();
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { selectedDevice.Device.Children[0].UID }
			};

			var gkLogic = new GKLogic();
			gkLogic.OnClausesGroup.Clauses.Add(clause);
			GKManager.AddDelay(delay);
			GKManager.SetDelayLogic(delay, gkLogic);
			GKManager.SetDeviceLogic(selectedDevice.Device.Children[0], gkLogic);
			selectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Device == selectedDevice.Device.Children[0]);
			selectedDevice.Driver = RSR2_RM_2_Helper.Create();
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			var device = selectedDevice.Device.Children[0];
			Assert.IsTrue(device.DriverType == GKDriverType.RSR2_RM_2);
			Assert.IsTrue(device.Logic.GetObjects().Count == 0);
			Assert.IsTrue(device.InputDependentElements.Count == 0);
			Assert.IsTrue(delay.OutputDependentElements.Count == 0);
			Assert.IsTrue(delay.Logic.GetObjects().Count == 0);
		}
		[Test]
		public void ChangeDeviceWithMaxAddresOnShleif()
		{
			SetMokForNewDeviceViewModel(GKDriverType.RSR2_AM_1, 255);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsNull(GKManager.ChangeDriver(selectedDevice.Device.Children[0], RSR2_AM_4_Group_Helper.Create()));
			Assert.IsTrue(selectedDevice.Device.Children.Max(x => x.IntAddress) == 255);
			Assert.IsTrue(selectedDevice.Device.Children.Count == 255);
		}


		void SetMokForNewDeviceViewModel(GKDriverType drivertype, int count = 1, bool addInStartList = false)
		{
			MockDialogService.OnShowModal += x =>
			{
				var newDeviceViewModel = x as NewDeviceViewModel;
				newDeviceViewModel.SelectedDriver = GKManager.Drivers.FirstOrDefault(y => y.DriverType == drivertype);
				newDeviceViewModel.Count = count;
				newDeviceViewModel.AddInStartlList = addInStartList;
				newDeviceViewModel.SaveCommand.Execute();

			};
		}

		[Test]
		public void AddZone()
		{
			MockDialogService.OnShowModal += x =>
			{
				var zoneDetailsViewModel = x as ZoneDetailsViewModel;
				zoneDetailsViewModel.Name = "Test zone";
				zoneDetailsViewModel.SaveCommand.Execute();
			};
			var zonesViewModel = GroupControllerModule.ZonesViewModel;
			zonesViewModel.OnShow();
			zonesViewModel.AddCommand.Execute();
			Assert.IsTrue(zonesViewModel.Zones.Count == 1);
		}

		[Test]
		public void AddDirection()
		{
			MockDialogService.OnShowModal += x =>
			{
				var directionDetailsViewModel = x as DirectionDetailsViewModel;
				directionDetailsViewModel.Name = "Test direction";
				directionDetailsViewModel.SaveCommand.Execute();
			};
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			directionsViewModel.AddCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
		}

		[Test]
		public void RemoveZone()
		{
			GKManager.Zones.Add(new GKZone());
			CreateGroupControllerModule();
			MockMessageBoxService.ShowConfirmationResult = true;

			var zonesViewModel = GroupControllerModule.ZonesViewModel;
			zonesViewModel.OnShow();
			zonesViewModel.DeleteCommand.Execute();
			Assert.IsTrue(zonesViewModel.Zones.Count == 0);
		}

		[Test]
		public void SetDirectionLogic()
		{
			GKManager.Directions.Add(new GKDirection());
			CreateGroupControllerModule();

			MockDialogService.OnShowModal += x =>
			{
				var logicViewModel = x as LogicViewModel;
				if (logicViewModel != null)
				{
					logicViewModel.OnClausesGroup.Clauses[0].SelectedClauseOperationType = ClauseOperationType.AnyDevice;
					logicViewModel.OnClausesGroup.Clauses[0].SelectedStateType = logicViewModel.OnClausesGroup.Clauses[0].StateTypes.FirstOrDefault(y => y.StateBit == GKStateBit.Failure);
					logicViewModel.OnClausesGroup.Clauses[0].SelectDevicesCommand.Execute();
					logicViewModel.SaveCommand.Execute();
				}
			};
			MockDialogService.OnShowModal += x =>
			{
				var devicesSelectationViewModel = x as DevicesSelectationViewModel;
				if (devicesSelectationViewModel != null)
				{
					devicesSelectationViewModel.AddCommand.Execute(devicesSelectationViewModel.AvailableDevices.Where(y => y.DriverType == GKDriverType.RSR2_KAU).ToList());
					devicesSelectationViewModel.SaveCommand.Execute();
				}
			};
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			directionsViewModel.SelectedDirection.ShowLogicCommand.Execute();
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Logic.OnClausesGroup.Clauses.Count == 1);
			var presentationLogic = directionsViewModel.SelectedDirection.PresentationLogic;
		}
	}
}