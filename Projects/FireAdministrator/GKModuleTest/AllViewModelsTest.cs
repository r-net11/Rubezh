﻿using System;
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
	public class AllViewModelsTest
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
			gkDevice1 = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice11 = GKManager.AddChild(gkDevice1, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice12 = GKManager.AddChild(gkDevice1, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);

			gkDevice2 = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice21 = GKManager.AddChild(gkDevice2, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice22 = GKManager.AddChild(gkDevice2, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);

			GKManager.UpdateConfiguration();
			ClientManager.PlansConfiguration = new PlansConfiguration();
			ClientManager.PlansConfiguration.AllPlans = new List<Plan>();

			ServiceFactory.Initialize(null, null);
			ServiceFactory.ResourceService = new MockResourceService();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			ServiceFactory.MessageBoxService = MockMessageBoxService = new MockMessageBoxService();
			ServiceFactory.MenuService = new MenuService(x => { ;});
			ServiceFactory.RibbonService = new MockRibbonService();

			var mockRepository = new MockRepository();
			mockRepository.ReplayAll();

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
			SetMokForAdddevice(GKDriverType.RSR2_AM_1);
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
		public void AddDevicePositiveValueForRSR()
		{
			SetMokForAdddevice(GKDriverType.RSR2_AM_1, 255);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 255);
		}

		[Test]
		public void AddDeviceNegativeValueForRSR2()
		{
			SetMokForAdddevice(GKDriverType.RSR2_AM_1, 256);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 0);
		}

		[Test]
		public void AddDevicePositiveValueForGK()
		{
			SetMokForAdddevice(GKDriverType.RSR2_GKMirror,123);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 127);
		}

		[Test]
		public void AddDeviceNegativeValueForGK()
		{
			SetMokForAdddevice(GKDriverType.RSR2_GKMirror, 128);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.Count() == 0);
		}


		[Test]
		public void AddGroupDevice()
		{
			SetMokForAdddevice(GKDriverType.RSR2_AM_4);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Children.Count() == 1);
			Assert.IsTrue(selectedDevice.Device.Children[0].DriverType == GKDriverType.RSR2_AM_4);
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.All(x => x.DriverType == GKDriverType.RSR2_AM_1));
			Assert.IsTrue(selectedDevice.Device.Children.All(x => x.AllParents.Count == 4));
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.All(x => x.AllParents.Count == 5));
			Assert.IsTrue(selectedDevice.Device.Children[0].Children.All(x =>selectedDevice.Children.All(y => y.Device == x.Parent)));
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
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.AllParents.Count() == 1 );
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.Parent == selectedDevice.Device);
		}

		[Test]
		public void AddGroupDeviceOPSZ()
		{
			SetMokForAdddevice(GKDriverType.RSR2_OPSZ);
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
			SetMokForAdddevice(GKDriverType.RSR2_KAU);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			devicesViewModel.OnShow();
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			int count = selectedDevice.Device.Children.Count();
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.Count() > count);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Count() == 9);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Where(x=> x.Driver.DriverType == GKDriverType.KAUIndicator).Count() == 1);
			Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Where(x=> x.Driver.DriverType == GKDriverType.RSR2_KAU_Shleif ).Count() == 8);
		}
		[Test]
		public void AddMirror()
		{
			SetMokForAdddevice(GKDriverType.RSR2_GKMirror);
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
			SetMokForAdddevice(GKDriverType.RSR2_GKMirror);
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			devicesViewModel.OnShow();
			var selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.GK);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			MockDialogService.OnShowModal -= x =>
			{
				var newDeviceViewModel = x as NewDeviceViewModel;
				newDeviceViewModel.SelectedDriver = GKManager.Drivers.FirstOrDefault(y => y.DriverType == GKDriverType.RSR2_GKMirror);
				newDeviceViewModel.SaveCommand.Execute();
			};
			SetMokForAdddevice(GKDriverType.RSR2_GKMirrorFireZone);
			devicesViewModel.OnShow();
			selectedDevice = devicesViewModel.SelectedDevice = devicesViewModel.AllDevices.FirstOrDefault(x => x.Driver.DriverType == GKDriverType.RSR2_GKMirror);
			devicesViewModel.SelectedDevice.AddCommand.Execute();
			Assert.IsTrue(selectedDevice.Device.Children.Where(x=> x.Driver.HasMirror == true).Count() == 1);
			//Assert.IsTrue(selectedDevice.Children.LastOrDefault().Children.Count() == 2);
			//Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.Children[0].DriverType == GKDriverType.GKIndicatorsGroup);
			//Assert.IsTrue(selectedDevice.Children.LastOrDefault().Device.Children[1].DriverType == GKDriverType.GKRelaysGroup);
			//Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[0].Children.All(x => x.DriverType == GKDriverType.GKIndicator));
			//Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[1].Children.All(x => x.DriverType == GKDriverType.GKRele));
			//Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[0].Children.Count == 16);
			//Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().Children[1].Children.Count == 5);
			//Assert.IsTrue(selectedDevice.Device.Children.LastOrDefault().IntAddress - selectedDevice.Device.Children[count - 1].IntAddress == 1);
		}

		void SetMokForAdddevice(GKDriverType drivertype, int count = 1)
		{
			MockDialogService.OnShowModal += x =>
			{
				var newDeviceViewModel = x as NewDeviceViewModel;
				newDeviceViewModel.SelectedDriver = GKManager.Drivers.FirstOrDefault(y => y.DriverType == drivertype);
				newDeviceViewModel.Count = count;
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