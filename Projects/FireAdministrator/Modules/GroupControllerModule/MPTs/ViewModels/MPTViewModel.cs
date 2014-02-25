using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure;
using FiresecClient;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

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
			var devices = new List<XDevice>();
			foreach (var device in XManager.Devices)
			{
				switch (device.DriverType)
				{
					case XDriverType.RSR2_MVK8:
					case XDriverType.RSR2_Table:
					case XDriverType.RSR2_Siren:
					case XDriverType.RSR2_AM_1:
						devices.Add(device);
						break;
				}
			}

			var deviceSelectationViewModel = new DeviceSelectationViewModel(null, devices);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				deviceSelectationViewModel.SelectedDevice.IsInMPT = true;

				var mptDevice = new MPTDevice();
				mptDevice.Device = deviceSelectationViewModel.SelectedDevice;
				mptDevice.DeviceUID = mptDevice.Device.UID;
				mptDevice.MPTDeviceType = MPTDevice.GetAvailableMPTDeviceTypes(mptDevice.Device.DriverType).FirstOrDefault();

				var deviceViewModel = new MPTDeviceViewModel(mptDevice);
				Devices.Add(deviceViewModel);
				SelectedDevice = deviceViewModel;
				MPT.MPTDevices.Add(mptDevice);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Devices.Remove(SelectedDevice);
			MPT.MPTDevices.Remove(SelectedDevice.MPTDevice);
			SelectedDevice.MPTDevice.Device.IsInMPT = true;
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanDelete()
		{
			return SelectedDevice != null;
		}

		public void Update()
		{
			OnPropertyChanged("MPT");
		}

		public RelayCommand ChangeStartLogicCommand { get; private set; }
		void OnChangeStartLogic()
		{
			var deviceLogicViewModel = new DeviceLogicViewModel(XManager.DeviceConfiguration.RootDevice, MPT.StartLogic);
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
	}
}