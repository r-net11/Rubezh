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
	public class DirectionTest
	{
		GKDevice gkDevice1;
		GKDevice kauDevice11;

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
			GKManager.UpdateConfiguration();
			GroupControllerModule = new GroupControllerModule();
			GroupControllerModule.CreateViewModels();
			GroupControllerModule.Initialize();
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
		public void AddIdenticalDirectionTest()
		{
			var direction = new GKDirection()
			{
				No = 1,
				Name = "Test 1 Direction"
			};
			GKManager.AddDirection(direction);
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
		public void CheckExistingDirectionTest()
		{
			var direction = new GKDirection()
			{
				Name = "Test 2 Direction"
			};
			GKManager.AddDirection(direction);
			CreateGroupControllerModule();
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test 2 Direction");
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
			GKManager.AddDirection(direction);
			CreateGroupControllerModule();

			MockDialogService.OnShowModal += x =>
			{
				(x as DirectionDetailsViewModel).Name = "Test Edit Direction Properties";
				(x as DirectionDetailsViewModel).Description = "Удалено";
				(x as DirectionDetailsViewModel).No = 2;
				(x as DirectionDetailsViewModel).Delay = 1;
				(x as DirectionDetailsViewModel).Hold = 2;
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
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Delay == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Hold == 2);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.DelayRegime == DelayRegime.On);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Description == "Удалено");
		}

		[Test]
		public void CopyDirectionTest()
		{
			var direction = new GKDirection()
			{
				No = 3,
			};
			GKManager.AddDirection(direction);
			CreateGroupControllerModule();

			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.No == 3);
			directionsViewModel.CopyCommand.Execute();
			directionsViewModel.PasteCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 2);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.No == 4);
		}

		[Test]
		public void DeleteDirectionTest()
		{
			MockMessageBoxService.ShowConfirmationResult = true;
			var direction = new GKDirection();
			GKManager.AddDirection(direction);
			CreateGroupControllerModule();
			var directionsViewModel = GroupControllerModule.DirectionsViewModel;
			directionsViewModel.OnShow();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
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
			var direction_1 = new GKDirection()
			{
				Name = "Test1Direction"
			};
			var direction_2 = new GKDirection()
			{
				Name = "Test2Direction"
			};
			GKManager.AddDirection(direction);
			GKManager.AddDirection(direction_1);
			GKManager.AddDirection(direction_2);
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
			directionsViewModel.DeleteAllEmptyCommand.Execute();
			Assert.IsTrue(directionsViewModel.Directions.Count == 1);
			Assert.IsTrue(directionsViewModel.SelectedDirection.Direction.Name == "Test2Direction");
		}
	}
}