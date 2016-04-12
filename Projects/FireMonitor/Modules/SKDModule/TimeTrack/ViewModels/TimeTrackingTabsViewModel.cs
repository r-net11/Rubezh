using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule.ViewModels
{
	public class TimeTrackingTabsViewModel : ViewPartViewModel
	{
		public DayIntervalsViewModel DayIntervalsViewModel { get; set; }
		public ScheduleSchemesViewModel ScheduleSchemesViewModel { get; set; }
		public HolidaysViewModel HolidaysViewModel { get; set; }
		public SchedulesViewModel SchedulesViewModel { get; set; }
		public TimeTrackingViewModel TimeTrackingViewModel { get; set; }

		SKDTabItems SKDTabItems;
		HRFilter Filter { get { return SKDTabItems.Filter; } set { SKDTabItems.Filter = value; } }

		public TimeTrackingTabsViewModel(SKDTabItems skdTabItems)
		{
			_isConnected = true;
			SKDTabItems = skdTabItems;
			EditFilterCommand = new RelayCommand(OnEditFilter, CanEditFilter);
			DayIntervalsViewModel = new DayIntervalsViewModel();
			ScheduleSchemesViewModel = new ScheduleSchemesViewModel();
			HolidaysViewModel = new HolidaysViewModel();
			SchedulesViewModel = new SchedulesViewModel();
			TimeTrackingViewModel = new TimeTrackingViewModel();
			if (CanSelectDayIntervals) 
				IsDayIntervalsSelected = true;
			else if (CanSelectScheduleSchemes) 
				IsScheduleSchemesSelected = true;
			else if (CanSelectHolidays) 
				IsHolidaysSelected = true;
			else if (CanSelectSchedules) 
				IsSchedulesSelected = true;
			else if (CanSelectTimeTracking) 
				IsTimeTrackingSelected = true;
		}

		public void Initialize()
		{
			DayIntervalsViewModel.Initialize(new DayIntervalFilter 
			{ 
				UserUID = ClientManager.CurrentUser.UID, 
				LogicalDeletationType = Filter.LogicalDeletationType, 
				OrganisationUIDs = Filter.OrganisationUIDs
			});
			ScheduleSchemesViewModel.Initialize(new ScheduleSchemeFilter
			{
				UserUID = ClientManager.CurrentUser.UID,
				LogicalDeletationType = Filter.LogicalDeletationType,
				OrganisationUIDs = Filter.OrganisationUIDs
			});
			HolidaysViewModel.Initialize(new HolidayFilter
			{
				UserUID = ClientManager.CurrentUser.UID,
				LogicalDeletationType = Filter.LogicalDeletationType,
				OrganisationUIDs = Filter.OrganisationUIDs
			});
			SchedulesViewModel.Initialize(new ScheduleFilter
			{
				UserUID = ClientManager.CurrentUser.UID,
				LogicalDeletationType = Filter.LogicalDeletationType,
				OrganisationUIDs = Filter.OrganisationUIDs
			});
		}

		bool _IsDayIntervalsSelected;
		public bool IsDayIntervalsSelected
		{
			get { return _IsDayIntervalsSelected; }
			set
			{
				_IsDayIntervalsSelected = value;
				OnPropertyChanged(() => IsDayIntervalsSelected);
			}
		}
		
		bool _IsScheduleSchemesSelected;
		public bool IsScheduleSchemesSelected
		{
			get { return _IsScheduleSchemesSelected; }
			set
			{
				_IsScheduleSchemesSelected = value;
				OnPropertyChanged(() => IsScheduleSchemesSelected);
			}
		}

		bool _IsHolidaysSelected;
		public bool IsHolidaysSelected
		{
			get { return _IsHolidaysSelected; }
			set
			{
				_IsHolidaysSelected = value;
				OnPropertyChanged(() => IsHolidaysSelected);
			}
		}

		bool _IsSchedulesSelected;
		public bool IsSchedulesSelected
		{
			get { return _IsSchedulesSelected; }
			set
			{
				_IsSchedulesSelected = value;
				OnPropertyChanged(() => IsSchedulesSelected);
			}
		}

		bool _IsTimeTrackingSelected;
		public bool IsTimeTrackingSelected
		{
			get { return _IsTimeTrackingSelected; }
			set
			{
				_IsTimeTrackingSelected = value;
				OnPropertyChanged(() => IsTimeTrackingSelected);
			}
		}

		public bool CanSelectDayIntervals { get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_TimeTrack_DaySchedules_View); } }
		public bool CanSelectScheduleSchemes { get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_View); } }
		public bool CanSelectHolidays { get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_TimeTrack_Holidays_View); } }
		public bool CanSelectSchedules { get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_TimeTrack_Schedules_View); } }
		public bool CanSelectTimeTracking { get { return ClientManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_TimeTrack_Report_View); } }

		public string FilterImageSource { get { return Filter.EmployeeFilter.IsNotEmpty ? "archive" : "filter"; } }

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new HRFilterViewModel(Filter, false, false);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				OnPropertyChanged(() => FilterImageSource);
				SKDTabItems.Initialize();
			}
		}
		bool CanEditFilter()
		{
			return _isConnected;
		}

		public bool ShowFromJournal(Guid uid)
		{
			bool isFounded = false;
			if(DayIntervalsViewModel.ShowFromJournal(uid))
			{
				IsDayIntervalsSelected = true;
				isFounded = true;
			}
			else if(ScheduleSchemesViewModel.ShowFromJournal(uid))
			{
				IsScheduleSchemesSelected = true;
				isFounded = true;
			}
			else if(HolidaysViewModel.ShowFromJournal(uid))
			{
				IsHolidaysSelected = true;
				isFounded = true;
			}
			else if(SchedulesViewModel.ShowFromJournal(uid))
			{
				IsSchedulesSelected = true;
				isFounded = true;
			}
			if (isFounded)
				ApplicationService.Layout.Show(this);
			return isFounded;
		}

		bool _isConnected;

		public void OnConnectionLost()
		{
			_isConnected = false;
			DayIntervalsViewModel.OnConnectionLost();
			ScheduleSchemesViewModel.OnConnectionLost();
			HolidaysViewModel.OnConnectionLost();
			SchedulesViewModel.OnConnectionLost();
			TimeTrackingViewModel.OnConnectionLost();
		}

		public void OnConnectionAppeared()
		{
			_isConnected = true;
			DayIntervalsViewModel.OnConnectionAppeared();
			ScheduleSchemesViewModel.OnConnectionAppeared();
			HolidaysViewModel.OnConnectionAppeared();
			SchedulesViewModel.OnConnectionAppeared();
			TimeTrackingViewModel.OnConnectionAppeared();
		}
	}
}
