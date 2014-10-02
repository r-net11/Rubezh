using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DoorViewModel : BaseViewModel
	{
		public GKDoor Door { get; set; }

		public DoorViewModel(GKDoor door)
		{
			Door = door;
			ChangeEnterDeviceCommand = new RelayCommand(OnChangeEnterDevice);
			ChangeExitDeviceCommand = new RelayCommand(OnChangeExitDevice);
			ChangeLockDeviceCommand = new RelayCommand(OnChangeLockDevice);
			ChangeLockControlDeviceCommand = new RelayCommand(OnChangeLockControlDevice);
			Update();
		}

		public string Name
		{
			get { return Door.Name; }
			set
			{
				Door.Name = value;
				Door.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Description
		{
			get { return Door.Description; }
			set
			{
				Door.Description = value;
				Door.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void Update(GKDoor door)
		{
			Door = door;
			OnPropertyChanged(() => Door);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}
		public void Update()
		{
			EnterDevice = GKManager.Devices.FirstOrDefault(x=>x.UID == Door.EnterDeviceUID);
			ExitDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.ExitDeviceUID);
			LockDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockDeviceUID);
			LockControlDevice = GKManager.Devices.FirstOrDefault(x => x.UID == Door.LockControlDeviceUID);

			if (ExitDevice != null)
			{
				if (Door.DoorType == GKDoorType.OneWay && ExitDevice.DriverType != GKDriverType.AM_1)
				{
					Door.ExitDeviceUID = Guid.Empty;
					ExitDevice = null;
				}
				if (Door.DoorType == GKDoorType.TwoWay && ExitDevice.DriverType != GKDriverType.RSR2_CodeReader)
				{
					Door.ExitDeviceUID = Guid.Empty;
					ExitDevice = null;
				}
			}


			OnPropertyChanged(() => EnterDevice);
			OnPropertyChanged(() => ExitDevice);
			OnPropertyChanged(() => LockDevice);
			OnPropertyChanged(() => LockControlDevice);
		}

		public GKDevice EnterDevice { get; private set; }
		public GKDevice ExitDevice { get; private set; }
		public GKDevice LockDevice { get; private set; }
		public GKDevice LockControlDevice { get; private set; }

		public RelayCommand ChangeEnterDeviceCommand { get; private set; }
		void OnChangeEnterDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(EnterDevice, GKManager.Devices.Where(x=>x.DriverType == GKDriverType.RSR2_CodeReader));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.EnterDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeExitDeviceCommand { get; private set; }
		void OnChangeExitDevice()
		{
			var driverType = Door.DoorType == GKDoorType.OneWay ? GKDriverType.AM_1 : GKDriverType.RSR2_CodeReader;
			var deviceSelectationViewModel = new DeviceSelectationViewModel(ExitDevice, GKManager.Devices.Where(x => x.DriverType == driverType));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.ExitDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeLockDeviceCommand { get; private set; }
		void OnChangeLockDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockDevice, GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_RM_1 || x.DriverType == GKDriverType.RSR2_MVK8));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.LockDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangeLockControlDeviceCommand { get; private set; }
		void OnChangeLockControlDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockDevice, GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_AM_1));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.LockControlDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}