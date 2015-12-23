using GKModule.ViewModels;
using NUnit.Framework;
using RubezhAPI.GK;
using System.Linq;
using RubezhAPI;
using GKProcessor;
using Infrastructure;
using GKModule;
using Infrastructure.Services;
using RubezhClient;

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
		[SetUp]
		public void Initialize()
		{
			GKManager.DeviceLibraryConfiguration = new GKDeviceLibraryConfiguration();
			GKManager.DeviceConfiguration = CreateConfiguration();

			ServiceFactory.Initialize(null, null);
			ServiceFactory.ResourceService = new MockResourceService();
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			ServiceFactory.MessageBoxService = MockMessageBoxService = new MockMessageBoxService();
			ServiceFactory.MenuService = new MenuService(x => {; });
			ServiceFactory.RibbonService = new MockRibbonService();

			CreateGroupControllerModule();
		}
		[Test]
		public void AddMptDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateAm1();
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			GKManager.UpdateConfiguration();
			var mptViewModel = new MPTViewModel(mpt);
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType = new MPTDeviceTypeViewModel(GKMPTDeviceType.HandStart);
				mptDeviceSelectationViewModel.SelectedDevice =
					mptDeviceSelectationViewModel.Devices.FirstOrDefault(device => device.UID == am1Device.UID);
				mptDeviceSelectationViewModel.SaveCommand.Execute();
			};
			mptViewModel.AddCommand.Execute();
			Assert.IsTrue(mpt.MPTDevices.FirstOrDefault(x=>x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart) != null);
			Assert.IsTrue(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1 && mpt.InputDependentElements.Contains(am1Device));
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 1 && am1Device.OutputDependentElements.Contains(mpt));
		}
		[Test]
		public void DeleteMptDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateAm1();
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			GKManager.UpdateConfiguration();
			var mptViewModel = new MPTViewModel(mpt);
			mptViewModel.DeleteCommand.Execute();
			Assert.IsTrue(mpt.MPTDevices.FirstOrDefault(x => x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart) == null);
			Assert.IsFalse(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 0);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 0);
		}
		[Test]
		public void EditMptDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateAm1();
			var rm1Device = CreateRm1();
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			GKManager.UpdateConfiguration();
			var mptViewModel = new MPTViewModel(mpt) { SelectedDevice = new MPTDeviceViewModel(mptDevice) };
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType = new MPTDeviceTypeViewModel(GKMPTDeviceType.DoNotEnterBoard);
				mptDeviceSelectationViewModel.SelectedDevice =
					mptDeviceSelectationViewModel.Devices.FirstOrDefault(device => device.UID == rm1Device.UID);
                mptDeviceSelectationViewModel.SaveCommand.Execute();
			};
			mptViewModel.EditCommand.Execute();
			Assert.IsTrue(mpt.MPTDevices.FirstOrDefault(x => x.Device == rm1Device && x.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard) != null);
			Assert.IsFalse(am1Device.IsInMPT);
			Assert.IsTrue(rm1Device.IsInMPT);
			Assert.IsFalse(mpt.InputDependentElements.Contains(am1Device));
			Assert.IsTrue(mpt.InputDependentElements.Contains(rm1Device));
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 0);
			Assert.IsTrue(rm1Device.OutputDependentElements.Count == 1 && rm1Device.OutputDependentElements.Contains(mpt));
		}
		[Test]
		public void AddMptDeviceFewMptsTest()
		{
			var am1Device1 = CreateAm1();
			var am1Device2 = CreateAm1();
			var mpt1 = new GKMPT();
			var mpt2 = new GKMPT();
			var mptDevice1 = new GKMPTDevice { Device = am1Device1, DeviceUID = am1Device1.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt1.MPTDevices.Add(mptDevice1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt2);
			GKManager.UpdateConfiguration();
			var mptViewModel = new MPTViewModel(mpt2);
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType = new MPTDeviceTypeViewModel(GKMPTDeviceType.HandStart);
				Assert.IsTrue(mptDeviceSelectationViewModel.Devices.Count == 1 
					&& mptDeviceSelectationViewModel.Devices.Contains(am1Device2));
			};
			mptViewModel.AddCommand.Execute();
		}
		[Test]
		public void EditMptDeviceFewMptsTest()
		{
			var am1Device1 = CreateAm1();
			var am1Device2 = CreateAm1();
			var am1Device3 = CreateAm1();
			var mpt1 = new GKMPT();
			var mpt2 = new GKMPT();
			var mptDevice1 = new GKMPTDevice { Device = am1Device1, DeviceUID = am1Device1.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			var mptDevice2 = new GKMPTDevice { Device = am1Device2, DeviceUID = am1Device2.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt1.MPTDevices.Add(mptDevice1);
			mpt2.MPTDevices.Add(mptDevice2);
			GKManager.DeviceConfiguration.MPTs.Add(mpt1);
			GKManager.DeviceConfiguration.MPTs.Add(mpt2);
			GKManager.UpdateConfiguration();
			var mptViewModel1 = new MPTViewModel(mpt1);
			var mptViewModel2 = new MPTViewModel(mpt2);
			MockDialogService.OnShowModal += x =>
			{
				var mptDeviceSelectationViewModel = x as MPTDeviceSelectationViewModel;
				mptDeviceSelectationViewModel.SelectedMPTDeviceType = new MPTDeviceTypeViewModel(GKMPTDeviceType.HandStart);
				Assert.IsTrue(mptDeviceSelectationViewModel.Devices.Count == 2
					&& mptDeviceSelectationViewModel.Devices.Contains(am1Device1)
					&& mptDeviceSelectationViewModel.Devices.Contains(am1Device3));
			};
			mptViewModel1.EditCommand.Execute();
		}
		[Test]
		public void RemoveMptTest()
		{
			MockMessageBoxService.ShowConfirmationResult = true;
			var mpt = new GKMPT();
			var am1Device = CreateAm1();
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			GKManager.UpdateConfiguration();
			Assert.IsTrue(mpt.MPTDevices.FirstOrDefault(x => x.Device == am1Device && x.MPTDeviceType == GKMPTDeviceType.HandStart) != null);
			Assert.IsTrue(am1Device.IsInMPT);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1);
			Assert.IsTrue(am1Device.OutputDependentElements.Count == 1);
			var mptsViewModel = new MPTsViewModel();
			mptsViewModel.Initialize();
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
			var am1Device = CreateAm1();
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			GKManager.UpdateConfiguration();
			ClientManager.PlansConfiguration = new RubezhAPI.Models.PlansConfiguration();
			var devicesViewModel = new DevicesViewModel();
			devicesViewModel.Initialize();
			var deviceViewModel = devicesViewModel.AllDevices.FirstOrDefault(x => x.Device == am1Device);

			Assert.IsTrue(mpt.MPTDevices.Count == 1);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1 && mpt.InputDependentElements.Contains(am1Device));
			deviceViewModel.RemoveCommand.Execute();
			Assert.IsTrue(mpt.MPTDevices.Count == 0);
			Assert.IsTrue(mpt.InputDependentElements.Count == 0);
		}
		public void ChangeDriverDeviceTest()
		{
			var mpt = new GKMPT();
			var am1Device = CreateAm1();
			var mptDevice = new GKMPTDevice { Device = am1Device, DeviceUID = am1Device.UID, MPTDeviceType = GKMPTDeviceType.HandStart };
			mpt.MPTDevices.Add(mptDevice);
			GKManager.DeviceConfiguration.MPTs.Add(mpt);
			GKManager.UpdateConfiguration();
			ClientManager.PlansConfiguration = new RubezhAPI.Models.PlansConfiguration();
			var devicesViewModel = new DevicesViewModel();
			devicesViewModel.Initialize();
			var deviceViewModel = devicesViewModel.AllDevices.FirstOrDefault(x => x.Device == am1Device);

			Assert.IsTrue(mpt.MPTDevices.Count == 1);
			Assert.IsTrue(mpt.InputDependentElements.Count == 1 && mpt.InputDependentElements.Contains(am1Device));
			var cardReaderDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_CardReader);
			deviceViewModel.Driver = cardReaderDriver;
            Assert.IsTrue(mpt.MPTDevices.Count == 0);
			Assert.IsTrue(mpt.InputDependentElements.Count == 0);
		}
		GKDevice CreateAm1()
		{
			var am1Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_AM_1);
			var am1Device = new GKDevice { DriverUID = am1Driver.UID };
			AlsDevice.Children.Add(am1Device);
			return am1Device;
		}
		GKDevice CreateCardReader()
		{
			var cardReaderDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_CardReader);
			var cardReaderDevice = new GKDevice { DriverUID = cardReaderDriver.UID };
			AlsDevice.Children.Add(cardReaderDevice);
			return cardReaderDevice;
		}
		GKDevice CreateRm1()
		{
			var rm1Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_RM_1);
			var rm1Device = new GKDevice { DriverUID = rm1Driver.UID };
			AlsDevice.Children.Add(rm1Device);
			return rm1Device;
		}

		void CreateGroupControllerModule()
		{
			GroupControllerModule = new GroupControllerModule();
			GroupControllerModule.CreateViewModels();
			GroupControllerModule.Initialize();
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

			AlsDevice = new GKDevice { DriverUID = alsDriver.UID };

			KauDevice = new GKDevice { DriverUID = kauDriver.UID };
			KauDevice.Children.Add(AlsDevice);

			GkDevice = new GKDevice { DriverUID = gkDriver.UID };
			GkDevice.Children.Add(KauDevice);

			newConfiguration.RootDevice = new GKDevice { DriverUID = systemDriver.UID };
			newConfiguration.RootDevice.Children.Add(GkDevice);
			newConfiguration.UpdateConfiguration();
			return newConfiguration;
		}
	}
}