using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ZonesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public ZonesViewModel()
		{
			ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Subscribe(Select);
		}

		public void Initialize()
		{
			Zones = (from XZone zone in XManager.DeviceConfiguration.Zones
					 orderby zone.No
					 select new ZoneViewModel(zone.ZoneState)).ToList();

			SelectedZone = Zones.FirstOrDefault();
		}

		List<ZoneViewModel> _zones;
		public List<ZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged("Zones");
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				InitializeDevices();
				OnPropertyChanged("SelectedZone");
			}
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		void InitializeDevices()
		{
			if (SelectedZone == null)
				return;

			var devices = new HashSet<XDevice>();

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.ZoneUIDs.Contains(SelectedZone.Zone.UID))
				{
					device.AllParents.ForEach(x => { devices.Add(x); });
					devices.Add(device);
				}
			}

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				Devices.Add(new DeviceViewModel(device, Devices)
				{
					IsExpanded = true,
					IsBold = device.ZoneUIDs.Contains(SelectedZone.Zone.UID)
				});
			}

			foreach (var device in Devices)
			{
				if (device.Device.Parent != null)
				{
					var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
					device.Parent = parent;
					parent.Children.Add(device);
				}
			}

			OnPropertyChanged("Devices");
		}
	}
}