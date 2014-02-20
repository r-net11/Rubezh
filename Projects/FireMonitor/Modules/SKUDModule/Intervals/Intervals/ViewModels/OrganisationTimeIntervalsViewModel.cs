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
	public class OrganisationTimeIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		EmployeeTimeInterval IntervalToCopy;

		public OrganisationTimeIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(string name, List<EmployeeTimeInterval> employeeTimeIntervals)
		{
			Name = name;

			var neverTimeInterval = employeeTimeIntervals.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			if (neverTimeInterval == null)
			{
				neverTimeInterval = new EmployeeTimeInterval() { Name = "Никогда", IsDefault = true };
				neverTimeInterval.TimeIntervalParts.Add(new EmployeeTimeIntervalPart() { StartTime = DateTime.MinValue, EndTime = DateTime.MinValue });
				employeeTimeIntervals.Add(neverTimeInterval);
			}

			TimeIntervals = new ObservableCollection<TimeIntervalViewModel>();
			foreach (var timeInterval in employeeTimeIntervals)
			{
				var timeInrervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeInrervalViewModel);
			}
			SelectedTimeInterval = TimeIntervals.FirstOrDefault();
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		ObservableCollection<TimeIntervalViewModel> _timeIntervals;
		public ObservableCollection<TimeIntervalViewModel> TimeIntervals
		{
			get { return _timeIntervals; }
			set
			{
				_timeIntervals = value;
				OnPropertyChanged("TimeIntervals");
			}
		}

		TimeIntervalViewModel _selectedTimeInterval;
		public TimeIntervalViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
			}
		}

		public void Select(Guid timeIntervalUID)
		{
			if (timeIntervalUID != Guid.Empty)
			{
				var timeIntervalViewModel = TimeIntervals.FirstOrDefault(x => x.TimeInterval.UID == timeIntervalUID);
				if (timeIntervalViewModel != null)
				{
					SelectedTimeInterval = timeIntervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeIntervalDetailsViewModel = new TimeIntervalDetailsViewModel(this);
			if (DialogService.ShowModalWindow(timeIntervalDetailsViewModel))
			{
				var timeIntervalViewModel = new TimeIntervalViewModel(timeIntervalDetailsViewModel.TimeInterval);
				TimeIntervals.Add(timeIntervalViewModel);
				SelectedTimeInterval = timeIntervalViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			TimeIntervals.Remove(SelectedTimeInterval);
		}
		bool CanDelete()
		{
			return SelectedTimeInterval != null && !SelectedTimeInterval.TimeInterval.IsDefault;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var timeInrervalDetailsViewModel = new TimeIntervalDetailsViewModel(this, SelectedTimeInterval.TimeInterval);
			if (DialogService.ShowModalWindow(timeInrervalDetailsViewModel))
			{
				SelectedTimeInterval.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedTimeInterval != null && !SelectedTimeInterval.TimeInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedTimeInterval.TimeInterval);
		}
		bool CanCopy()
		{
			return SelectedTimeInterval != null && !SelectedTimeInterval.TimeInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			var timeInrervalViewModel = new TimeIntervalViewModel(newInterval);
			TimeIntervals.Add(timeInrervalViewModel);
			SelectedTimeInterval = timeInrervalViewModel;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null;
		}

		EmployeeTimeInterval CopyInterval(EmployeeTimeInterval source)
		{
			var copy = new EmployeeTimeInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalPart in source.TimeIntervalParts)
			{
				var copyTimeIntervalPart = new EmployeeTimeIntervalPart()
				{
					StartTime = timeIntervalPart.StartTime,
					EndTime = timeIntervalPart.EndTime
				};
				copy.TimeIntervalParts.Add(copyTimeIntervalPart);
			}
			return copy;
		}
	}
}