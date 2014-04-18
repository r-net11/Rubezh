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
	public class OrganisationMonthlyIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Organisation Organization { get; private set; }
		ScheduleScheme IntervalToCopy;

		public OrganisationMonthlyIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(Organisation organization, List<ScheduleScheme> ScheduleSchemes)
		{
			Organization = organization;

			//var neverTimeInterval = ScheduleSchemes.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			//if (neverTimeInterval == null)
			//{
			//    neverTimeInterval = new ScheduleScheme() { Name = "Никогда", IsDefault = true };
			//    ScheduleSchemes.Add(neverTimeInterval);
			//}

			MonthlyIntervals = new ObservableCollection<MonthlyIntervalViewModel>();
			foreach (var monthlyInterval in ScheduleSchemes)
			{
				var timeInrervalViewModel = new MonthlyIntervalViewModel(monthlyInterval);
				MonthlyIntervals.Add(timeInrervalViewModel);
			}
			SelectedMonthlyInterval = MonthlyIntervals.FirstOrDefault();
		}

		ObservableCollection<MonthlyIntervalViewModel> _monthlyIntervals;
		public ObservableCollection<MonthlyIntervalViewModel> MonthlyIntervals
		{
			get { return _monthlyIntervals; }
			set
			{
				_monthlyIntervals = value;
				OnPropertyChanged("MonthlyIntervals");
			}
		}

		MonthlyIntervalViewModel _selectedMonthlyInterval;
		public MonthlyIntervalViewModel SelectedMonthlyInterval
		{
			get { return _selectedMonthlyInterval; }
			set
			{
				_selectedMonthlyInterval = value;
				OnPropertyChanged("SelectedMonthlyInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = MonthlyIntervals.FirstOrDefault(x => x.MonthlyInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedMonthlyInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var monthlyIntervalDetailsViewModel = new MonthlyIntervalDetailsViewModel(this);
			if (DialogService.ShowModalWindow(monthlyIntervalDetailsViewModel))
			{
				var timeInrervalViewModel = new MonthlyIntervalViewModel(monthlyIntervalDetailsViewModel.MonthlyInterval);
				MonthlyIntervals.Add(timeInrervalViewModel);
				SelectedMonthlyInterval = timeInrervalViewModel;
			}
		}
		bool CanAdd()
		{
			return MonthlyIntervals.Count < 256;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			MonthlyIntervals.Remove(SelectedMonthlyInterval);
		}
		bool CanDelete()
		{
			return SelectedMonthlyInterval != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var monthlyIntervalDetailsViewModel = new MonthlyIntervalDetailsViewModel(this, SelectedMonthlyInterval.MonthlyInterval);
			if (DialogService.ShowModalWindow(monthlyIntervalDetailsViewModel))
			{
				SelectedMonthlyInterval.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedMonthlyInterval != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedMonthlyInterval.MonthlyInterval);
		}
		bool CanCopy()
		{
			return SelectedMonthlyInterval != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			var timeInrervalViewModel = new MonthlyIntervalViewModel(newInterval);
			MonthlyIntervals.Add(timeInrervalViewModel);
			SelectedMonthlyInterval = timeInrervalViewModel;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null && MonthlyIntervals.Count < 256;
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