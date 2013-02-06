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
			ChangePumpsCommand = new RelayCommand(OnChangePumps);
			ChangeDirectionsCommand = new RelayCommand(OnChangeDirections);
			ChooseJokeyPumpCommand = new RelayCommand(OnChooseJokeyPump);
			ChooseDrenajPumpCommand = new RelayCommand(OnChooseDrenajPump);
			ChooseCompressorPumpCommand = new RelayCommand(OnChooseCompressorPump);
			ChooseCompensationPumpCommand = new RelayCommand(OnChooseCompensationPump);
			Device = device;
			if (device.PumpStationProperty == null)
			{
				device.PumpStationProperty = new XPumpStationProperty();
			}

			PumpsCount = device.PumpStationProperty.PumpsCount;
			DelayTime = device.PumpStationProperty.DelayTime;
			JokeyPumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.PumpStationProperty.JokeyPumpUID);
			DrenajPumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.PumpStationProperty.DrenajPumpUID);
			CompressorPumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.PumpStationProperty.CompressorPumpUID);

			Devices = new List<XDevice>();
			foreach (var deviceUID in device.PumpStationProperty.FirePumpUIDs)
			{
				var pumpDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (pumpDevice != null)
				{
					Devices.Add(pumpDevice);
				}
			}

			Directions = new List<XDirection>();
			foreach (var directionUID in device.PumpStationProperty.DirectionUIDs)
			{
				var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == directionUID);
				if (direction != null)
				{
					Directions.Add(direction);
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

		List<XDirection> _directions;
		public List<XDirection> Directions
		{
			get { return _directions; }
			set
			{
				_directions = value;
				OnPropertyChanged("Directions");
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
		public RelayCommand ChangePumpsCommand { get; private set; }
		void OnChangePumps()
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
			}
		}

		public RelayCommand ChangeDirectionsCommand { get; private set; }
		void OnChangeDirections()
		{
			var directionsSelectationViewModel = new DirectionsSelectationViewModel(Directions);
			if (DialogService.ShowModalWindow(directionsSelectationViewModel))
			{
				Directions = directionsSelectationViewModel.Directions;
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
		public RelayCommand ChooseJokeyPumpCommand { get; private set; }
		void OnChooseJokeyPump()
		{
			var device = ChoosePump(12);
			if(device != null)
			{
				JokeyPumpDevice = device;
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
		public RelayCommand ChooseDrenajPumpCommand { get; private set; }
		void OnChooseDrenajPump()
		{
			var device = ChoosePump(14);
			if (device != null)
			{
				DrenajPumpDevice = device;
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
		public RelayCommand ChooseCompressorPumpCommand { get; private set; }
		void OnChooseCompressorPump()
		{
			var device = ChoosePump(13);
			if (device != null)
			{
				CompressorPumpDevice = device;
			}
		}

		XDevice _compensationPumpDevice;
		public XDevice CompensationPumpDevice
		{
			get { return _compensationPumpDevice; }
			set
			{
				_compensationPumpDevice = value;
				OnPropertyChanged("CompensationPumpDevice");
			}
		}
		public RelayCommand ChooseCompensationPumpCommand { get; private set; }
		void OnChooseCompensationPump()
		{
			var device = ChoosePump(15);
			if (device != null)
			{
				CompensationPumpDevice = device;
			}
		}

		XDevice ChoosePump(byte addressOnShleif)
		{
			var devices = new List<XDevice>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.IntAddress % 256 == addressOnShleif)
				{
					if (device.Driver.DriverType == XDriverType.Pump)
						devices.Add(device);
				}
			}
			var deviceSelectationViewModel = new DeviceSelectationViewModel(devices, DrenajPumpDevice);
			if (DialogService.ShowModalWindow(deviceSelectationViewModel))
			{
				var pumpDevice = deviceSelectationViewModel.SelectedDevice.Device;
				return pumpDevice;
			}
			return null;
		}

		protected override bool Save()
		{
			Device.PumpStationProperty = new XPumpStationProperty()
			{
				PumpsCount = PumpsCount,
				DelayTime = DelayTime,
				DrenajPumpUID = DrenajPumpDevice != null ? DrenajPumpDevice.UID : Guid.Empty,
				JokeyPumpUID = JokeyPumpDevice != null ? JokeyPumpDevice.UID : Guid.Empty,
				CompressorPumpUID = CompressorPumpDevice != null ? CompressorPumpDevice.UID : Guid.Empty,
				CompensationPumpUID = CompensationPumpDevice != null ? CompensationPumpDevice.UID : Guid.Empty
			};
			foreach (var device in Devices)
			{
				Device.PumpStationProperty.FirePumpUIDs.Add(device.UID);
			}
			foreach (var direction in Directions)
			{
				Device.PumpStationProperty.DirectionUIDs.Add(direction.UID);
			}
			return true;
		}
	}
}