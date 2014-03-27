using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using SKDModule.Models;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class ReportsViewModel : ViewPartViewModel
	{
		private EmployeeFilter _employeeFilter;
		private EmployeeReportSettings _settings;

		public ReportsViewModel()
		{
			_employeeFilter = new EmployeeFilter();
			_settings = new EmployeeReportSettings();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			RefreshCommand = new RelayCommand(OnRefresh);
			PrintCommand = new RelayCommand(OnPrint);
			UpdateReports();
		}

		private ObservableCollection<ReportViewModel> _reports;
		public ObservableCollection<ReportViewModel> Reports
		{
			get { return _reports; }
			set
			{
				_reports = value;
				OnPropertyChanged(() => Reports);
			}
		}

		private ReportViewModel selectedReport;
		public ReportViewModel SelectedReport
		{
			get { return selectedReport; }
			set
			{
				selectedReport = value;
				OnPropertyChanged(() => SelectedReport);
			}
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		private void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(_employeeFilter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
				UpdateReports();
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		private void OnShowSettings()
		{
			var reportSettingsViewModel = new ReportSettingsViewModel(_settings);
			if (DialogService.ShowModalWindow(reportSettingsViewModel))
				UpdateReports();
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			UpdateReports();
		}

		public RelayCommand PrintCommand { get; private set; }
		private void OnPrint()
		{
			MessageBoxService.Show("not implemented");
		}

		private void UpdateReports()
		{
			Reports = new ObservableCollection<ReportViewModel>();
			var employees = EmployeeHelper.Get(_employeeFilter);
			if (employees == null)
				return;
			foreach (var employee in employees)
				Reports.Add(new ReportViewModel(employee));
			SelectedReport = Reports.FirstOrDefault();
		}
	}
}