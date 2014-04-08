using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Organization = FiresecAPI.Organization;

namespace SKDModule.ViewModels
{
	public class OrganisationTimeIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Organization Organization { get; private set; }
		NamedInterval IntervalToCopy;

		public OrganisationTimeIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(Organization organization, List<NamedInterval> employeeTimeIntervals)
		{
			Organization = organization;

			//var neverTimeInterval = employeeTimeIntervals.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			//if (neverTimeInterval == null)
			//{
			//    neverTimeInterval = new NamedInterval() { Name = "Никогда", IsDefault = true };
			//    neverTimeInterval.TimeIntervals.Add(new TimeInterval() { StartTime = DateTime.MinValue, EndTime = DateTime.MinValue });
			//    employeeTimeIntervals.Add(neverTimeInterval);
			//}

			TimeIntervals = new ObservableCollection<TimeIntervalViewModel>();
			foreach (var timeInterval in employeeTimeIntervals)
			{
				var timeInrervalViewModel = new TimeIntervalViewModel(timeInterval);
				TimeIntervals.Add(timeInrervalViewModel);
			}
			SelectedTimeInterval = TimeIntervals.FirstOrDefault();
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

		NamedInterval CopyInterval(NamedInterval source)
		{
			var copy = new NamedInterval();
			copy.Name = source.Name;
			foreach (var timeIntervalPart in source.TimeIntervals)
			{
				var copyTimeIntervalPart = new TimeInterval()
				{
					StartTime = timeIntervalPart.StartTime,
					EndTime = timeIntervalPart.EndTime
				};
				copy.TimeIntervals.Add(copyTimeIntervalPart);
			}
			return copy;
		}
	}
}