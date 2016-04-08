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
	public class DelaysTest
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
			GKDriversCreator.Create();
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
		public void AddDelayTest()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as DelayDetailsViewModel).Name = "Test Delay";
				(x as DelayDetailsViewModel).SaveCommand.Execute();
			};
			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			Assert.IsTrue(delaysViewModel.Delays.Count == 0);
			Assert.IsTrue(delaysViewModel.SelectedDelay == null);
			delaysViewModel.AddCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Test Delay");
		}

		[Test]
		public void AddDelayPropertiesTest()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as DelayDetailsViewModel).Name = "Test Delay Properties";
				(x as DelayDetailsViewModel).Description = "Примечание";
				(x as DelayDetailsViewModel).DelayTime = 15;
				(x as DelayDetailsViewModel).Hold = 14;
				(x as DelayDetailsViewModel).DelayRegime = DelayRegime.Off;
				(x as DelayDetailsViewModel).SaveCommand.Execute();
			};
			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			Assert.IsTrue(delaysViewModel.Delays.Count == 0);
			Assert.IsTrue(delaysViewModel.SelectedDelay == null);
			delaysViewModel.AddCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Test Delay Properties");
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.DelayTime == 15);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Hold == 14);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.DelayRegime == DelayRegime.Off);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Description == "Примечание");
		}

		[Test]
		public void AddCancelDelayTest()
		{
			MockDialogService.OnShowModal += x =>
			{
				(x as DelayDetailsViewModel).CancelCommand.Execute();
			};
			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			Assert.IsTrue(delaysViewModel.Delays.Count == 0);
			Assert.IsTrue(delaysViewModel.SelectedDelay == null);
			delaysViewModel.AddCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 0);
			Assert.IsTrue(delaysViewModel.SelectedDelay == null);
		}

		[Test]
		public void AddIdenticalDelayTest()
		{
			var delay = new GKDelay()
			{
				No = 1,
				Name = "Test 1 Delay"
			};
			GKManager.AddDelay(delay);
			CreateGroupControllerModule();
			MockDialogService.OnShowModal += x =>
			{
				(x as DelayDetailsViewModel).No = 1;
				(x as DelayDetailsViewModel).Name = "Test Delay";
				(x as DelayDetailsViewModel).SaveCommand.Execute();
			};
			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name.CompareTo("Test 1 Delay") == 0);
			delaysViewModel.AddCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Test 1 Delay");
		}

		[Test]
		public void CheckExistingDelayTest()
		{
			var delay = new GKDelay()
			{
				Name = "Delay 1"
			};
			GKManager.AddDelay(delay);
			CreateGroupControllerModule();
			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Delay 1");
		}

		[Test]
		public void EditDelayPropertiesTest()
		{
			var delay = new GKDelay()
			{
				No = 1,
				Name = "Delay Properties",
				Description = "Примечание",
				DelayTime = 1,
				Hold = 2,
				DelayRegime = DelayRegime.Off,
			};
			GKManager.AddDelay(delay);
			CreateGroupControllerModule();

			MockDialogService.OnShowModal += x =>
			{
				(x as DelayDetailsViewModel).Name = "Edit Delay Properties";
				(x as DelayDetailsViewModel).Description = "Удалено";
				(x as DelayDetailsViewModel).No = 2;
				(x as DelayDetailsViewModel).DelayTime = 11;
				(x as DelayDetailsViewModel).Hold = 12;
				(x as DelayDetailsViewModel).DelayRegime = DelayRegime.On;
				(x as DelayDetailsViewModel).SaveCommand.Execute();
			};
			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.No == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Delay Properties");
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.DelayTime == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Hold == 2);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.DelayRegime == DelayRegime.Off);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Description == "Примечание");
			delaysViewModel.EditCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.No == 2);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Edit Delay Properties");
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.DelayTime == 11);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Hold == 12);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.DelayRegime == DelayRegime.On);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Description == "Удалено");
		}

		[Test]
		public void CopyDelayTest()
		{
			var delay = new GKDelay()
			{
				No = 3
			};
			GKManager.AddDelay(delay);
			CreateGroupControllerModule();

			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.No == 3);
			delaysViewModel.CopyCommand.Execute();
			delaysViewModel.PasteCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 2);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.No == 4);
		}

		[Test]
		public void DeleteDelayTest()
		{
			MockMessageBoxService.ShowConfirmationResult = true;
			var delay = new GKDelay();
			GKManager.AddDelay(delay);
			CreateGroupControllerModule();
			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			delaysViewModel.DeleteCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 0);
		}

		[Test]
		public void DeleteAllEmptyDelayTest()
		{
			MockMessageBoxService.ShowConfirmationResult = true;
			var delay1 = new GKDelay()
			{
				Name = "Delay1"
			};
			var delay2 = new GKDelay()
			{
				Name = "Delay2"
			};
			var delay3 = new GKDelay()
			{
				Name = "Delay3"
			};
			GKManager.AddDelay(delay1);
			GKManager.AddDelay(delay2);
			GKManager.AddDelay(delay3);
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

			var delaysViewModel = GroupControllerModule.DelaysViewModel;
			delaysViewModel.OnShow();
			delaysViewModel.SelectedDelay = delaysViewModel.Delays[2];
			Assert.IsTrue(delaysViewModel.Delays.Count == 3);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Delay3");
			delaysViewModel.SelectedDelay.ShowLogicCommand.Execute();
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Logic.OnClausesGroup.Clauses.Count == 1);
			delaysViewModel.DeleteAllEmptyCommand.Execute();
			Assert.IsTrue(delaysViewModel.Delays.Count == 1);
			Assert.IsTrue(delaysViewModel.SelectedDelay.Delay.Name == "Delay3");
		}
	}
}