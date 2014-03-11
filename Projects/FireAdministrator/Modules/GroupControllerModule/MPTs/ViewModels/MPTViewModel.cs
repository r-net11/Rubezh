using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System;

namespace GKModule.ViewModels
{
	public class MPTViewModel : BaseViewModel
	{
		public XMPT MPT { get; set; }

		public MPTViewModel(XMPT mpt)
		{
			MPT = mpt;
			ChangeStartLogicCommand = new RelayCommand(OnChangeStartLogic);
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			Devices = new ObservableCollection<MPTDeviceViewModel>();
			foreach (var mptDevice in MPT.MPTDevices)
			{
				var deviceViewModel = new MPTDeviceViewModel(mptDevice);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		ObservableCollection<MPTDeviceViewModel> _devices;
		public ObservableCollection<MPTDeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		MPTDeviceViewModel _selectedDevice;
		public MPTDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var mptDeviceTypeSelectationViewModel = new MPTDeviceTypeSelectationViewModel();
			if (DialogService.ShowModalWindow(mptDeviceTypeSelectationViewModel))
			{
				var mptDevice = new MPTDevice();
				mptDevice.MPTDeviceType = mptDeviceTypeSelectationViewModel.SelectedMPTDeviceType.MPTDeviceType;

				var mptDeviceViewModel = new MPTDeviceViewModel(mptDevice);
				Devices.Add(mptDeviceViewModel);
				SelectedDevice = mptDeviceViewModel;
				MPT.MPTDevices.Add(mptDevice);

				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var devices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (MPTDevice.GetAvailableMPTDriverTypes(SelectedDevice.MPTDeviceType).Any(x => device.DriverType == x))
					if (!MPT.MPTDevices.Any(x => x.DeviceUID == device.BaseUID))
						devices.Add(device);
			}

			var deviceSelectationViewModel = new DeviceSelectationViewModel(SelectedDevice.MPTDevice.Device, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				if (SelectedDevice.MPTDevice.Device != null)
				{
					ChangeIsInMPT(SelectedDevice.MPTDevice.Device, false);
				}

				var selectedDevice = deviceSelectationViewModel.SelectedDevice;
				SelectedDevice.MPTDevice.Device = selectedDevice;
				SelectedDevice.MPTDevice.DeviceUID = selectedDevice != null ? selectedDevice.BaseUID : Guid.Empty;
				UpdateConfigurationHelper.CopyMPTProperty(SelectedDevice.MPTDevice);
				SelectedDevice.Device = selectedDevice;
				ChangeIsInMPT(SelectedDevice.MPTDevice.Device, true);

				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedDevice != null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			ChangeIsInMPT(SelectedDevice.MPTDevice.Device, false);
			MPT.MPTDevices.Remove(SelectedDevice.MPTDevice);
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanDelete()
		{
			return SelectedDevice != null;
		}

		public static void ChangeIsInMPT(XDevice device, bool isInMPT)
		{
			if (device != null)
			{
				device.IsInMPT = isInMPT;
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.BaseUID == device.BaseUID);
				if (deviceViewModel != null)
				{
					deviceViewModel.UpdateProperties();
				}
				device.OnChanged();
			}
		}

		public void Update()
		{
			OnPropertyChanged("MPT");
		}

		public RelayCommand ChangeStartLogicCommand { get; private set; }
		void OnChangeStartLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, MPT.StartLogic, false);
			if (DialogService.ShowModalWindow(deviceLogicViewModel))
			{
				MPT.StartLogic = deviceLogicViewModel.GetModel();
				OnPropertyChanged("StartPresentationName");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string StartPresentationName
		{
			get { return XManager.GetPresentationZone(MPT.StartLogic); }
		}

		public int Delay
		{
			get { return MPT.Delay; }
			set
			{
				MPT.Delay = value;
				OnPropertyChanged("Delay");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool UseDoorAutomatic
		{
			get { return MPT.UseDoorAutomatic; }
			set
			{
				MPT.UseDoorAutomatic = value;
				OnPropertyChanged("UseDoorAutomatic");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool UseDoorStop
		{
			get { return MPT.UseDoorStop; }
			set
			{
				MPT.UseDoorStop = value;
				OnPropertyChanged("UseDoorStop");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public bool UseFailureAutomatic
		{
			get { return MPT.UseFailureAutomatic; }
			set
			{
				MPT.UseFailureAutomatic = value;
				OnPropertyChanged("UseFailureAutomatic");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}