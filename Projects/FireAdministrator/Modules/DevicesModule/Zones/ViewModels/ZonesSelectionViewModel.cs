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

				//if ((zoneLogicState == ZoneLogicState.MPTAutomaticOn) || (zoneLogicState == ZoneLogicState.MPTOn) || (zoneLogicState == ZoneLogicState.Firefighting))
				//{
                //    if (device.ParentPanel.Children.Any(x => x.Driver.DriverType == DriverType.MPT && x.ZoneUID == zone.UID) == false)
				//    {
				//        continue;
				//    }
				//}

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
			var index = SourceZones.IndexOf(SelectedSourceZone);
			TargetZones.Add(SelectedSourceZone);
			SelectedTargetZone = SelectedSourceZone;
			SourceZones.Remove(SelectedSourceZone);
			if (SourceZones.Count > 0)
				SelectedSourceZone = SourceZones[Math.Min(index, SourceZones.Count - 1)];
		}

		public RelayCommand RemoveOneCommand { get; private set; }
		void OnRemoveOne()
		{
			var index = TargetZones.IndexOf(SelectedTargetZone);
			SourceZones.Add(SelectedTargetZone);
			SelectedSourceZone = SelectedTargetZone;
			TargetZones.Remove(SelectedTargetZone);
			SelectedTargetZone = TargetZones.FirstOrDefault();
			if (TargetZones.Count > 0)
				SelectedTargetZone = TargetZones[Math.Min(index, TargetZones.Count - 1)];
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