using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class SKDZonesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static SKDZonesViewModel Current { get; private set; }
		public SKDZonesViewModel()
		{
			Current = this;
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<SKDZoneViewModel>();
			foreach (var skdZone in GKManager.SKDZones.OrderBy(x => x.No))
			{
				var zoneViewModel = new SKDZoneViewModel(skdZone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<SKDZoneViewModel> _zones;
		public ObservableCollection<SKDZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		SKDZoneViewModel _selectedZone;
		public SKDZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				InitializeDevices();
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				SelectedZone = Zones.FirstOrDefault(x => x.SKDZone.UID == zoneUID);
			}
		}

		public DeviceViewModel RootDevice { get; private set; }
		public DeviceViewModel[] RootDevices
		{
			get { return RootDevice == null ? null : new[] { RootDevice }; }
		}

		void InitializeDevices()
		{
			if (SelectedZone == null)
				return;

			var devices = new HashSet<GKDevice>();

			foreach (var device in GKManager.Devices)
			{
			}

			var deviceViewModels = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				deviceViewModels.Add(new DeviceViewModel(device)
				{
					IsExpanded = true,
				});
			}

			foreach (var device in deviceViewModels)
			{
				if (device.Device.Parent != null)
				{
					var parent = deviceViewModels.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
					if (parent != null) parent.AddChild(device);
				}
			}

			RootDevice = deviceViewModels.FirstOrDefault(x => x.Parent == null);
			OnPropertyChanged(() => RootDevices);
		}
	}
}