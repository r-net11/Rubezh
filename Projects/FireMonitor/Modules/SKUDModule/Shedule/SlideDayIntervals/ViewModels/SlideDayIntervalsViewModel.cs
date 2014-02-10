using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public SlideDayIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			Initialize();
		}

		public void Initialize()
		{
			SlideDayIntervals = new ObservableCollection<SlideDayIntervalViewModel>();
			var employeeSlideDayIntervals = new List<EmployeeSlideDayInterval>();
			foreach (var slideDayInterval in employeeSlideDayIntervals)
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideDayInterval = SlideDayIntervals.FirstOrDefault();
		}

		EmployeeSlideDayInterval IntervalToCopy;

		ObservableCollection<SlideDayIntervalViewModel> _slideDayIntervals;
		public ObservableCollection<SlideDayIntervalViewModel> SlideDayIntervals
		{
			get { return _slideDayIntervals; }
			set
			{
				_slideDayIntervals = value;
				OnPropertyChanged("SlideDayIntervals");
			}
		}

		SlideDayIntervalViewModel _selectedSlideDayInterval;
		public SlideDayIntervalViewModel SelectedSlideDayInterval
		{
			get { return _selectedSlideDayInterval; }
			set
			{
				_selectedSlideDayInterval = value;
				OnPropertyChanged("SelectedSlideDayInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = SlideDayIntervals.FirstOrDefault(x => x.SlideDayInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedSlideDayInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayIntervalDetailsViewModel.SlideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
				SelectedSlideDayInterval = timeInrervalViewModel;
			}
		}
		bool CanAdd()
		{
			return SlideDayIntervals.Count < 256;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SlideDayIntervals.Remove(SelectedSlideDayInterval);
		}
		bool CanDelete()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(SelectedSlideDayInterval.SlideDayInterval);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				SelectedSlideDayInterval.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideDayInterval.SlideDayInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideDayInterval != null && !SelectedSlideDayInterval.SlideDayInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			var timeInrervalViewModel = new SlideDayIntervalViewModel(newInterval);
			SlideDayIntervals.Add(timeInrervalViewModel);
			SelectedSlideDayInterval = timeInrervalViewModel;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && SlideDayIntervals.Count < 256;
		}

		EmployeeSlideDayInterval CopyInterval(EmployeeSlideDayInterval source)
		{
			var copy = new EmployeeSlideDayInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalUID in source.TimeIntervalUIDs)
			{
				copy.TimeIntervalUIDs.Add(timeIntervalUID);
			}
			return copy;
		}
	}
}