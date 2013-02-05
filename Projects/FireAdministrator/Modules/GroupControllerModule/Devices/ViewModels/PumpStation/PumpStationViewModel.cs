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
using Infrastructure;

namespace GKModule.ViewModels
{
	public class PumpStationViewModel : SaveCancelDialogViewModel
	{
		XDevice Device;

		public PumpStationViewModel(XDevice device)
		{
			Title = "Насосная станция";
			ChooseDrenajPumpCommand = new RelayCommand(OnChooseDrenajPump);
			ChangeCommand = new RelayCommand(OnChange);
			Device = device;
			if (device.PumpStationProperty != null)
			{
				device.PumpStationProperty = new XPumpStationProperty();
			}

			PumpsCount = device.PumpStationProperty.PumpsCount;
			DelayTime = device.PumpStationProperty.DelayTime;
			JokeyPumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.PumpStationProperty.JokeyPumpUID);
			DrenajPumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.PumpStationProperty.DrenajPumpUID);
			CompressorPumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.PumpStationProperty.CompressorPumpUID);

			Devices = new List<XDevice>();
				foreach (var deviceUID in device.PumpStationProperty.DeviceUIDs)
				{
					var pumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (pumpDevice != null)
					{
						Devices.Add(pumpDevice);
					}
				}
			}

		ushort _pumpsCount;
		public ushort PumpsCount
		{
			get { return _pumpsCount; }
			set
			{
				_pumpsCount = value;
				OnPropertyChanged("PumpsCount");
			}
		}

		ushort _delayTime;
		public ushort DelayTime
		{
			get { return _delayTime; }
			set
			{
				_delayTime = value;
				OnPropertyChanged("DelayTime");
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

		XDevice _jokeyPumpDevice;
		public XDevice JokeyPumpDevice
		{
			get { return _jokeyPumpDevice; }
			set
			{
				_jokeyPumpDevice = value;
				OnPropertyChanged("JokeyPumpDevice");
			}
		}

		XDevice _drenajPumpDevice;
		public XDevice DrenajPumpDevice
		{
			get { return _drenajPumpDevice; }
			set
			{
				_drenajPumpDevice = value;
				OnPropertyChanged("DrenajPumpDevice");
			}
		}

		XDevice _compressorPumpDevice;
		public XDevice CompressorPumpDevice
		{
			get { return _compressorPumpDevice; }
			set
			{
				_compressorPumpDevice = value;
				OnPropertyChanged("CompressorPumpDevice");
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

		public RelayCommand ChooseDrenajPumpCommand { get; private set; }
		void OnChooseDrenajPump()
		{
			var devices = new List<XDevice>();
			foreach(var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.Pump)
					devices.Add(device);
			}
			var deviceSelectationViewModel = new DeviceSelectationViewModel(devices, DrenajPumpDevice);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				DrenajPumpDevice = deviceSelectationViewModel.SelectedDevice.Device;
			}
		}

		protected override bool Save()
		{
			Device.PumpStationProperty = new XPumpStationProperty()
			{
				PumpsCount = PumpsCount,
				DelayTime = DelayTime
			};
			foreach (var device in Devices)
			{
				Device.PumpStationProperty.DeviceUIDs.Add(device.UID);
			}
			return true;
		}
	}
}