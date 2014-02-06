using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class ZonesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static ZonesViewModel Current { get; private set; }
		public ZonesViewModel()
		{
			Current = this;
		}

		public void Initialize()
		{
			BuildTree();
			if (RootZone != null)
			{
				RootZone.IsExpanded = true;
				SelectedZone = RootZone;
			}

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}

			OnPropertyChanged("RootZones");
		}

		#region ZoneSelection
		public List<ZoneViewModel> AllZones;

		public void FillAllZones()
		{
			AllZones = new List<ZoneViewModel>();
			AddChildPlainZones(RootZone);
		}

		void AddChildPlainZones(ZoneViewModel parentViewModel)
		{
			AllZones.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainZones(childViewModel);
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				var zoneViewModel = AllZones.FirstOrDefault(x => x.Zone.UID == zoneUID);
				if (zoneViewModel != null)
					zoneViewModel.ExpandToThis();
				SelectedZone = zoneViewModel;
			}
		}
		#endregion

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
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

		ZoneViewModel _rootZone;
		public ZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public ZoneViewModel[] RootZones
		{
			get { return new ZoneViewModel[] { RootZone }; }
		}

		void BuildTree()
		{
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			FillAllZones();
		}

		private ZoneViewModel AddZoneInternal(SKDZone zone, ZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new ZoneViewModel(zone);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}

		ObservableCollection<DeviceViewModel> _devices;
		public ObservableCollection<DeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		void InitializeDevices()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.HasOuterZone)
				{
					if (device.OuterZoneUID == SelectedZone.Zone.UID)
					{
						var deviceViewModel = new DeviceViewModel(device);
						Devices.Add(deviceViewModel);
					}
				}
			}
		}
	}
}