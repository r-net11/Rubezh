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
			PrintCommand = new RelayCommand(OnPrint);
			RowHeight = 60;

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
			MessageBoxService.Show("Not Implemented");
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
						var timeTrackViewModel = new TimeTrackViewModel(timeTrackEmployeeResult.ShortEmployee, timeTrackEmployeeResult.DayTimeTracks);
						timeTrackViewModel.DocumentsViewModel = new DocumentsViewModel(timeTrackEmployeeResult, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
						TimeTracks.Add(timeTrackViewModel);
					}
				}
			}
		}
	}
}