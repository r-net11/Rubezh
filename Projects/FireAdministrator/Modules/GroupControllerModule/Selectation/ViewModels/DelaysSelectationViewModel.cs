using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DelaysSelectationViewModel : SaveCancelDialogViewModel
	{
		public List<XDelay> Delays { get; private set; }
		public bool CanCreateNew { get; private set; }

		public DelaysSelectationViewModel(List<XDelay> delays, bool canCreateNew = false)
		{
			Title = "Выбор задержек";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			Delays = delays;
			CanCreateNew = canCreateNew;
			TargetDelays = new ObservableCollection<XDelay>();
			SourceDelays = new ObservableCollection<XDelay>();

			foreach (var delay in XManager.DeviceConfiguration.Delays)
			{
				if (Delays.Contains(delay))
					TargetDelays.Add(delay);
				else
					SourceDelays.Add(delay);
			}

			SelectedTargetDelay = TargetDelays.FirstOrDefault();
			SelectedSourceDelay = SourceDelays.FirstOrDefault();
		}

		public ObservableCollection<XDelay> SourceDelays { get; private set; }

		XDelay _selectedSourceDelay;
		public XDelay SelectedSourceDelay
		{
			get { return _selectedSourceDelay; }
			set
			{
				_selectedSourceDelay = value;
				OnPropertyChanged("SelectedSourceDelay");
			}
		}

		public ObservableCollection<XDelay> TargetDelays { get; private set; }

		XDelay _selectedTargetDelay;
		public XDelay SelectedTargetDelay
		{
			get { return _selectedTargetDelay; }
			set
			{
				_selectedTargetDelay = value;
				OnPropertyChanged("SelectedTargetDelay");
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceDelays;
		void OnAdd(object parameter)
		{
			var index = SourceDelays.IndexOf(SelectedSourceDelay);

			SelectedSourceDelays = (IList)parameter;
			var delayViewModels = new List<XDelay>();
			foreach (var selectedDelay in SelectedSourceDelays)
			{
				var delayViewModel = selectedDelay as XDelay;
				if (delayViewModel != null)
					delayViewModels.Add(delayViewModel);
			}
			foreach (var delayViewModel in delayViewModels)
			{
				TargetDelays.Add(delayViewModel);
				SourceDelays.Remove(delayViewModel);
			}
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
			var delayViewModels = new List<XDelay>();
			foreach (var selectedDelay in SelectedTargetDelays)
			{
				var delayViewModel = selectedDelay as XDelay;
				if (delayViewModel != null)
					delayViewModels.Add(delayViewModel);
			}
			foreach (var delayViewModel in delayViewModels)
			{
				SourceDelays.Add(delayViewModel);
				TargetDelays.Remove(delayViewModel);
			}
			SelectedSourceDelay = SourceDelays.LastOrDefault();
			OnPropertyChanged("TargetDelays");

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
			Delays = new List<XDelay>(TargetDelays);
			return base.Save();
		}
	}
}