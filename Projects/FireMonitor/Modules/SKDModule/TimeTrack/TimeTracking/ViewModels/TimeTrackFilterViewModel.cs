using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Localization.SKD.ViewModels;
using SKDModule.Model;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class TimeTrackFilterViewModel : OrganisationFilterBaseViewModel<TimeTrackFilter>
	{
		public EmployeesFilterViewModel EmployeeFilterViewModel { get; private set; }
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public TimeTrackFilter TimeTrackFilter { get; private set; }

		public TimeTrackFilterViewModel(TimeTrackFilter filter)
			: base(filter, false)
		{
			TimeTrackFilter = filter;
			ResetCommand = new RelayCommand(OnReset);
			InitializeFilter(filter);
		}

		void InitializeFilter(TimeTrackFilter timeTrackFilter)
		{
			EmployeeFilterViewModel = new EmployeesFilterViewModel();
			EmployeeFilterViewModel.Initialize(timeTrackFilter.EmployeeFilter);
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel();
			DepartmentsFilterViewModel.Initialize(new DepartmentFilter { UIDs = Filter.EmployeeFilter.DepartmentUIDs });
			PositionsFilterViewModel = new PositionsFilterViewModel();
			PositionsFilterViewModel.Initialize(new PositionFilter { UIDs = Filter.EmployeeFilter.PositionUIDs });

			Periods = new ObservableCollection<TimeTrackingPeriod>(Enum.GetValues(typeof(TimeTrackingPeriod)).OfType<TimeTrackingPeriod>());
			Period = timeTrackFilter.Period;
			StartDate = timeTrackFilter.StartDate;
			EndDate = timeTrackFilter.EndDate;

			Totals = new ObservableCollection<TimeTrackTypeFilterItem>
			{
				new TimeTrackTypeFilterItem(TimeTrackType.Balance),
				new TimeTrackTypeFilterItem(TimeTrackType.Presence),
				new TimeTrackTypeFilterItem(TimeTrackType.Late),
				new TimeTrackTypeFilterItem(TimeTrackType.Absence),
				new TimeTrackTypeFilterItem(TimeTrackType.EarlyLeave),
				new TimeTrackTypeFilterItem(TimeTrackType.Overtime),
				new TimeTrackTypeFilterItem(TimeTrackType.Night),
				new TimeTrackTypeFilterItem(TimeTrackType.DocumentPresence),
				new TimeTrackTypeFilterItem(TimeTrackType.DocumentAbsenceReasonable),
				new TimeTrackTypeFilterItem(TimeTrackType.DocumentAbsence),
				new TimeTrackTypeFilterItem(TimeTrackType.DocumentOvertime)
			};

			foreach (var totalTimeTrackTypeFilter in timeTrackFilter.TotalTimeTrackTypeFilters)
			{
				var timeTrackTypeFilterItem = Totals.FirstOrDefault(x => x.TimeTrackType == totalTimeTrackTypeFilter);
				if (timeTrackTypeFilterItem != null)
					timeTrackTypeFilterItem.IsChecked = true;
			}
		}

		DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged(() => StartDate);
				if (IsFreePeriod && value.AddYears(1) <= _endDate)
				{
					_endDate = value.AddYears(1);
					OnPropertyChanged(() => EndDate);
				}
			}
		}

		DateTime _endDate;
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				OnPropertyChanged(() => EndDate);
				if (IsFreePeriod && value.AddYears(-1) >= _startDate)
				{
					_startDate = value.AddYears(-1);
					OnPropertyChanged(() => StartDate);
				}
			}
		}

		public DateTime MinDate
		{
			get
			{
				var result = PassJournalHelper.GetMinPassJournalDate();
				return result != null ? result.Value : new DateTime();
			}
		}
		public DateTime MaxDate { get { return DateTime.Now; } }

		public ObservableCollection<TimeTrackingPeriod> Periods { get; private set; }

		public bool IsFreePeriod
		{
			get { return Period == TimeTrackingPeriod.Period; }
		}

		TimeTrackingPeriod _period;
		public TimeTrackingPeriod Period
		{
			get { return _period; }
			set
			{
				_period = value;
				OnPropertyChanged(() => Period);
				OnPropertyChanged(() => IsFreePeriod);
			}
		}

		public ObservableCollection<TimeTrackTypeFilterItem> Totals { get; private set; }

		protected override bool Save()
		{
			base.Save();

			if (Period == TimeTrackingPeriod.Period && StartDate > EndDate)
			{
				MessageBoxService.ShowWarning(CommonViewModels.EndDateLaterStartDate);
				return false;
			}

			if (Period == TimeTrackingPeriod.Period && StartDate.Date > DateTime.Now.Date)
			{
				MessageBoxService.ShowWarning(CommonViewModels.NotValidPeriod);
				return false;
			}

			if (Period == TimeTrackingPeriod.Period && EndDate.Date > DateTime.Now.Date)
			{
				MessageBoxService.ShowWarning(CommonViewModels.NotValidPeriod);
				return false;
			}

			TimeTrackFilter.Period = Period;
			switch (Period)
			{
				case TimeTrackingPeriod.CurrentMonth:
					TimeTrackFilter.StartDate = TimeTrackingViewModel.GetFirstDayOfMonth();
					TimeTrackFilter.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.CurrentWeek:
					TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek) % 7);
					TimeTrackFilter.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.PreviousMonth:
					TimeTrackFilter.StartDate = TimeTrackingViewModel.GetFirstDayOfMonth().AddMonths(-1);
					TimeTrackFilter.EndDate = TimeTrackingViewModel.GetFirstDayOfMonth().AddDays(-1);
					break;
				case TimeTrackingPeriod.PreviousWeek:
					var firstWeekDay = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek + 1) % 7);
					TimeTrackFilter.StartDate = firstWeekDay.AddDays(-6);
					TimeTrackFilter.EndDate = firstWeekDay;
					break;
				case TimeTrackingPeriod.Period:
					TimeTrackFilter.StartDate = StartDate;
					TimeTrackFilter.EndDate = EndDate;
					break;
			}

			Filter.EmployeeFilter = EmployeeFilterViewModel.Filter;
			Filter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs.ToList();
			Filter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs.ToList();
			Filter.EmployeeFilter.OrganisationUIDs = Organisations.Items.Where(x => x.IsChecked).Select(x => x.Organisation.UID).ToList();

			Filter.TotalTimeTrackTypeFilters = new List<TimeTrackType>();
			foreach (var timeTrackTypeFilterItem in Totals)
			{
				if (timeTrackTypeFilterItem.IsChecked)
				{
					Filter.TotalTimeTrackTypeFilters.Add(timeTrackTypeFilterItem.TimeTrackType);
				}
			}
			Filter.EmployeeFilter.PersonType = PersonType.Employee;
			return true;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Filter = new TimeTrackFilter();
			InitializeFilter(Filter);
		}
	}
}