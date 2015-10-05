using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using DevExpress.XtraReports.UI;
using Infrastructure.Common.Windows;
using System.IO;
using Resurs.Reports;
using System;
using System.Linq;
using DevExpress.XtraReports.Localization;
using Resurs.ViewModels;

namespace Resurs.ViewModels
{
	public class ReportsViewModel : BaseViewModel
	{
		public ReportsViewModel()
		{
			ChangeFilterCommand = new RelayCommand(OnChangeFilter, CanChangeFilter);
			RefreshReportCommand = new RelayCommand(OnRefreshReport, CanRefreshReport);
			OpenReportDesignerCommand = new RelayCommand(OnOpenReportDesigner, CanOpenReportDesigner);
			ShowPreviewReportCommand = new RelayCommand(OnShowPreviewReport, CanShowPreviewReport);

			Reports = new List<ReportViewModel>();
			ReportTypes = new List<ReportType>(Enum.GetValues(typeof(ReportType)).Cast<ReportType>());
			SelectedReportType = ReportTypes.FirstOrDefault();
			//var path = @"C:\";
			//foreach (var filePath in Directory.EnumerateFiles(path, "*.repx"))
			//{
			//	Reports.Add(new ReportViewModel(XtraReport.FromFile(filePath, true)));
			//}
			//Reports.Add(new ReportViewModel(new CustomerReport()));
			//Reports.Add(new ReportViewModel(new EmployeeReport()));
			//Reports.Add(new ReportViewModel(new ProductReport()));
		}
		private XtraReport _model;
		public XtraReport Model
		{
			get { return _model; }
			set
			{
				_model = value;
				OnPropertyChanged(() => Model);
			}
		}
		private List<ReportViewModel> _reports;
		public List<ReportViewModel> Reports
		{
			get { return _reports; }
			set
			{
				_reports = value;
				OnPropertyChanged(() => Reports);
			}
		}
		private ReportViewModel _selectedReport;
		public ReportViewModel SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				_selectedReport = value;
				if (value != null)
					Model = value.Model;
				OnPropertyChanged(() => SelectedReport);
			}
		}
		public List<ReportType> ReportTypes { get; private set; }
		private ReportType _selectedReportType;
 		public ReportType SelectedReportType
		{
			get { return _selectedReportType;}
			set
			{
				_selectedReportType = value;
				if (Model != null)
					Model.ClosePreview();
				Model = ReportHelper.GetDefaultReport(value);
				OnPropertyChanged(() => Model);
				OnPropertyChanged(() => SelectedReportType);
			}
		}
		public RelayCommand ChangeFilterCommand { get; private set; }
		private void OnChangeFilter()
		{
			BuildReport();
		}
		private bool CanChangeFilter()
		{
			return SelectedReport != null;
		}
		public RelayCommand RefreshReportCommand { get; private set; }
		private void OnRefreshReport()
		{
		}
		private bool CanRefreshReport()
		{
			return true;
		}
		public RelayCommand OpenReportDesignerCommand { get; private set; }
		private void OnOpenReportDesigner()
		{
			var reportDesignerViewModel = new ReportDesignerViewModel(Model);
			DialogService.ShowModalWindow(reportDesignerViewModel);
		}
		private bool CanOpenReportDesigner()
		{
			return true;
		}
		public RelayCommand ShowPreviewReportCommand { get; private set; }
		private void OnShowPreviewReport()
		{
			var repxSelectionViewModel = new RepxSelectionViewModel(SelectedReportType);
			if (DialogService.ShowModalWindow(repxSelectionViewModel))
			{
				Model = repxSelectionViewModel.SelectedRepx.Model;
			}
			BuildReport();
		}
		private bool CanShowPreviewReport()
		{
			return true;
		}
		private void BuildReport()
		{
			Model.CreateDocument();
		}
	}
}