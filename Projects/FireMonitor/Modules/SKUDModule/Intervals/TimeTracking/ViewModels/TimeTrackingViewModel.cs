using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using SKDModule.Intervals.TimeTracking.Model;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using SKDModule.ViewModels;
using FiresecClient.SKDHelpers;
using FiresecAPI.SKD.EmployeeTimeIntervals;
using System.Collections;

namespace SKDModule.Intervals.TimeTracking.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		private EmployeeFilter _employeeFilter;
		private TimeTrackingSettings _settings;

		public TimeTrackingViewModel()
		{
			_employeeFilter = new EmployeeFilter()
			{
				OrganisationUIDs = FiresecClient.FiresecManager.CurrentUser.OrganisationUIDs,
			};
			_settings = new TimeTrackingSettings()
			{
				Period = TimeTrackingPeriod.PreviosMonth,
				StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day),
				EndDate = DateTime.Today,
			};
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			RefreshCommand = new RelayCommand(OnRefresh);
			PrintCommand = new RelayCommand(OnPrint);
			UpdateGrid();
		}

		private ObservableCollection<TimeTrackViewModel> _timeTracks;
		public ObservableCollection<TimeTrackViewModel> TimeTracks
		{
			get { return _timeTracks; }
			set
			{
				_timeTracks = value;
				OnPropertyChanged(() => TimeTracks);
			}
		}

		private TimeTrackViewModel _selectedTimeTrack;
		public TimeTrackViewModel SelectedTimeTrack
		{
			get { return _selectedTimeTrack; }
			set
			{
				_selectedTimeTrack = value;
				OnPropertyChanged(() => SelectedTimeTrack);
			}
		}

		private int _totalDays;
		public int TotalDays
		{
			get { return _totalDays; }
			set
			{
				_totalDays = value;
				OnPropertyChanged(() => TotalDays);
			}
		}

		private DateTime _firstDay;
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
		private void OnShowFilter()
		{
			var filter = new HRFilter()
			{
				Appointed = _employeeFilter.Appointed,
				DepartmentUIDs = _employeeFilter.DepartmentUIDs,
				Dismissed = _employeeFilter.Dismissed,
				OrganisationUID = _employeeFilter.OrganisationUID,
				OrganisationUIDs = _employeeFilter.OrganisationUIDs,
				PersonType = _employeeFilter.PersonType,
				PositionUIDs = _employeeFilter.PositionUIDs,
				RemovalDates = _employeeFilter.RemovalDates,
				Uids = _employeeFilter.Uids,
				WithDeleted = _employeeFilter.WithDeleted,
			};
			var filterViewModel = new HRFilterViewModel(filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				_employeeFilter.Appointed = filterViewModel.Filter.Appointed;
				_employeeFilter.DepartmentUIDs = filterViewModel.Filter.DepartmentUIDs;
				_employeeFilter.Dismissed = filterViewModel.Filter.Dismissed;
				_employeeFilter.OrganisationUID = filterViewModel.Filter.OrganisationUID;
				_employeeFilter.OrganisationUIDs = filterViewModel.Filter.OrganisationUIDs;
				_employeeFilter.PersonType = filterViewModel.Filter.PersonType;
				_employeeFilter.PositionUIDs = filterViewModel.Filter.PositionUIDs;
				_employeeFilter.RemovalDates = filterViewModel.Filter.RemovalDates;
				_employeeFilter.Uids = filterViewModel.Filter.Uids;
				_employeeFilter.WithDeleted = filterViewModel.Filter.WithDeleted;
				UpdateGrid();
			}
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		private void OnShowSettings()
		{
			var settingsViewModel = new TimeTrackingSettingsViewModel(_settings);
			if (DialogService.ShowModalWindow(settingsViewModel))
				UpdateGrid();
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			UpdateGrid();
		}

		public RelayCommand PrintCommand { get; private set; }
		private void OnPrint()
		{
			MessageBoxService.Show("not implemented");
		}

		private void UpdateGrid()
		{
			using (new WaitWrapper())
			{
				TotalDays = (int)(_settings.EndDate - _settings.StartDate).TotalDays + 1;
				FirstDay = _settings.StartDate;
				var employees = EmployeeHelper.Get(_employeeFilter);
				var random = new Random();
				TimeTracks = new ObservableCollection<TimeTrackViewModel>(
				employees.Select(item =>
					new TimeTrackViewModel(
						new TimeTrack()
						{
							DepartmentName = item.DepartmentName,
							EmployeeUID = item.UID,
							FirstName = item.FirstName,
							LastName = item.LastName,
							PositionName = item.PositionName,
							SecondName = item.SecondName,
							Hours = Enumerable.Repeat<double>(random.Next(5, 12), TotalDays).ToList(),
						})));
			}
		}
	}
}