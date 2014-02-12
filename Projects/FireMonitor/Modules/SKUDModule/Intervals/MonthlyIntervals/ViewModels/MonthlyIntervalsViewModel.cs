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
	public class MonthlyIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public MonthlyIntervalsViewModel()
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
			var employeeMonthlyIntervals = new List<EmployeeMonthlyInterval>();
			var neverTimeInterval = employeeMonthlyIntervals.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			if (neverTimeInterval == null)
			{
				neverTimeInterval = new EmployeeMonthlyInterval() { Name = "Никогда", IsDefault = true };
				employeeMonthlyIntervals.Add(neverTimeInterval);
			}

			MonthlyIntervals = new ObservableCollection<MonthlyIntervalViewModel>();
			foreach (var monthlyInterval in employeeMonthlyIntervals)
			{
				var timeInrervalViewModel = new MonthlyIntervalViewModel(monthlyInterval);
				MonthlyIntervals.Add(timeInrervalViewModel);
			}
			SelectedMonthlyInterval = MonthlyIntervals.FirstOrDefault();
		}

		EmployeeMonthlyInterval IntervalToCopy;

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
			return SelectedMonthlyInterval != null && !SelectedMonthlyInterval.MonthlyInterval.IsDefault;
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
			return SelectedMonthlyInterval != null && !SelectedMonthlyInterval.MonthlyInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedMonthlyInterval.MonthlyInterval);
		}
		bool CanCopy()
		{
			return SelectedMonthlyInterval != null && !SelectedMonthlyInterval.MonthlyInterval.IsDefault;
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

		EmployeeMonthlyInterval CopyInterval(EmployeeMonthlyInterval source)
		{
			var copy = new EmployeeMonthlyInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalUID in source.MonthlyIntervalParts)
			{
				copy.MonthlyIntervalParts.Add(timeIntervalUID);
			}
			return copy;
		}
	}
}