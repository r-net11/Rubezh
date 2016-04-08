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
using RubezhAPI;
using Common;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class PumpStationsSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<GKPumpStation> PumpStations { get; private set; }

		public PumpStationsSelectationViewModel(List<GKPumpStation> pumpStations)
		{
			Title = "Выбор насосных станций";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			PumpStations = pumpStations;
			TargetPumpStations = new SortableObservableCollection<GKPumpStation>();
			SourcePumpStations = new SortableObservableCollection<GKPumpStation>();

			foreach (var pumpStation in GKManager.PumpStations)
			{
				if (PumpStations.Contains(pumpStation))
					TargetPumpStations.Add(pumpStation);
				else
					SourcePumpStations.Add(pumpStation);
			}

			SelectedTargetPumpStation = TargetPumpStations.FirstOrDefault();
			SelectedSourcePumpStation = SourcePumpStations.FirstOrDefault();
		}

		public SortableObservableCollection<GKPumpStation> SourcePumpStations { get; private set; }

		GKPumpStation _selectedSourcePumpStation;
		public GKPumpStation SelectedSourcePumpStation
		{
			get { return _selectedSourcePumpStation; }
			set
			{
				_selectedSourcePumpStation = value;
				OnPropertyChanged(() => SelectedSourcePumpStation);
			}
		}

		public SortableObservableCollection<GKPumpStation> TargetPumpStations { get; private set; }

		GKPumpStation _selectedTargetPumpStation;
		public GKPumpStation SelectedTargetPumpStation
		{
			get { return _selectedTargetPumpStation; }
			set
			{
				_selectedTargetPumpStation = value;
				OnPropertyChanged(() => SelectedTargetPumpStation);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourcePumpStations;
		void OnAdd(object parameter)
		{
			var index = SourcePumpStations.IndexOf(SelectedSourcePumpStation);

			SelectedSourcePumpStations = (IList)parameter;
			var SourcePumpStationViewModels = new List<GKPumpStation>();
			foreach (var SourcePumpStation in SelectedSourcePumpStations)
			{
				var SourcePumpStationViewModel = SourcePumpStation as GKPumpStation;
				if (SourcePumpStationViewModel != null)
					SourcePumpStationViewModels.Add(SourcePumpStationViewModel);

			}
			foreach (var SourcePumpStationViewModel in SourcePumpStationViewModels)
			{
				TargetPumpStations.Add(SourcePumpStationViewModel);
				SourcePumpStations.Remove(SourcePumpStationViewModel);
			}
			TargetPumpStations.Sort(x => x.No);
			SelectedTargetPumpStation = TargetPumpStations.LastOrDefault();
			OnPropertyChanged(() => SourcePumpStations);

			index = Math.Min(index, SourcePumpStations.Count - 1);
			if (index > -1)
				SelectedSourcePumpStation = SourcePumpStations[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetPumpStations;
		void OnRemove(object parameter)
		{
			var index = TargetPumpStations.IndexOf(SelectedTargetPumpStation);

			SelectedTargetPumpStations = (IList)parameter;
			var TargetPumpStationViewModels = new List<GKPumpStation>();
			foreach (var TargetPumpStation in SelectedTargetPumpStations)
			{
				var TargetPumpStationViewModel = TargetPumpStation as GKPumpStation;
				if (TargetPumpStationViewModel != null)
					TargetPumpStationViewModels.Add(TargetPumpStationViewModel);
			}
			foreach (var TargetPumpStationViewModel in TargetPumpStationViewModels)
			{
				SourcePumpStations.Add(TargetPumpStationViewModel);
				TargetPumpStations.Remove(TargetPumpStationViewModel);
			}
			SourcePumpStations.Sort(x => x.No);
			SelectedSourcePumpStation = SourcePumpStations.LastOrDefault();
			OnPropertyChanged(() => TargetPumpStations);

			index = Math.Min(index, TargetPumpStations.Count - 1);
			if (index > -1)
				SelectedTargetPumpStation = TargetPumpStations[index];
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourcePumpStation != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetPumpStation != null;
		}
		
		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var zoneViewModel in SourcePumpStations)
			{
				TargetPumpStations.Add(zoneViewModel);
			}
			TargetPumpStations.Sort(x => x.No);
			SourcePumpStations.Clear();
			SelectedTargetPumpStation = TargetPumpStations.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var zoneViewModel in TargetPumpStations)
			{
				SourcePumpStations.Add(zoneViewModel);
			}
			SourcePumpStations.Sort(x => x.No);
			TargetPumpStations.Clear();
			SelectedSourcePumpStation = SourcePumpStations.FirstOrDefault();
		}
		public bool CanAddAll()
		{
			return (SourcePumpStations.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetPumpStations.Count > 0);
		}
		protected override bool Save()
		{
			PumpStations = new List<GKPumpStation>(TargetPumpStations);
			return base.Save();
		}
	}
}