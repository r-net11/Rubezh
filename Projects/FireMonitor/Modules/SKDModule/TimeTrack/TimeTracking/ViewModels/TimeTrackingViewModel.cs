﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		TimeTrackFilter TimeTrackFilter;
		List<TimeTrackEmployeeResult> TimeTrackEmployeeResults;

		public TimeTrackingViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			ShowDocumentTypesCommand = new RelayCommand(OnShowDocumentTypes);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);


			TimeTrackFilter = new TimeTrackFilter();
			TimeTrackFilter.EmployeeFilter = new EmployeeFilter()
			{
				UserUID = FiresecClient.FiresecManager.CurrentUser.UID,
			};

			TimeTrackFilter.Period = TimeTrackingPeriod.CurrentMonth;
			TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
			TimeTrackFilter.EndDate = DateTime.Today;

			
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
				OnPropertyChanged(() => HasSelectedTimeTrack);
			}
		}

		public bool HasSelectedTimeTrack
		{
			get { return SelectedTimeTrack != null; }
		}

		public int TotalDays { get; private set; }
		public DateTime FirstDay { get; private set; }

		int _rowHeight;
		public int RowHeight
		{
			get { return _rowHeight; }
			set
			{
				_rowHeight = value;
				OnPropertyChanged(() => RowHeight);
			}
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var filterViewModel = new TimeTrackFilterViewModel(TimeTrackFilter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				UpdateGrid();
			}
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
			if (TimeTracks.Count == 0)
			{
				MessageBoxService.ShowWarning("В отчете нет ни одного сотрудника");
				return;
			}
			var organisationUIDs = new HashSet<Guid>();
			var departmentNames = new HashSet<string>();
			foreach (var timeTrack in TimeTracks)
			{
				if (timeTrack.ShortEmployee.OrganisationUID != Guid.Empty)
					organisationUIDs.Add(timeTrack.ShortEmployee.OrganisationUID);

				if (string.IsNullOrEmpty(timeTrack.ShortEmployee.DepartmentName))
				{
					MessageBoxService.ShowWarning("Сотрудник " + timeTrack.ShortEmployee.FIO + " не относится ни к одному из подразделений");
					return;
				}
				departmentNames.Add(timeTrack.ShortEmployee.DepartmentName);
			}
			if (organisationUIDs.Count > 1)
			{
				MessageBoxService.ShowWarning("В отчете должны дыть сотрудники только из одной организации");
				return;
			}
			if (departmentNames.Count > 1)
			{
				MessageBoxService.ShowWarning("В отчете должны дыть сотрудники только из одного подразделения");
				return;
			}

			if (TimeTrackFilter.StartDate.Date.Month < TimeTrackFilter.EndDate.Date.Month || TimeTrackFilter.StartDate.Date.Year < TimeTrackFilter.EndDate.Date.Year)
			{
				MessageBoxService.ShowWarning("В отчете содержаться данные за несколько месяцев. Будут показаны данные только за первый месяц");
			}

			var reportSettingsViewModel = new ReportSettingsViewModel(TimeTrackFilter, TimeTrackEmployeeResults);
			DialogService.ShowModalWindow(reportSettingsViewModel);
		}
		bool CanPrint()
		{
			return ApplicationService.IsReportEnabled;
		}

		void UpdateGrid()
		{
			using (new WaitWrapper())
			{
				var employeeUID = Guid.Empty;
				if (SelectedTimeTrack != null)
				{
					employeeUID = SelectedTimeTrack.ShortEmployee.UID;
				}

				TotalDays = (int)(TimeTrackFilter.EndDate - TimeTrackFilter.StartDate).TotalDays + 1;
				FirstDay = TimeTrackFilter.StartDate;

				TimeTracks = new ObservableCollection<TimeTrackViewModel>();
				var timeTrackResult = EmployeeHelper.GetTimeTracks(TimeTrackFilter.EmployeeFilter, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
				//var stream = FiresecManager.FiresecService.GetTimeTracksStream(TimeTrackFilter.EmployeeFilter, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
				if (timeTrackResult != null)
				{
					TimeTrackEmployeeResults = timeTrackResult.TimeTrackEmployeeResults;
					foreach (var timeTrackEmployeeResult in TimeTrackEmployeeResults)
					{
						var timeTrackViewModel = new TimeTrackViewModel(TimeTrackFilter, timeTrackEmployeeResult);
						TimeTracks.Add(timeTrackViewModel);
					}

					RowHeight = 60 + 20 * TimeTrackFilter.TotalTimeTrackTypeFilters.Count;
				}
				SelectedTimeTrack = TimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == employeeUID);
				if (SelectedTimeTrack == null)
					SelectedTimeTrack = TimeTracks.FirstOrDefault();
			}
		}

		public RelayCommand ShowDocumentTypesCommand { get; private set; }
		void OnShowDocumentTypes()
		{
			var documentTypesViewModel = new DocumentTypesViewModel();
			DialogService.ShowModalWindow(documentTypesViewModel);
		}

		void OnUserChanged(UserChangedEventArgs args)
		{
			TimeTrackFilter.EmployeeFilter.UserUID = FiresecClient.FiresecManager.CurrentUser.UID;
			UpdateGrid();
		}
	}
}