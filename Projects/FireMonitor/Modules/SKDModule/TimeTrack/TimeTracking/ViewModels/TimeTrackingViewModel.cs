using System;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;
using System.Diagnostics;

namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		EmployeeFilter EmployeeFilter;
		TimeTrackingSettings TimeTrackingSettings;

		public TimeTrackingViewModel()
		{
			EmployeeFilter = new EmployeeFilter()
			{
				UserUID = FiresecClient.FiresecManager.CurrentUser.UID,
			};

			TimeTrackingSettings = new TimeTrackingSettings()
			{
				Period = TimeTrackingPeriod.CurrentMonth,
				StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day),
				EndDate = DateTime.Today
			};
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			RefreshCommand = new RelayCommand(OnRefresh);
			PrintCommand = new RelayCommand(OnPrint);
			UpdateGrid();
		}

		ObservableCollection<TimeTrackViewModel> _timeTracks;
		public ObservableCollection<TimeTrackViewModel> TimeTracks
		{
			get { return _timeTracks; }
			set
			{
				_timeTracks = value;
				OnPropertyChanged(() => TimeTracks);
			}
		}

		TimeTrackViewModel _selectedTimeTrack;
		public TimeTrackViewModel SelectedTimeTrack
		{
			get { return _selectedTimeTrack; }
			set
			{
				_selectedTimeTrack = value;
				OnPropertyChanged(() => SelectedTimeTrack);
			}
		}

		int _totalDays;
		public int TotalDays
		{
			get { return _totalDays; }
			set
			{
				_totalDays = value;
				OnPropertyChanged(() => TotalDays);
			}
		}

		DateTime _firstDay;
		public DateTime FirstDay
		{
			get { return _firstDay; }
			set
			{
				_firstDay = value;
				OnPropertyChanged(() => FirstDay);
			}
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilter = new EmployeeFilter()
			{
				OrganisationUIDs = EmployeeFilter.OrganisationUIDs,
				DepartmentUIDs = EmployeeFilter.DepartmentUIDs,
				PositionUIDs = EmployeeFilter.PositionUIDs,
				Appointed = EmployeeFilter.Appointed,
				PersonType = EmployeeFilter.PersonType
			};
			var filter = new HRFilter()
			{
				EmployeeFilter = employeeFilter,
				RemovalDates = EmployeeFilter.RemovalDates,
				UIDs = EmployeeFilter.UIDs,
				LogicalDeletationType = EmployeeFilter.LogicalDeletationType,
			};
			var filterViewModel = new HRFilterViewModel(filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				EmployeeFilter.OrganisationUIDs = filterViewModel.Filter.OrganisationUIDs;
				EmployeeFilter.DepartmentUIDs = filterViewModel.Filter.EmployeeFilter.DepartmentUIDs;
				EmployeeFilter.PositionUIDs = filterViewModel.Filter.EmployeeFilter.PositionUIDs;
				EmployeeFilter.Appointed = filterViewModel.Filter.EmployeeFilter.Appointed;
				EmployeeFilter.PersonType = filterViewModel.Filter.EmployeeFilter.PersonType;
				EmployeeFilter.RemovalDates = filterViewModel.Filter.RemovalDates;
				EmployeeFilter.UIDs = filterViewModel.Filter.UIDs;
				EmployeeFilter.LogicalDeletationType = filterViewModel.Filter.LogicalDeletationType;
				UpdateGrid();
			}
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var settingsViewModel = new TimeTrackingSettingsViewModel(TimeTrackingSettings);
			if (DialogService.ShowModalWindow(settingsViewModel))
				UpdateGrid();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			UpdateGrid();
			stopwatch.Stop();
			Trace.WriteLine("OnRefresh time " + stopwatch.Elapsed.ToString());
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			MessageBoxService.Show("Not Implemented");
		}

		void UpdateGrid()
		{
			using (new WaitWrapper())
			{
				TotalDays = (int)(TimeTrackingSettings.EndDate - TimeTrackingSettings.StartDate).TotalDays + 1;
				FirstDay = TimeTrackingSettings.StartDate;
				TimeTracks = new ObservableCollection<TimeTrackViewModel>();
				var timeTrackResult = EmployeeHelper.GetTimeTracks(EmployeeFilter, TimeTrackingSettings.StartDate, TimeTrackingSettings.EndDate);
				if (timeTrackResult != null)
				{
					foreach (var timeTrackEmployeeResult in timeTrackResult.TimeTrackEmployeeResults)
					{
						TimeTracks.Add(new TimeTrackViewModel(timeTrackEmployeeResult.ShortEmployee, timeTrackEmployeeResult.DayTimeTracks));
					}
				}
			}
		}
	}
}