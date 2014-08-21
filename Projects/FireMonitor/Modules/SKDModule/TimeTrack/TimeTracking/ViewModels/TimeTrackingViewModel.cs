using System;
using System.Linq;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Model;
using System.Diagnostics;
using Infrastructure;
using Infrastructure.Events.Reports;
using SKDModule.Reports;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		TimeTrackFilter TimeTrackFilter;

		public TimeTrackingViewModel()
		{
			TimeTrackFilter = new TimeTrackFilter();
			TimeTrackFilter.EmployeeFilter = new EmployeeFilter()
			{
				UserUID = FiresecClient.FiresecManager.CurrentUser.UID,
			};

			TimeTrackFilter.Period = TimeTrackingPeriod.CurrentMonth;
			TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
			TimeTrackFilter.EndDate = DateTime.Today;

			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);

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
			var uids = new HashSet<Guid>();
			foreach (var timeTrack in TimeTracks)
			{
				if (timeTrack.ShortEmployee.OrganisationUID.HasValue)
					uids.Add(timeTrack.ShortEmployee.OrganisationUID.Value);
			}
			if (uids.Count > 1)
			{
				MessageBoxService.ShowWarning("В отчете должны дыть сотрудники только из одной организации");
				return;
			}

			if (TimeTrackFilter.StartDate.Date.Month < TimeTrackFilter.EndDate.Date.Month || TimeTrackFilter.StartDate.Date.Year < TimeTrackFilter.EndDate.Date.Year)
			{
				MessageBoxService.ShowWarning("В отчете содержаться данные за несколько месяцев. Будут показаны данные только за первый месяц");
			}

			var reportModel = new ReportModel();
			reportModel.StartDateTime = new DateTime(TimeTrackFilter.StartDate.Date.Year, TimeTrackFilter.StartDate.Month, 1);
			reportModel.EndDateTime = reportModel.StartDateTime.AddMonths(1).AddDays(-1);
			if (reportModel.EndDateTime > TimeTrackFilter.EndDate)
				reportModel.EndDateTime = TimeTrackFilter.EndDate;

			var reportSettingsViewModel = new ReportSettingsViewModel();
			DialogService.ShowModalWindow(reportSettingsViewModel);

			ServiceFactory.Events.GetEvent<PrintReportPreviewEvent>().Publish(new T13Report(MessageBoxService.ShowConfirmation2("Печать отчет в пейзажном формате?"), reportModel));
		}
		bool CanPrint()
		{
			return ApplicationService.IsReportEnabled;
		}

		void UpdateGrid()
		{
			using (new WaitWrapper())
			{
				TotalDays = (int)(TimeTrackFilter.EndDate - TimeTrackFilter.StartDate).TotalDays + 1;
				FirstDay = TimeTrackFilter.StartDate;
				TimeTracks = new ObservableCollection<TimeTrackViewModel>();
				var timeTrackResult = EmployeeHelper.GetTimeTracks(TimeTrackFilter.EmployeeFilter, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
				if (timeTrackResult != null)
				{
					foreach (var timeTrackEmployeeResult in timeTrackResult.TimeTrackEmployeeResults)
					{
						var timeTrackViewModel = new TimeTrackViewModel(TimeTrackFilter, timeTrackEmployeeResult.ShortEmployee, timeTrackEmployeeResult.DayTimeTracks);
						timeTrackViewModel.DocumentsViewModel = new DocumentsViewModel(timeTrackEmployeeResult, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
						TimeTracks.Add(timeTrackViewModel);
					}

					RowHeight = 60 + 20 * GetVisibleFilterRorsCount();
				}
			}
		}

		int GetVisibleFilterRorsCount()
		{
			return (TimeTrackFilter.IsTotal ? 1 : 0) +
				(TimeTrackFilter.IsTotalMissed ? 1 : 0) +
				(TimeTrackFilter.IsTotalInSchedule ? 1 : 0) +
				(TimeTrackFilter.IsTotalOvertime ? 1 : 0) +
				(TimeTrackFilter.IsTotalLate ? 1 : 0) +
				(TimeTrackFilter.IsTotalEarlyLeave ? 1 : 0) +
				(TimeTrackFilter.IsTotalPlanned ? 1 : 0) +
				(TimeTrackFilter.IsTotalEavening ? 1 : 0) +
				(TimeTrackFilter.IsTotalNight ? 1 : 0) +
				(TimeTrackFilter.IsTotal_DocumentOvertime ? 1 : 0) +
				(TimeTrackFilter.IsTotal_DocumentPresence ? 1 : 0) +
				(TimeTrackFilter.IsTotal_DocumentAbsence ? 1 : 0);
		}
	}
}