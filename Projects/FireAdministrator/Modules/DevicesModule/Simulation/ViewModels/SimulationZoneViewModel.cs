using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
	public class SimulationZoneViewModel : BaseViewModel
	{
		public SimulationZoneViewModel(Zone zone)
		{
			Zone = zone;
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
	}
}