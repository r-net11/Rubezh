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
	public class SlideWeekIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public SlideWeekIntervalsViewModel()
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
			SlideWeekIntervals = new ObservableCollection<SlideWeekIntervalViewModel>();
			var employeeSlideWeeklyIntervals = new List<EmployeeSlideWeeklyInterval>();
			foreach (var slideWeekInterval in employeeSlideWeeklyIntervals)
			{
				var timeInrervalViewModel = new SlideWeekIntervalViewModel(slideWeekInterval);
				SlideWeekIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideWeekInterval = SlideWeekIntervals.FirstOrDefault();
		}

		EmployeeSlideWeeklyInterval IntervalToCopy;

		ObservableCollection<SlideWeekIntervalViewModel> _slideWeekIntervals;
		public ObservableCollection<SlideWeekIntervalViewModel> SlideWeekIntervals
		{
			get { return _slideWeekIntervals; }
			set
			{
				_slideWeekIntervals = value;
				OnPropertyChanged("SlideWeekIntervals");
			}
		}

		SlideWeekIntervalViewModel _selectedSlideWeekInterval;
		public SlideWeekIntervalViewModel SelectedSlideWeekInterval
		{
			get { return _selectedSlideWeekInterval; }
			set
			{
				_selectedSlideWeekInterval = value;
				OnPropertyChanged("SelectedSlideWeekInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = SlideWeekIntervals.FirstOrDefault(x => x.SlideWeekInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedSlideWeekInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var slideWeekIntervalDetailsViewModel = new SlideWeekIntervalDetailsViewModel();
			if (DialogService.ShowModalWindow(slideWeekIntervalDetailsViewModel))
			{
				var timeInrervalViewModel = new SlideWeekIntervalViewModel(slideWeekIntervalDetailsViewModel.SlideWeekInterval);
				SlideWeekIntervals.Add(timeInrervalViewModel);
				SelectedSlideWeekInterval = timeInrervalViewModel;
			}
		}
		bool CanAdd()
		{
			return SlideWeekIntervals.Count < 256;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SlideWeekIntervals.Remove(SelectedSlideWeekInterval);
		}
		bool CanDelete()
		{
			return SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.SlideWeekInterval.IsDefault;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var slideWeekIntervalDetailsViewModel = new SlideWeekIntervalDetailsViewModel(SelectedSlideWeekInterval.SlideWeekInterval);
			if (DialogService.ShowModalWindow(slideWeekIntervalDetailsViewModel))
			{
				SelectedSlideWeekInterval.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.SlideWeekInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideWeekInterval.SlideWeekInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideWeekInterval != null && !SelectedSlideWeekInterval.SlideWeekInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			var timeInrervalViewModel = new SlideWeekIntervalViewModel(newInterval);
			SlideWeekIntervals.Add(timeInrervalViewModel);
			SelectedSlideWeekInterval = timeInrervalViewModel;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && SlideWeekIntervals.Count < 256;
		}

		EmployeeSlideWeeklyInterval CopyInterval(EmployeeSlideWeeklyInterval source)
		{
			var copy = new EmployeeSlideWeeklyInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalUID in source.WeeklyIntervalUIDs)
			{
				copy.WeeklyIntervalUIDs.Add(timeIntervalUID);
			}
			return copy;
		}
	}
}