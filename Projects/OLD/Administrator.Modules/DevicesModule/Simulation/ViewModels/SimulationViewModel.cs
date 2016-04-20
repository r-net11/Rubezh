using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;

namespace DevicesModule.ViewModels
{
	public class SimulationViewModel : MenuViewPartViewModel
	{
		public static SimulationViewModel Current { get; private set; }

		public SimulationViewModel()
		{
			Current = this;
			Menu = new SimulationMenuViewModel(this);
			ResetCommand = new RelayCommand(OnReset, CanReset);
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<SimulationZoneViewModel>();
			foreach (var zone in FiresecManager.Zones)
			{
				var simulationZoneViewModel = new SimulationZoneViewModel(zone);
				Zones.Add(simulationZoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
			Devices = new ObservableCollection<SimulationDeviceViewModel>();
		}

		ObservableCollection<SimulationZoneViewModel> _zones;
		public ObservableCollection<SimulationZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		SimulationZoneViewModel _selectedZone;
		public SimulationZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					value.Initialize();
				OnPropertyChanged(() => SelectedZone);
			}
		}

		ObservableCollection<SimulationDeviceViewModel> _devices;
		public ObservableCollection<SimulationDeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
		}

		SimulationDeviceViewModel _selectedDevice;
		public SimulationDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		public void UpdateDevices()
		{
			var devices = new HashSet<Device>();
			foreach (var zone in Zones)
			{
				if (zone.ZoneState != null)
				{
					foreach (var device in zone.Zone.DevicesInZoneLogic)
					{
						foreach (var clause in device.ZoneLogic.Clauses)
						{
							if (clause.State == zone.ZoneState)
							{
								var allZonesHaveState = true;
								if (clause.Operation.Value == ZoneLogicOperation.All)
								{
									foreach (var claseZone in clause.Zones)
									{
										var simulationZoneViewModel = Zones.FirstOrDefault(x => x.Zone.UID == claseZone.UID);
										if (simulationZoneViewModel != null)
										{
											if (simulationZoneViewModel.ZoneState != zone.ZoneState)
											{
												allZonesHaveState = false;
												break;
											}
										}
									}
								}
								if (allZonesHaveState)
								{
									devices.Add(device);
								}
							}
						}
					}
				}
			}
			Devices = new ObservableCollection<SimulationDeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new SimulationDeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			foreach (var zone in Zones)
			{
				zone.ZoneState = null;
			}
			Devices = new ObservableCollection<SimulationDeviceViewModel>();
		}
		bool CanReset()
		{
			return Devices.Count > 0;
		}

		int FSChangesCount;
		public override void OnShow()
		{
			base.OnShow();
			if (ServiceFactory.SaveService.FSChangesCount > FSChangesCount)
			{
				FSChangesCount = ServiceFactory.SaveService.FSChangesCount;
				Initialize();
			}
		}

		public override void OnHide()
		{
			base.OnHide();
		}
	}
}