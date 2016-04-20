using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;
using Common;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DelaysSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<GKDelay> Delays { get; private set; }

		public DelaysSelectationViewModel(List<GKDelay> delays)
		{
			Title = "Выбор задержек";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			Delays = delays;
			TargetDelays = new SortableObservableCollection<GKDelay>();
			SourceDelays = new SortableObservableCollection<GKDelay>();

			foreach (var delay in GKManager.DeviceConfiguration.Delays)
			{
				if (Delays.Contains(delay))
					TargetDelays.Add(delay);
				else
					SourceDelays.Add(delay);
			}
			TargetDelays.Sort(x => x.No);
			SourceDelays.Sort(x => x.No);

			SelectedTargetDelay = TargetDelays.FirstOrDefault();
			SelectedSourceDelay = SourceDelays.FirstOrDefault();
		}

		public SortableObservableCollection<GKDelay> SourceDelays { get; private set; }

		GKDelay _selectedSourceDelay;
		public GKDelay SelectedSourceDelay
		{
			get { return _selectedSourceDelay; }
			set
			{
				_selectedSourceDelay = value;
				OnPropertyChanged(() => SelectedSourceDelay);
			}
		}

		public SortableObservableCollection<GKDelay> TargetDelays { get; private set; }

		GKDelay _selectedTargetDelay;
		public GKDelay SelectedTargetDelay
		{
			get { return _selectedTargetDelay; }
			set
			{
				_selectedTargetDelay = value;
				OnPropertyChanged(() => SelectedTargetDelay);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceDelays;
		void OnAdd(object parameter)
		{
			var index = SourceDelays.IndexOf(SelectedSourceDelay);

			SelectedSourceDelays = (IList)parameter;
			var delayViewModels = new List<GKDelay>();
			foreach (var selectedDelay in SelectedSourceDelays)
			{
				var delayViewModel = selectedDelay as GKDelay;
				if (delayViewModel != null)
					delayViewModels.Add(delayViewModel);
			}
			foreach (var delayViewModel in delayViewModels)
			{
				TargetDelays.Add(delayViewModel);
				SourceDelays.Remove(delayViewModel);
			}
			TargetDelays.Sort(x => x.No);
			SelectedTargetDelay = TargetDelays.LastOrDefault();
			OnPropertyChanged("SourceDelays");

			index = Math.Min(index, SourceDelays.Count - 1);
			if (index > -1)
				SelectedSourceDelay = SourceDelays[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetDelays;
		void OnRemove(object parameter)
		{
			var index = TargetDelays.IndexOf(SelectedTargetDelay);

			SelectedTargetDelays = (IList)parameter;
			var delayViewModels = new List<GKDelay>();
			foreach (var selectedDelay in SelectedTargetDelays)
			{
				var delayViewModel = selectedDelay as GKDelay;
				if (delayViewModel != null)
					delayViewModels.Add(delayViewModel);
			}
			foreach (var delayViewModel in delayViewModels)
			{
				SourceDelays.Add(delayViewModel);
				TargetDelays.Remove(delayViewModel);
			}
			SourceDelays.Sort(x => x.No);
			SelectedSourceDelay = SourceDelays.LastOrDefault();
			OnPropertyChanged(() => TargetDelays);

			index = Math.Min(index, TargetDelays.Count - 1);
			if (index > -1)
				SelectedTargetDelay = TargetDelays[index];
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var delayViewModel in SourceDelays)
			{
				TargetDelays.Add(delayViewModel);
			}
			TargetDelays.Sort(x => x.No);
			SourceDelays.Clear();
			SelectedTargetDelay = TargetDelays.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var delayViewModel in TargetDelays)
			{
				SourceDelays.Add(delayViewModel);
			}
			SourceDelays.Sort(x => x.No);
			TargetDelays.Clear();
			SelectedSourceDelay = SourceDelays.FirstOrDefault();
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceDelay != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedTargetDelay != null;
		}

		public bool CanAddAll()
		{
			return (SourceDelays.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetDelays.Count > 0);
		}

		protected override bool Save()
		{
			Delays = new List<GKDelay>(TargetDelays);
			return base.Save();
		}
	}
}