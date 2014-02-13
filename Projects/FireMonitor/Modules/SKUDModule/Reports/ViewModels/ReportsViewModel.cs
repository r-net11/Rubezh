using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using SKDModule.Models;

namespace SKDModule.ViewModels
{
	public class ReportsViewModel : ViewPartViewModel
	{
		public ReportsViewModel()
		{
			Filter = new EmployeeFilter();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			SelectedReport = Reports.FirstOrDefault();
			RefreshCommand = new RelayCommand(OnRefresh);
		}

		EmployeeReportFilter EmployeeReportFilter = new EmployeeReportFilter();

		EmployeeFilter filter;
		public EmployeeFilter Filter
		{
			get { return filter; }
			set
			{
				filter = value;
				UpdateReports();
			}
		}

		ObservableCollection<ReportViewModel> _reports;
		public ObservableCollection<ReportViewModel> Reports
		{
			get { return _reports; }
			set
			{
				_reports = value;
				OnPropertyChanged("Reports");
			}
		}

		ReportViewModel selectedReport;
		public ReportViewModel SelectedReport
		{
			get { return selectedReport; }
			set
			{
				selectedReport = value;
				OnPropertyChanged("SelectedReport");
			}
		}

		void UpdateReports()
		{
			Reports = new ObservableCollection<ReportViewModel>();
			FiresecManager.GetEmployees(Filter).ToList().ForEach(x => Reports.Add(new ReportViewModel(x)));
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
			}
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var reportSettingsViewModel = new ReportSettingsViewModel(EmployeeReportFilter);
			if (DialogService.ShowModalWindow(reportSettingsViewModel))
			{
				EmployeeReportFilter = reportSettingsViewModel.EmployeeReportFilter;
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
		}
	}
}