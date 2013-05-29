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

			Device = device;
			PumpsCount = device.PumpStationProperty.PumpsCount;
			DelayTime = device.PumpStationProperty.DelayTime;

			PumpDevices = new ObservableCollection<PumpDeviceViewModel>();
			foreach (var pumpStationPump in device.PumpStationProperty.PumpStationPumps)
			{
				var pumpDeviceViewModel = new PumpDeviceViewModel(pumpStationPump);
				PumpDevices.Add(pumpDeviceViewModel);
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

		ObservableCollection<PumpDeviceViewModel> _pumpDevices;
		public ObservableCollection<PumpDeviceViewModel> PumpDevices
		{
			get { return _pumpDevices; }
			set
			{
				_pumpDevices = value;
				OnPropertyChanged("PumpDevices");
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

		public RelayCommand ChangePumpsCommand { get; private set; }
		void OnChangePumps()
		{
			var sourceDevices = new List<XDevice>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.Pump)
					sourceDevices.Add(device);
			}
			var devices = new List<XDevice>();
			foreach (var pumpDevice in PumpDevices)
			{
				devices.Add(pumpDevice.Device);
			}

			PumpDevices.Clear();
			var devicesSelectationViewModel = new DevicesSelectationViewModel(devices, sourceDevices);
			if (DialogService.ShowModalWindow(devicesSelectationViewModel))
			{
				foreach (var device in devicesSelectationViewModel.DevicesList)
				{
					var pumpStationPump = new XPumpStationPump()
					{
						DeviceUID = device.UID,
						Device = device
					};
					var pumpDeviceViewModel = new PumpDeviceViewModel(pumpStationPump);
					PumpDevices.Add(pumpDeviceViewModel);
				}
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

		protected override bool Save()
		{
			Device.PumpStationProperty = new XPumpStationProperty()
			{
				PumpsCount = PumpsCount,
				DelayTime = DelayTime,
			};

			foreach (var pumpDevice in PumpDevices)
			{
				var pumpStationPump = new XPumpStationPump()
				{
					Device = pumpDevice.Device,
					DeviceUID = pumpDevice.Device.UID,
					PumpStationPumpType = pumpDevice.SelectedPumpStationPumpType
				};
				Device.PumpStationProperty.PumpStationPumps.Add(pumpStationPump);
			}
			foreach (var direction in Directions)
			{
				Device.PumpStationProperty.DirectionUIDs.Add(direction.UID);
			}
			return true;
		}
	}
}