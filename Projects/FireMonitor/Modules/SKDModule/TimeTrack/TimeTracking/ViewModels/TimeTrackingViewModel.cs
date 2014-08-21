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
		private void OnPrint()
		{
			ServiceFactory.Events.GetEvent<PrintReportPreviewEvent>().Publish(new T13Report(MessageBoxService.ShowConfirmation2("Печать отчет в пейзажном формате?")));
		}
		private bool CanPrint()
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