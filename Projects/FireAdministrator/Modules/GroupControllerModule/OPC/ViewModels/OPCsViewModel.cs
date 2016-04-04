using System.Collections.ObjectModel;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using RubezhAPI.GK;
using System;
using Infrastructure;
using Infrastructure.Events;
using System.ComponentModel;
using System.Windows.Input;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class OPCsViewModel : MenuViewPartViewModel
	{

		public OPCsViewModel()
		{
			AddCommand = new RelayCommand(Add);
			DeleteCommand = new RelayCommand(Delete, CanDelete);

			Menu = new OPCMenuViewModel(this);
		}

		public void Initialize()
		{
			Devices = new ObservableCollection<OPCItemViewModel>();

			foreach (var zone in GKManager.Zones.Where(x=> GKManager.DeviceConfiguration.OPCSettings.ZoneUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(zone);
				Devices.Add(OPCItem);
			}

			foreach (var device in GKManager.Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DeviceUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(device);
				Devices.Add(OPCItem);
			}

			foreach (var delay in GKManager.Delays.Where(x=> GKManager.DeviceConfiguration.OPCSettings.DelayUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(delay);
				Devices.Add(OPCItem);
			}

			foreach (var guardZone in GKManager.GuardZones.Where(x=> GKManager.DeviceConfiguration.OPCSettings.GuardZoneUIDs.Contains(x.UID)))
			{
				var  OPCItem = new OPCItemViewModel(guardZone);
				Devices.Add(OPCItem);
			}

			foreach (var derection in GKManager.Directions.Where(x => GKManager.DeviceConfiguration.OPCSettings.DiretionUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(derection);
				Devices.Add(OPCItem);
			}

			foreach (var mpt in GKManager.MPTs.Where(x => GKManager.DeviceConfiguration.OPCSettings.MPTUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(mpt);
				Devices.Add(OPCItem);
			}

			foreach (var pump in GKManager.PumpStations.Where(x=> GKManager.DeviceConfiguration.OPCSettings.NSUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(pump);
				Devices.Add(OPCItem);	
			}

			foreach (var door in GKManager.Doors .Where(x => GKManager.DeviceConfiguration.OPCSettings.DoorUIDs.Contains(x.UID)))
			{
				var OPCItem = new OPCItemViewModel(door);
				Devices.Add(OPCItem);
			}

			SelectedDevices = Devices.FirstOrDefault();
		}

		ObservableCollection<OPCItemViewModel> _devices;
		public ObservableCollection<OPCItemViewModel> Devices
		{
			get { return _devices; }
			private set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
		}

		OPCItemViewModel _selectedDevices;
		public OPCItemViewModel SelectedDevices
		{
			get { return _selectedDevices; }
			set
			{
				_selectedDevices = value;
				OnPropertyChanged(() => SelectedDevices);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void Add()
		{
			var opcDetailsViewModel = new OPCDetailsViewModel();
			if (DialogService.ShowModalWindow(opcDetailsViewModel))
			{
				Initialize();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void Delete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить" + SelectedDevices.Device.PresentationName))
			{
				var index = Devices.IndexOf(SelectedDevices);
				Devices.Remove(SelectedDevices);
				GKManager.DeviceConfiguration.OPCSettings.DoorUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DoorUIDs.Contains(x.Device.UID)).Select(x=>x.Device.UID).ToList();
				GKManager.DeviceConfiguration.OPCSettings.ZoneUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.ZoneUIDs.Contains(x.Device.UID)).Select(x => x.Device.UID).ToList();
				GKManager.DeviceConfiguration.OPCSettings.DelayUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DelayUIDs.Contains(x.Device.UID)).Select(x => x.Device.UID).ToList();
				GKManager.DeviceConfiguration.OPCSettings.DeviceUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DeviceUIDs.Contains(x.Device.UID)).Select(x => x.Device.UID).ToList();
				GKManager.DeviceConfiguration.OPCSettings.DiretionUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DiretionUIDs.Contains(x.Device.UID)).Select(x => x.Device.UID).ToList();
				GKManager.DeviceConfiguration.OPCSettings.GuardZoneUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.GuardZoneUIDs.Contains(x.Device.UID)).Select(x => x.Device.UID).ToList();
				GKManager.DeviceConfiguration.OPCSettings.MPTUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.MPTUIDs.Contains(x.Device.UID)).Select(x => x.Device.UID).ToList();
				GKManager.DeviceConfiguration.OPCSettings.NSUIDs = Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.NSUIDs.Contains(x.Device.UID)).Select(x => x.Device.UID).ToList();
				index = Math.Min(index, Devices.Count - 1);
				if (index > -1)
					SelectedDevices = Devices[index];
				ServiceFactory.SaveService.GKChanged = true;
			}	
		}
		bool CanDelete()
		{
			return Devices.Count > 0;
		}

		public override void OnShow()
		{
			Initialize();
			base.OnShow();
		}
	}
}