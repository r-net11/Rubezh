using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	[SaveSizeAttribute]
	public class ZonesSelectionViewModel : SaveCancelDialogViewModel
	{
		public List<Guid> Zones { get; private set; }

		public ZonesSelectionViewModel(Device device, List<Guid> zones, ZoneLogicState zoneLogicState)
		{
			Title = "Выбор зон";
			AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
			RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

			Zones = zones;
			TargetZones = new ObservableCollection<ZoneViewModel>();
			SourceZones = new ObservableCollection<ZoneViewModel>();

			var zoneTypeFilter = ZoneType.Fire;
			switch (zoneLogicState)
			{
				case ZoneLogicState.Alarm:
				case ZoneLogicState.GuardSet:
				case ZoneLogicState.GuardUnSet:
				case ZoneLogicState.PCN:
				case ZoneLogicState.Lamp:
					zoneTypeFilter = ZoneType.Guard;
					break;
			}

			List<Zone> availableZones = FiresecManager.FiresecConfiguration.GetChannelZones(device);
			if (device.Driver.DriverType == DriverType.Exit)
			{
				availableZones = FiresecManager.FiresecConfiguration.GetPanelZones(device);
			}

			foreach (var zone in availableZones)
			{
				var zoneViewModel = new ZoneViewModel(zone);

				if (zone.ZoneType != zoneTypeFilter)
				{
					continue;
				}

				if ((zoneLogicState == ZoneLogicState.MPTAutomaticOn) || (zoneLogicState == ZoneLogicState.MPTOn) || (zoneLogicState == ZoneLogicState.Firefighting))
				{
					if (device.ParentPanel.Children.Any(x => x.Driver.DriverType == DriverType.MPT && x.ZoneUID == zone.UID) == false)
					{
						continue;
					}
				}

                if (Zones.Contains(zone.UID))
					TargetZones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public ObservableCollection<ZoneViewModel> SourceZones { get; private set; }

		ZoneViewModel _selectedSourceZone;
		public ZoneViewModel SelectedSourceZone
		{
			get { return _selectedSourceZone; }
			set
			{
				_selectedSourceZone = value;
				OnPropertyChanged("SelectedSourceZone");
			}
		}

		public ObservableCollection<ZoneViewModel> TargetZones { get; private set; }

		ZoneViewModel _selectedTargetZone;
		public ZoneViewModel SelectedTargetZone
		{
			get { return _selectedTargetZone; }
			set
			{
				_selectedTargetZone = value;
				OnPropertyChanged("SelectedTargetZone");
			}
		}

		public RelayCommand AddOneCommand { get; private set; }
		void OnAddOne()
		{
            var tempZones = new ObservableCollection<ZoneViewModel>(SourceZones);
            foreach (var zone in SourceZones)
		    {
                if (zone.IsSelected)
                {
                    TargetZones.Add(zone);
                    tempZones.Remove(zone);
                }
		    }
            SourceZones = new ObservableCollection<ZoneViewModel>(tempZones);
            OnPropertyChanged("SourceZones");
		    SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public RelayCommand RemoveOneCommand { get; private set; }
		void OnRemoveOne()
		{
            var tempZones = new ObservableCollection<ZoneViewModel>(TargetZones);
            foreach (var zone in TargetZones)
            {
                if (zone.IsSelected)
                {
                    SourceZones.Add(zone);
                    tempZones.Remove(zone);
                }
            }
            TargetZones = new ObservableCollection<ZoneViewModel>(tempZones);
            OnPropertyChanged("TargetZones");
            SelectedSourceZone = TargetZones.FirstOrDefault();

		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var zoneViewModel in SourceZones)
			{
				TargetZones.Add(zoneViewModel);
			}
			SourceZones.Clear();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var zoneViewModel in TargetZones)
			{
				SourceZones.Add(zoneViewModel);
			}
			TargetZones.Clear();
		}

		bool CanAdd()
		{
			return SelectedSourceZone != null;
		}

		bool CanRemove()
		{
			return SelectedTargetZone != null;
		}

		protected override bool Save()
		{
            Zones = new List<Guid>(TargetZones.Select(x => x.Zone.UID));
			return base.Save();
		}
	}
}