using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackFilterViewModel : OrganisationFilterBaseViewModel<TimeTrackFilter>
	{
		public EmployeesFilterViewModel EmployeeFilterViewModel { get; private set; }
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public TimeTrackFilterViewModel(TimeTrackFilter filter) : base(filter) { }

		public override void Initialize(TimeTrackFilter timeTrackFilter)
		{
			base.Initialize(timeTrackFilter);
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
			Totals = new ObservableCollection<TimeTrackTypeFilterItem>();
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.Balance));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.Presence));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.Absence));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.AbsenceInsidePlan));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.PresenceInBrerak));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.Late));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.EarlyLeave));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.Overtime));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.Night));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.DocumentOvertime));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.DocumentPresence));
			Totals.Add(new TimeTrackTypeFilterItem(TimeTrackType.DocumentAbsence));
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
				MessageBoxService.ShowWarning("Дата окончания не может быть раньше даты начала");
				return false;
			}
			if (Period == TimeTrackingPeriod.Period && StartDate.Date > DateTime.Now.Date)
			{
				MessageBoxService.ShowWarning("Указан несуществующий период");
				return false;
			}
			if (Period == TimeTrackingPeriod.Period && EndDate.Date > DateTime.Now.Date)
			{
				MessageBoxService.ShowWarning("Указан несуществующий период");
				return false;
			}
			Filter.Period = Period;
			switch (Period)
			{
				case TimeTrackingPeriod.CurrentMonth:
					Filter.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
					Filter.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.CurrentWeek:
					Filter.StartDate = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek) % 7);
					Filter.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.PreviousMonth:
					var firstMonthDay = DateTime.Today.AddDays(1 - DateTime.Today.Day);
					Filter.StartDate = firstMonthDay.AddMonths(-1);
					Filter.EndDate = firstMonthDay.AddDays(-1);
					break;
				case TimeTrackingPeriod.PreviousWeek:
					var firstWeekDay = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek + 1) % 7);
					Filter.StartDate = firstWeekDay.AddDays(-6);
					Filter.EndDate = firstWeekDay;
					break;
				case TimeTrackingPeriod.Period:
					Filter.StartDate = StartDate;
					Filter.EndDate = EndDate;
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

		protected override List<IHRFilterTab> HRFilterTabs
		{
			get { return new List<IHRFilterTab> { DepartmentsFilterViewModel, EmployeeFilterViewModel, PositionsFilterViewModel }; }
		}
	}
}