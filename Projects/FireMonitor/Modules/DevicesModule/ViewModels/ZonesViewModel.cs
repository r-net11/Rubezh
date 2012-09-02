using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZonesViewModel : ViewPartViewModel, ISelectable<int?>
	{
		public ZonesViewModel()
		{
			ServiceFactory.Events.GetEvent<ZoneSelectedEvent>().Subscribe(Select);
		}

		public void Initialize()
		{
			Zones = (from ZoneState zoneState in FiresecManager.DeviceStates.ZoneStates
					 orderby zoneState.No
					 select new ZoneViewModel(zoneState)).ToList();
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

		public void Select(int? zoneNo)
		{
			if (zoneNo.HasValue)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		void InitializeDevices()
		{
			if (SelectedZone == null)
				return;

			var devices = new HashSet<Device>();

			foreach (var device in FiresecManager.Devices)
			{
				if (device.Driver.IsZoneDevice && device.ZoneNo == SelectedZone.Zone.No)
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
					IsBold = device.Driver.IsZoneDevice || device.Driver.IsZoneLogicDevice
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