using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class PumpStationViewModel : SaveCancelDialogViewModel
	{
		XDevice Device;

		public PumpStationViewModel(XDevice device)
		{
			Title = "Насосная станция";
			ChangeCommand = new RelayCommand(OnChange);
			Device = device;
			Devices = new List<XDevice>();
			if (device.PumpStationProperty != null)
			{
				foreach (var deviceUID in device.PumpStationProperty.DeviceUIDs)
				{
					var pumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
if(pumpDevice != null)
{
	Devices.Add(pumpDevice);
}
				}
			}
		}

		List<XDevice> _devices;
		public List<XDevice> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		public RelayCommand ChangeCommand { get; private set; }
		void OnChange()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.Pump)
					sourceDevices.Add(device);
			}
			var devicesSelectationViewModel = new DevicesSelectationViewModel(Devices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				Devices = devicesSelectationViewModel.DevicesList;
				OnPropertyChanged("PresenrationDevices");
			}
		}

		protected override bool Save()
		{
			Device.PumpStationProperty = new XPumpStationProperty();
			foreach (var device in Devices)
			{
				Device.PumpStationProperty.DeviceUIDs.Add(device.UID);
			}
			return true;
		}
	}
}