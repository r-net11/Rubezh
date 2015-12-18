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
	public class DirectionTest
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
		public void CreateConfiguration()
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

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddDevice(device.Children[1], GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}

		[Test]
		public void AddDirectionTest()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as DirectionDetailsViewModel).Name = "Test Direction";
				(x as DirectionDetailsViewModel).SaveCommand.Execute();

			};
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 0);
			Assert.IsTrue(directionsViewModel.SelectedDirection == null);
			directionsViewModel.AddCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test Direction");
		}

		[Test]
		public void AddCancelDirectionTest()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as DirectionDetailsViewModel).CancelCommand.Execute();

			};
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 0);
			Assert.IsTrue(directionsViewModel.SelectedDirection == null);
			directionsViewModel.AddCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 0);
			Assert.IsTrue(directionsViewModel.SelectedDirection == null);
		}

		[Test]
		public void CheckExistingDirectionTest()
		{
			var direction = new GKDirection()
			{
				Name = "Test 2 Direction"
			};
			GKManager.Directions.Add(direction);
			CreateGroupControllerModule();
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test 2 Direction");
		}

		[Test]
		public void DeleteDirectionTest()
		{
			MockMessageBoxService.ShowConfirmationResult = true;
			var direction = new GKDirection()
			{
				Name = "Test 2 Direction"
			};
			GKManager.Directions.Add(direction);
			CreateGroupControllerModule();
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			directionsViewModel.DeleteCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 0);
		}

		[Test]
		public void DeleteAllEmptyDirectionTest()
		{
			MockMessageBoxService.ShowConfirmationResult = true;
			var direction = new GKDirection()
			{
				Name = "Test Direction"
			};
			GKManager.Directions.Add(direction);
			var direction_1 = new GKDirection()
			{
				Name = "Test1Direction"
			};
			GKManager.Directions.Add(direction_1);
			var direction_2 = new GKDirection()
			{
				Name = "Test2Direction"
			};
			GKManager.Directions.Add(direction_2);
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
			directionsViewModel.SelectedDirection = directionsViewModel.Directions[2];
			Assert.IsTrue(directionsViewModel.Directions.Count == 3);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test2Direction");
			directionsViewModel.SelectedDirection.ShowLogicCommand.Execute();
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Logic.OnClausesGroup.Clauses.Count == 1);
			var presentationLogic = directionsViewModel.SelectedDirection.PresentationLogic;
			directionsViewModel.DeleteAllEmptyCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
		}

		[Test]
		public void AddIdenticalDirectionTest()
		{
			var direction = new GKDirection()
			{
				No = 1,
				Name = "Test 1 Direction"
			};
			GKManager.Directions.Add(direction);
			CreateGroupControllerModule();
			MockDialogService.OnShowModal += x =>
			{
				(x as DirectionDetailsViewModel).No = 1;
				(x as DirectionDetailsViewModel).Name = "Test Direction";
				(x as DirectionDetailsViewModel).SaveCommand.Execute();

			};
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name.CompareTo("Test 1 Direction") == 0);
			directionsViewModel.AddCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test 1 Direction");
		}

		[Test]
		public void AddDirectionPropertiesTest()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as DirectionDetailsViewModel).Name = "Test Direction Properties";
				(x as DirectionDetailsViewModel).Description = "Примечание";
				(x as DirectionDetailsViewModel).Delay = 15;
				(x as DirectionDetailsViewModel).Hold = 14;
				(x as DirectionDetailsViewModel).DelayRegime = DelayRegime.Off;
				(x as DirectionDetailsViewModel).SaveCommand.Execute();

			};

			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 0);
			Assert.IsTrue(directionsViewModel.SelectedDirection == null);
			directionsViewModel.AddCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test Direction Properties");
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Delay == 15);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Hold == 14);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.DelayRegime == DelayRegime.Off);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Description == "Примечание");

		}

		[Test]
		public void EditDirectionPropertiesTest()
		{
			var direction = new GKDirection()
			{
				No = 1,
				Name = "Test Direction Properties",
				Description = "Примечание",
				Delay = 15,
				Hold = 14,
				DelayRegime = DelayRegime.Off,
			};
			GKManager.Directions.Add(direction);
			CreateGroupControllerModule();

			MockDialogService.OnShowModal += x =>
			{
				(x as DirectionDetailsViewModel).Name = "Test Edit Direction Properties";
				(x as DirectionDetailsViewModel).Description = "Удалено";
				(x as DirectionDetailsViewModel).No = 2;
				(x as DirectionDetailsViewModel).Delay = 0;
				(x as DirectionDetailsViewModel).Hold = 0;
				(x as DirectionDetailsViewModel).DelayRegime = DelayRegime.On;
				(x as DirectionDetailsViewModel).SaveCommand.Execute();

			};
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.No == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test Direction Properties");
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Delay == 15);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Hold == 14);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.DelayRegime == DelayRegime.Off);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Description == "Примечание");
			directionsViewModel.EditCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.No == 2);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test Edit Direction Properties");
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Delay == 0);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Hold == 0);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.DelayRegime == DelayRegime.On);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Description == "Удалено");
		}

		[Test]
		public void CopyDirectionTest()
		{
			var direction = new GKDirection()
			{
				No = 3,
				Name = "Test Copy Direction",
			};
			GKManager.Directions.Add(direction);
			CreateGroupControllerModule();

			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.No == 3);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test Copy Direction");
			directionsViewModel.CopyCommand.Execute();
			directionsViewModel.PasteCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 2);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.No == 4);
		}
	}
}