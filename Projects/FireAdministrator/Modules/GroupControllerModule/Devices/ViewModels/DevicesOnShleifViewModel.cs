﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DevicesOnShleifViewModel : DialogViewModel
	{
		public GKDevice ShleifDevice { get; private set; }

		public DevicesOnShleifViewModel(GKDevice shleifDevice)
		{
			Title = "Выбор устройств на АЛС " + shleifDevice.PresentationName;
			ShleifDevice = shleifDevice;
			SelectAllCommand = new RelayCommand(OnSelectAll);
			DeSelectAllCommand = new RelayCommand(OnDeSelectAll);
			CopyCommand = new RelayCommand(OnCopy);
			RemoveCommand = new RelayCommand(OnRemove);

			Devices = new ObservableCollection<DeviceOnShleifViewModel>();
			foreach (var device in shleifDevice.Children)
			{
				var deviceOnShleifViewModel = new DeviceOnShleifViewModel(device);
				Devices.Add(deviceOnShleifViewModel);
			}
		}

		public List<GKDevice> CopyDevices { get; private set; }
		public ObservableCollection<DeviceOnShleifViewModel> Devices { get; private set; }

		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			foreach (var device in Devices)
			{
				device.IsActive = true;
			}
		}

		public RelayCommand DeSelectAllCommand { get; private set; }
		void OnDeSelectAll()
		{
			foreach (var device in Devices)
			{
				device.IsActive = false;
			}
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			var devicesToCopy = new List<GKDevice>();
			foreach (var device in Devices)
			{
				if (device.IsActive)
				{
					devicesToCopy.Add(GKManager.CopyDevice(device.Device, false));
				}
			}
			DevicesViewModel.Current.DevicesToCopy = devicesToCopy;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			foreach (var deviceOnShleif in Devices)
			{
				if (deviceOnShleif.IsActive)
				{
					var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == deviceOnShleif.Device.UID);
					if (deviceViewModel != null)
					{
						deviceViewModel.Remove(false);
					}
				}
			}
			ShleifDevice.OnAUParametersChanged();

			if (ShleifDevice.KAUParent != null)
				GKManager.RebuildRSR2Addresses(ShleifDevice);

			Devices = new ObservableCollection<DeviceOnShleifViewModel>(Devices.Where(x => !x.IsActive));
			OnPropertyChanged(() => Devices);
		}
	}

	public class DeviceOnShleifViewModel : BaseViewModel
	{
		public GKDevice Device { get; private set; }

		public DeviceOnShleifViewModel(GKDevice device)
		{
			Device = device;
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}
	}
}