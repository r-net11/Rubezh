using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common;
using SKDModule.Model;
using System.Collections.ObjectModel;
using System;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class TimeTrackFilterViewModel : OrganisationFilterBaseViewModel<TimeTrackFilter>
	{
		public EmployeeFilterViewModel EmployeeFilterViewModel { get; private set; }
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }
		public TimeTrackFilter TimeTrackFilter { get; private set; }

		public TimeTrackFilterViewModel(TimeTrackFilter filter)
			: base(filter)
		{
			TimeTrackFilter = filter;
			ResetCommand = new RelayCommand(OnReset);
			InitializeFilter(filter);
		}

		void InitializeFilter(TimeTrackFilter timeTrackFilter)
		{
			EmployeeFilterViewModel = new EmployeeFilterViewModel(timeTrackFilter.EmployeeFilter);
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel(timeTrackFilter.EmployeeFilter);
			PositionsFilterViewModel = new PositionsFilterViewModel(timeTrackFilter.EmployeeFilter);

			Periods = new ObservableCollection<TimeTrackingPeriod>(Enum.GetValues(typeof(TimeTrackingPeriod)).OfType<TimeTrackingPeriod>());
			Period = timeTrackFilter.Period;
			StartDate = timeTrackFilter.StartDate;
			EndDate = timeTrackFilter.EndDate;
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

		protected override bool Save()
		{
			base.Save();

			if (Period == TimeTrackingPeriod.Period && StartDate >= EndDate)
			{
				MessageBoxService.ShowWarning("Дата окончания не может быть раньше даты начала");
				return false;
			}

			TimeTrackFilter.Period = Period;
			switch (Period)
			{
				case TimeTrackingPeriod.CurrentMonth:
					TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
					TimeTrackFilter.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.CurrentWeek:
					TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek + 1) % 7);
					TimeTrackFilter.EndDate = DateTime.Today;
					break;
				case TimeTrackingPeriod.PreviousMonth:
					var firstMonthDay = DateTime.Today.AddDays(1 - DateTime.Today.Day);
					TimeTrackFilter.StartDate = firstMonthDay.AddMonths(-1);
					TimeTrackFilter.EndDate = firstMonthDay.AddDays(-1);
					break;
				case TimeTrackingPeriod.PreviousWeek:
					var firstWeekDay = DateTime.Today.AddDays(1 - ((int)DateTime.Today.DayOfWeek + 1) % 7);
					TimeTrackFilter.StartDate = firstWeekDay.AddDays(-7);
					TimeTrackFilter.EndDate = firstWeekDay.AddDays(-1);
					break;
				case TimeTrackingPeriod.Period:
					TimeTrackFilter.StartDate = StartDate;
					TimeTrackFilter.EndDate = EndDate;
					break;
			}

			Filter.EmployeeFilter = EmployeeFilterViewModel.Save();
			Filter.EmployeeFilter.DepartmentUIDs = DepartmentsFilterViewModel.UIDs.ToList();
			Filter.EmployeeFilter.PositionUIDs = PositionsFilterViewModel.UIDs.ToList();
			Filter.EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs;
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