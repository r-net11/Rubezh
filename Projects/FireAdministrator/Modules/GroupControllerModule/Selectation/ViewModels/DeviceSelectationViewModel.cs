using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DeviceSelectationViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectationViewModel(XDevice selectedDevice, List<XDevice> sourceDevices = null)
		{
			Title = "Выбор устройства";

			Devices = new ObservableCollection<XDevice>(sourceDevices);
			if (selectedDevice != null)
				SelectedDevice = Devices.FirstOrDefault(x => x.UID == selectedDevice.UID);
		}

		public ObservableCollection<XDevice> Devices { get; private set; }

		XDevice _selectedDevice;
		public XDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}
	}
}