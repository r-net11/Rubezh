using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class AccessZonesSelectationViewModel : SaveCancelDialogViewModel
	{
		public AccessZonesSelectationViewModel()
		{
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}
		}

		public List<AccessZoneViewModel> AllZones;

		AccessZoneViewModel _selectedZone;
		public AccessZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedZone");
				InitializeDevices();
			}
		}

		AccessZoneViewModel _rootZone;
		public AccessZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public AccessZoneViewModel[] RootZones
		{
			get { return new AccessZoneViewModel[] { RootZone }; }
		}

		AccessZoneViewModel AddZoneInternal(SKDZone zone, AccessZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new AccessZoneViewModel(zone);
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}

		void InitializeDevices()
		{
			Devices = new ObservableCollection<SKDDevice>();
			if (SelectedZone != null)
			{
				foreach (var device in SKDManager.Devices)
				{
					if (device.DriverType == XFiresecAPI.SKDDriverType.Controller && device.ZoneUID == SelectedZone.Zone.UID)
						Devices.Add(device);
				}
			}
		}

		ObservableCollection<SKDDevice> _devices;
		public ObservableCollection<SKDDevice> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		protected override bool Save()
		{
			return true;
		}
	}
}