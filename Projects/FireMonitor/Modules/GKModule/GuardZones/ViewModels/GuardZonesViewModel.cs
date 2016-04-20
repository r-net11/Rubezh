using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Models.Layouts;
using System.Windows;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class GuardZonesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static GuardZonesViewModel Current { get; private set; }
		public GridLength TopPanelRowHeight { get; private set; }
		public GridLength BottomPanelRowHeight { get; private set; }
		public GuardZonesViewModel()
		{
			Current = this;
			IsVisibleBottomPanel = true;
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<GuardZoneViewModel>();
			foreach (var guardZone in GKManager.DeviceConfiguration.GuardZones.OrderBy(x => x.No))
			{
				var zoneViewModel = new GuardZoneViewModel(guardZone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<GuardZoneViewModel> _zones;
		public ObservableCollection<GuardZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		GuardZoneViewModel _selectedZone;
		public GuardZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				InitializeDevices();
				OnPropertyChanged(() => SelectedZone);
			}
		}
		LayoutPartAdditionalProperties _properties;
		public LayoutPartAdditionalProperties Properties
		{
			get { return _properties; }
			set
			{
				_properties = value;
				IsVisibleBottomPanel = (_properties != null) ? _properties.IsVisibleBottomPanel : false;
			}
		}
		bool _isVisibleBottomPanel;
		public bool IsVisibleBottomPanel
		{
			get { return _isVisibleBottomPanel; }
			set
			{
				_isVisibleBottomPanel = value;
				RowsHeightCalculate();
				OnPropertyChanged(() => IsVisibleBottomPanel);
			}
		}
		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				SelectedZone = Zones.FirstOrDefault(x => x.GuardZone.UID == zoneUID);
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
				if (device.Driver.HasLogic)
				{
					foreach (var clause in device.Logic.OnClausesGroup.Clauses)
					{
						foreach (var clauseZone in clause.GuardZones)
						{
							if (clauseZone.UID == SelectedZone.GuardZone.UID)
							{
								device.AllParents.ForEach(x => { devices.Add(x); });
								devices.Add(device);
							}
						}
					}
				}

				if (device.Driver.HasGuardZone)
				{
					if (SelectedZone.GuardZone.GuardZoneDevices.Any(x => x.DeviceUID == device.UID))
					{
						device.AllParents.ForEach(x => devices.Add(x));
						devices.Add(device);
					}
				}
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
		void RowsHeightCalculate()
		{
			if (IsVisibleBottomPanel)
			{
				TopPanelRowHeight = new GridLength(3, GridUnitType.Star);
				BottomPanelRowHeight = new GridLength(1, GridUnitType.Star);
			}
			else
			{
				TopPanelRowHeight = new GridLength(1, GridUnitType.Star);
				BottomPanelRowHeight = new GridLength(1, GridUnitType.Auto);
			}
		}
	}
}