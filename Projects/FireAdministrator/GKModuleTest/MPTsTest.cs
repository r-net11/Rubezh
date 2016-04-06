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
using System.Linq;

namespace GKModuleTest
{
	[TestFixture]
	public class MPTsTest
	{
		GKDevice GkDevice;
		GKDevice KauDevice;
		GKDevice AlsDevice;
		GroupControllerModule GroupControllerModule;
		MockDialogService MockDialogService;
		MockMessageBoxService MockMessageBoxService;
		GKDevice CreateDevice(GKDriverType deviceDriverType)
		{
			var deviceDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == deviceDriverType);
			var device = new GKDevice { DriverUID = deviceDriver.UID };
			AlsDevice.Children.Add(device);
			return device;
		}
		void CreateGroupControllerModule()
		{
			GKManager.UpdateConfiguration();
			GroupControllerModule = new GroupControllerModule();
			GroupControllerModule.CreateViewModels();
			GroupControllerModule.Initialize();
		}
		void CreateConfiguration()
		{
			GKManager.DeviceLibraryConfiguration = new GKDeviceLibraryConfiguration();
			GKManager.DeviceConfiguration = new GKDeviceConfiguration();
			GKDriversCreator.Create();
			var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			Assert.IsNotNull(systemDriver);
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice { Driver = systemDriver, DriverUID = systemDriver.UID };
			GkDevice = GKManager.AddDevice(systemDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			KauDevice = GKManager.AddDevice(GkDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			AlsDevice = GKManager.AddDevice(KauDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif), 1);

			GKManager.UpdateConfiguration();
			ClientManager.PlansConfiguration = new PlansConfiguration();
		}
		[SetUp]
		public void Initialize()
		{
			CreateConfiguration();
			ServiceFactory.Initialize(null, null);
			ServiceFactory.ResourceService = new MockResourceService();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			ServiceFactory.MessageBoxService = MockMessageBoxService = new MockMessageBoxService();
			ServiceFactory.MenuService = new MenuService(x => {; });
			ServiceFactory.RibbonService = new MockRibbonService();
		}
		[Test]
		public void AddMptDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateDevice(GKDriverType.RSR2_AM_1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			CreateGroupControllerModule();
			var mptViewModel = GroupControllerModule.MPTsViewModel.MPTs[0];
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType =
					mptDeviceSelectationViewModel.AvailableMPTDeviceTypes.FirstOrDefault(type => type.MPTDeviceType == GKMPTDeviceType.HandStart);
				mptDeviceSelectationViewModel.SelectedDevice =
					mptDeviceSelectationViewModel.Devices.FirstOrDefault(device => device.UID == am1Device.UID);
				mptDeviceSelectationViewModel.SaveCommand.Execute();
			};
			mptViewModel.AddCommand.Execute();
			Assert.IsTrue(mpt.MPTDevices.Any(x => x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart));
			Assert.IsTrue(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1 && mpt.InputDependentElements[0] == am1Device);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 1 && am1Device.OutputDependentElements[0] == mpt);
		}
		[Test]
		public void AddCancelMptDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateDevice(GKDriverType.RSR2_AM_1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			CreateGroupControllerModule();
			var mptViewModel = GroupControllerModule.MPTsViewModel.MPTs[0];
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType =
					mptDeviceSelectationViewModel.AvailableMPTDeviceTypes.FirstOrDefault(type => type.MPTDeviceType == GKMPTDeviceType.HandStart);
				mptDeviceSelectationViewModel.SelectedDevice =
					mptDeviceSelectationViewModel.Devices.FirstOrDefault(device => device.UID == am1Device.UID);
				mptDeviceSelectationViewModel.CancelCommand.Execute();
			};
			mptViewModel.AddCommand.Execute();
			Assert.IsFalse(mpt.MPTDevices.Any(x => x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart));
			Assert.IsFalse(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 0);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 0);
		}
		[Test]
		public void DeleteMptDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateDevice(GKDriverType.RSR2_AM_1);
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			CreateGroupControllerModule();
			var mptViewModel = GroupControllerModule.MPTsViewModel.MPTs[0];
			Assert.IsTrue(mpt.MPTDevices.Any(x => x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart));
			Assert.IsTrue(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 1);

			mptViewModel.DeleteCommand.Execute();
			Assert.IsFalse(mpt.MPTDevices.Any(x => x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart));
			Assert.IsFalse(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 0);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 0);
		}
		[Test]
		public void EditMptDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateDevice(GKDriverType.RSR2_AM_1);
			var rm1Device = CreateDevice(GKDriverType.RSR2_RM_1);
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			CreateGroupControllerModule();
			var mptViewModel = GroupControllerModule.MPTsViewModel.MPTs[0];
			mptViewModel.SelectedDevice = mptViewModel.Devices.FirstOrDefault(x => x.MPTDevice == mptDevice);
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType =
					mptDeviceSelectationViewModel.AvailableMPTDeviceTypes.FirstOrDefault(type => type.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard);
				mptDeviceSelectationViewModel.SelectedDevice =
					mptDeviceSelectationViewModel.Devices.FirstOrDefault(device => device.UID == rm1Device.UID);
				mptDeviceSelectationViewModel.SaveCommand.Execute();
			};
			mptViewModel.EditCommand.Execute();
			Assert.IsTrue(mpt.MPTDevices.Any(x => x.Device == rm1Device && x.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard));
			Assert.IsFalse(am1Device.IsInMPT);
			Assert.IsTrue(rm1Device.IsInMPT);
			Assert.IsFalse(mpt.InputDependentElements.Contains(am1Device));
			Assert.IsTrue(mpt.InputDependentElements.Contains(rm1Device));
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 0);
			Assert.IsTrue(rm1Device.OutputDependentElements.Count == 1 && rm1Device.OutputDependentElements[0] == mpt);
			Assert.IsTrue(mptViewModel.Devices.Count == 1 && mptViewModel.Devices[0].MPTDevice.Device.DriverType == GKDriverType.RSR2_RM_1);
		}
		[Test]
		public void AddMptDeviceFewMptsTest()
		{
			var am1Device1 = CreateDevice(GKDriverType.RSR2_AM_1);
			var am1Device2 = CreateDevice(GKDriverType.RSR2_AM_1);
			var mpt1 = new GKMPT();
			var mpt2 = new GKMPT();
			var mptDevice1 = new GKMPTDevice { Device = am1Device1, DeviceUID = am1Device1.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt1.MPTDevices.Add(mptDevice1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt2);
			CreateGroupControllerModule();
			var mptViewModel = GroupControllerModule.MPTsViewModel.MPTs[1];
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType =
					mptDeviceSelectationViewModel.AvailableMPTDeviceTypes.FirstOrDefault(type => type.MPTDeviceType == GKMPTDeviceType.HandStart);
				Assert.IsTrue(mptDeviceSelectationViewModel.Devices.Count == 1
					&& mptDeviceSelectationViewModel.Devices[0] == am1Device2);
			};
			mptViewModel.AddCommand.Execute();
		}
		[Test]
		public void EditMptDeviceFewMptsTest()
		{
			var am1Device1 = CreateDevice(GKDriverType.RSR2_AM_1);
			var am1Device2 = CreateDevice(GKDriverType.RSR2_AM_1);
			var am1Device3 = CreateDevice(GKDriverType.RSR2_AM_1);
			var mpt1 = new GKMPT();
			var mpt2 = new GKMPT();
			var mptDevice1 = new GKMPTDevice { Device = am1Device1, DeviceUID = am1Device1.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			var mptDevice2 = new GKMPTDevice { Device = am1Device2, DeviceUID = am1Device2.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt1.MPTDevices.Add(mptDevice1);
			mpt2.MPTDevices.Add(mptDevice2);
			GKManager.DeviceConfiguration.MPTs.Add(mpt1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt2);
			CreateGroupControllerModule();
			var mptViewModel1 = GroupControllerModule.MPTsViewModel.MPTs[0];
			var mptViewModel2 = GroupControllerModule.MPTsViewModel.MPTs[1];
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType =
					mptDeviceSelectationViewModel.AvailableMPTDeviceTypes.FirstOrDefault(type => type.MPTDeviceType == GKMPTDeviceType.HandStart);
				Assert.IsTrue(mptDeviceSelectationViewModel.Devices.Count == 2
					&& mptDeviceSelectationViewModel.Devices[0] == am1Device1
					&& mptDeviceSelectationViewModel.Devices[1] == am1Device3);
				Assert.IsTrue(mptViewModel1.Devices.Count == 1 && mptViewModel1.Devices[0].MPTDevice.Device.DriverType == GKDriverType.RSR2_AM_1);
			};
			mptViewModel1.EditCommand.Execute();
		}
		[Test]
		public void RemoveMptTest()
		{
			MockMessageBoxService.ShowConfirmationResult = true;
			var mpt = new GKMPT();
			var am1Device = CreateDevice(GKDriverType.RSR2_AM_1);
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			CreateGroupControllerModule();
			Assert.IsTrue(mpt.MPTDevices.Any(x => x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart));
			Assert.IsTrue(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 1);
			var mptsViewModel = GroupControllerModule.MPTsViewModel;
			mptsViewModel.SelectedMPT = mptsViewModel.MPTs.FirstOrDefault(x => x.MPT == mpt);
			mptsViewModel.DeleteCommand.Execute();

			Assert.IsTrue(GKManager.DeviceConfiguration.MPTs.Count == 0);
			Assert.IsFalse(am1Device.IsInMPT);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 0);
		}
		[Test]
		public void RemoveDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateDevice(GKDriverType.RSR2_AM_1);
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			CreateGroupControllerModule();
			var devicesViewModel = GroupControllerModule.DevicesViewModel;
			var deviceViewModel = devicesViewModel.AllDevices.FirstOrDefault(x => x.Device == am1Device);
			var mptViewModel = GroupControllerModule.MPTsViewModel.MPTs[0];
			Assert.IsTrue(mpt.MPTDevices.Count == 1);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1 && mpt.InputDependentElements[0] == am1Device);
			Assert.IsTrue(mptViewModel.Devices.Count == 1 && mptViewModel.Devices[0].MPTDevice == mptDevice);

			deviceViewModel.RemoveCommand.Execute();
			Assert.IsTrue(mpt.MPTDevices.Count == 0);
			Assert.IsTrue(mpt.InputDependentElements.Count == 0);
			Assert.IsTrue(mptViewModel.Devices.Count == 0);
		}
		[Test]
		public void ChangeDriverDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateDevice(GKDriverType.RSR2_AM_1);
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			CreateGroupControllerModule();
			ClientManager.PlansConfiguration = new RubezhAPI.Models.PlansConfiguration();
			var devicesViewModel = new DevicesViewModel();
			devicesViewModel.Initialize();
			var deviceViewModel = devicesViewModel.AllDevices.FirstOrDefault(x => x.Device == am1Device);
			var mptViewModel = GroupControllerModule.MPTsViewModel.MPTs[0];
			Assert.IsTrue(mpt.MPTDevices.Count == 1);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1 && mpt.InputDependentElements[0] == am1Device);
			Assert.IsTrue(mptViewModel.Devices.Count == 1 && mptViewModel.Devices[0].MPTDevice.Device.DriverType == GKDriverType.RSR2_AM_1);

			var cardReaderDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_CardReader);
			deviceViewModel.Driver = cardReaderDriver;
			Assert.IsTrue(mpt.MPTDevices.Count == 0);
			Assert.IsTrue(mpt.InputDependentElements.Count == 0);
			Assert.IsTrue(mptViewModel.Devices.Count == 0);
		}
	}
}