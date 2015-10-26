﻿using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using RubezhClient;

namespace GKModule.ViewModels
{
	public class DevicesListViewModel : DialogViewModel
	{
		public DevicesListViewModel()
		{
			Title = "Список устройств";
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in GKManager.Devices)
			{
				if (device.IsRealDevice)
				{
					var deviceViewModel = new DeviceViewModel(device);
					deviceViewModel.CanUpdateDescriptorName = true;
					deviceViewModel.UpdateDescriptorName();
					Devices.Add(deviceViewModel);
				}
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}
	}
}