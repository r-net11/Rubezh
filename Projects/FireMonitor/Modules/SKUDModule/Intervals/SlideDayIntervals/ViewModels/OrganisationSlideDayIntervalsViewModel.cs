using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Organisation = FiresecAPI.Organisation;

namespace SKDModule.ViewModels
{
	public class OrganisationSlideDayIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Organisation Organization { get; private set; }
		ScheduleScheme IntervalToCopy;

		public OrganisationSlideDayIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(Organisation organization, List<ScheduleScheme> employeeSlideDayIntervals)
		{
			Organization = organization;

			//var neverTimeInterval = employeeSlideDayIntervals.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			//if (neverTimeInterval == null)
			//{
			//    neverTimeInterval = new EmployeeSlideDayInterval() { Name = "Никогда", IsDefault = true };
			//    //neverTimeInterval.TimeIntervalUIDs.Add(Guid.Empty);
			//    employeeSlideDayIntervals.Add(neverTimeInterval);
			//}

			SlideDayIntervals = new ObservableCollection<SlideDayIntervalViewModel>();
			foreach (var slideDayInterval in employeeSlideDayIntervals)
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
			}
			SelectedSlideDayInterval = SlideDayIntervals.FirstOrDefault();
		}

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
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(this);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				var timeInrervalViewModel = new SlideDayIntervalViewModel(slideDayIntervalDetailsViewModel.SlideDayInterval);
				SlideDayIntervals.Add(timeInrervalViewModel);
				SelectedSlideDayInterval = timeInrervalViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SlideDayIntervals.Remove(SelectedSlideDayInterval);
		}
		bool CanDelete()
		{
			return SelectedSlideDayInterval != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(this, SelectedSlideDayInterval.SlideDayInterval);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				SelectedSlideDayInterval.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedSlideDayInterval != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedSlideDayInterval.SlideDayInterval);
		}
		bool CanCopy()
		{
			return SelectedSlideDayInterval != null;
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
			return IntervalToCopy != null;
		}

		ScheduleScheme CopyInterval(ScheduleScheme source)
		{
			var copy = new ScheduleScheme();
			copy.Name = source.Name;
			foreach (var timeIntervalUID in source.DayIntervals)
			{
				copy.DayIntervals.Add(timeIntervalUID);
			}
			return copy;
		}
	}
}