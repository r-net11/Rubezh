﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(GKDevice selectedDevice, IEnumerable<GKDevice> sourceDevices = null)
		{
			Title = "Выбор устройства";
			Devices = new ObservableCollection<GKDevice>(sourceDevices);
			if (selectedDevice != null)
				SelectedDevice = Devices.FirstOrDefault(x => x.UID == selectedDevice.UID);
		}

		public ObservableCollection<GKDevice> Devices { get; private set; }

		GKDevice _selectedDevice;
		public GKDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public string Description
		{
			get
			{
				return SelectedDevice != null ? SelectedDevice.Description : "";
			}
			set
			{
				if (SelectedDevice != null)
				{
					SelectedDevice.Description = value;
				}
				OnPropertyChanged(() => Description);
			}
		}
	}
}