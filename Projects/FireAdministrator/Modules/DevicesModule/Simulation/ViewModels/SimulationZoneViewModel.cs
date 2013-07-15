using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecAPI;

namespace DevicesModule.ViewModels
{
	public class SimulationZoneViewModel : BaseViewModel
	{
		public SimulationZoneViewModel(Zone zone)
		{
			Zone = zone;
			SetFireCommand = new RelayCommand(OnSetFire, CanSetFire);
			SetNormCommand = new RelayCommand(OnSetNorm, CanSetNorm);

			ZoneState = ZoneLogicState.Fire;
			StateType = StateType.Fire;
		}

		public void Initialize()
		{
			Devices = new ObservableCollection<SimulationDeviceViewModel>();
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				var simulationDeviceViewModel = new SimulationDeviceViewModel(device);
				Devices.Add(simulationDeviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public Zone Zone { get; private set; }

		ObservableCollection<SimulationDeviceViewModel> _devices;
		public ObservableCollection<SimulationDeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		SimulationDeviceViewModel _selectedDevice;
		public SimulationDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		ZoneLogicState? _zoneState;
		public ZoneLogicState? ZoneState
		{
			get { return _zoneState; }
			set
			{
				_zoneState = value;
				OnPropertyChanged("ZoneState");
			}
		}

		public StateType StateType { get; set; }

		public RelayCommand SetFireCommand { get; private set; }
		void OnSetFire()
		{
			ZoneState = ZoneLogicState.Fire;
		}
		bool CanSetFire()
		{
			return Zone.ZoneType == ZoneType.Fire;
		}

		public RelayCommand SetNormCommand { get; private set; }
		void OnSetNorm()
		{
			ZoneState = null;
		}
		bool CanSetNorm()
		{
			return ZoneState != null;
		}
	}
}