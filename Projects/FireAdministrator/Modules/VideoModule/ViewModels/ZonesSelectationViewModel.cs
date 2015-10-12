﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class ZonesSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<Guid> Zones { get; private set; }

		public ZonesSelectationViewModel(List<Guid> zones)
		{
			Title = "Выбор зон";
			AddOneCommand = new RelayCommand(OnAddOne, CanAdd);
			RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

			Zones = zones;
			TargetZones = new ObservableCollection<GKZone>();
			SourceZones = new ObservableCollection<GKZone>();

			var sortedZones = GKManager.Zones.OrderBy(x => { return x.No; });
			foreach (var zone in sortedZones)
			{
				if (Zones.Contains(zone.UID))
					TargetZones.Add(zone);
				else
					SourceZones.Add(zone);
			}

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public ObservableCollection<GKZone> SourceZones { get; private set; }

		GKZone _selectedSourceZone;
		public GKZone SelectedSourceZone
		{
			get { return _selectedSourceZone; }
			set
			{
				_selectedSourceZone = value;
				OnPropertyChanged(() => SelectedSourceZone);
			}
		}

		public ObservableCollection<GKZone> TargetZones { get; private set; }

		GKZone _selectedTargetZone;
		public GKZone SelectedTargetZone
		{
			get { return _selectedTargetZone; }
			set
			{
				_selectedTargetZone = value;
				OnPropertyChanged(() => SelectedTargetZone);
			}
		}

		public RelayCommand AddOneCommand { get; private set; }
		void OnAddOne()
		{
			TargetZones.Add(SelectedSourceZone);
			SelectedTargetZone = SelectedSourceZone;
			SourceZones.Remove(SelectedSourceZone);
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public RelayCommand RemoveOneCommand { get; private set; }
		void OnRemoveOne()
		{
			SourceZones.Add(SelectedTargetZone);
			SelectedSourceZone = SelectedTargetZone;
			TargetZones.Remove(SelectedTargetZone);
			SelectedTargetZone = TargetZones.FirstOrDefault();
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var zoneViewModel in SourceZones)
			{
				TargetZones.Add(zoneViewModel);
			}
			SourceZones.Clear();
			SelectedTargetZone = TargetZones.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var zoneViewModel in TargetZones)
			{
				SourceZones.Add(zoneViewModel);
			}
			TargetZones.Clear();
			SelectedSourceZone = SourceZones.FirstOrDefault();
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
			Zones = new List<Guid>(TargetZones.Select(x => x.UID));
			return base.Save();
		}
	}
}