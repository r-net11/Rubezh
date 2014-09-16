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
		public XDoor Door { get; set; }

		public DoorViewModel(XDoor door)
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

		public void Update(XDoor door)
		{
			Door = door;
			OnPropertyChanged(() => Door);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}
		public void Update()
		{
			EnterDevice = XManager.Devices.FirstOrDefault(x=>x.UID == Door.EnterDeviceUID);
			ExitDevice = XManager.Devices.FirstOrDefault(x => x.UID == Door.ExitDeviceUID);
			LockDevice = XManager.Devices.FirstOrDefault(x => x.UID == Door.LockDeviceUID);
			LockControlDevice = XManager.Devices.FirstOrDefault(x => x.UID == Door.LockControlDeviceUID);

			OnPropertyChanged(() => EnterDevice);
			OnPropertyChanged(() => ExitDevice);
			OnPropertyChanged(() => LockDevice);
			OnPropertyChanged(() => LockControlDevice);
		}

		public XDevice EnterDevice { get; private set; }
		public XDevice ExitDevice { get; private set; }
		public XDevice LockDevice { get; private set; }
		public XDevice LockControlDevice { get; private set; }

		public RelayCommand ChangeEnterDeviceCommand { get; private set; }
		void OnChangeEnterDevice()
		{
			var deviceSelectationViewModel = new DeviceSelectationViewModel(EnterDevice, XManager.Devices.Where(x=>x.DriverType == XDriverType.RSR2_CodeReader));
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
			var deviceSelectationViewModel = new DeviceSelectationViewModel(ExitDevice, XManager.Devices.Where(x => x.DriverType == XDriverType.RSR2_CodeReader));
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
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockDevice, XManager.Devices.Where(x => x.DriverType == XDriverType.RSR2_RM_1));
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
			var deviceSelectationViewModel = new DeviceSelectationViewModel(LockDevice, XManager.Devices.Where(x => x.DriverType == XDriverType.RSR2_AM_1));
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				Door.LockControlDeviceUID = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.UID : Guid.Empty;
				Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}