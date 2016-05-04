using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using GKModule.Events;
using Infrastructure;
using RubezhAPI;
using Common;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class GuardZonesSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<GKGuardZone> GuardZones { get; private set; }

		public GuardZonesSelectationViewModel(List<GKGuardZone> guardZones)
		{
			Title = "Выбор охранных зон";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
			CreateNewCommand = new RelayCommand(OnCreateNew);

			GuardZones = guardZones;
			TargetZones = new SortableObservableCollection<GKGuardZone>();
			SourceZones = new SortableObservableCollection<GKGuardZone>();

			foreach (var guardZone in GKManager.GuardZones)
			{
				if (GuardZones.Contains(guardZone))
					TargetZones.Add(guardZone);
				else
					SourceZones.Add(guardZone);
			}
			TargetZones.Sort(x => x.No);
			SourceZones.Sort(x => x.No);

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public SortableObservableCollection<GKGuardZone> SourceZones { get; private set; }

		GKGuardZone _selectedSourceZone;
		public GKGuardZone SelectedSourceZone
		{
			get { return _selectedSourceZone; }
			set
			{
				_selectedSourceZone = value;
				OnPropertyChanged(() => SelectedSourceZone);
			}
		}

		public SortableObservableCollection<GKGuardZone> TargetZones { get; private set; }

		GKGuardZone _selectedTargetZone;
		public GKGuardZone SelectedTargetZone
		{
			get { return _selectedTargetZone; }
			set
			{
				_selectedTargetZone = value;
				OnPropertyChanged(() => SelectedTargetZone);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceZones;
		void OnAdd(object parameter)
		{
			var index = SourceZones.IndexOf(SelectedSourceZone);

			SelectedSourceZones = (IList)parameter;
			var zoneViewModels = new List<GKGuardZone>();
			foreach (var selectedZone in SelectedSourceZones)
			{
				var zoneViewModel = selectedZone as GKGuardZone;
				if (zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				TargetZones.Add(zoneViewModel);
				SourceZones.Remove(zoneViewModel);
			}
			TargetZones.Sort(x => x.No);
			SelectedTargetZone = TargetZones.LastOrDefault();
			OnPropertyChanged(() => SourceZones);

			index = Math.Min(index, SourceZones.Count - 1);
			if (index > -1)
				SelectedSourceZone = SourceZones[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetZones;
		void OnRemove(object parameter)
		{
			var index = TargetZones.IndexOf(SelectedTargetZone);

			SelectedTargetZones = (IList)parameter;
			var zoneViewModels = new List<GKGuardZone>();
			foreach (var selectedZone in SelectedTargetZones)
			{
				var zoneViewModel = selectedZone as GKGuardZone;
				if (zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				SourceZones.Add(zoneViewModel);
				TargetZones.Remove(zoneViewModel);
			}
			SourceZones.Sort(x => x.No);
			SelectedSourceZone = SourceZones.LastOrDefault();
			OnPropertyChanged(() => TargetZones);

			index = Math.Min(index, TargetZones.Count - 1);
			if (index > -1)
				SelectedTargetZone = TargetZones[index];
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var zoneViewModel in SourceZones)
			{
				TargetZones.Add(zoneViewModel);
			}
			TargetZones.Sort(x => x.No);
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
			SourceZones.Sort(x => x.No);
			TargetZones.Clear();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public RelayCommand CreateNewCommand { get; private set; }
		void OnCreateNew()
		{
			var createGuardZoneEventArg = new CreateGKGuardZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Publish(createGuardZoneEventArg);
			if (createGuardZoneEventArg.Zone != null)
			{
				SourceZones.Add(createGuardZoneEventArg.Zone);
			}
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceZone != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetZone != null;
		}

		public bool CanAddAll()
		{
			return (SourceZones.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetZones.Count > 0);
		}

		protected override bool Save()
		{
			GuardZones = new List<GKGuardZone>(TargetZones);
			return base.Save();
		}
	}
}