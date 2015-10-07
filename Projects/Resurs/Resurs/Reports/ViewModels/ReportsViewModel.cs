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
			OpenReportDesignerCommand = new RelayCommand(OnOpenReportDesigner, CanOpenReportDesigner);
			ShowPreviewReportCommand = new RelayCommand(OnShowPreviewReport, CanShowPreviewReport);
			ReportFilterViewModel = new ReportFilterViewModel();
			ReportTypes = new List<ReportType>(Enum.GetValues(typeof(ReportType)).Cast<ReportType>());
			Filter = ReportFilterViewModel.Filter;
			//var path = @"C:\";
			//foreach (var filePath in Directory.EnumerateFiles(path, "*.repx"))
			//{
			//	Reports.Add(new ReportViewModel(XtraReport.FromFile(filePath, true)));
			//}
			//Reports.Add(new ReportViewModel(new CustomerReport()));
			//Reports.Add(new ReportViewModel(new EmployeeReport()));
			//Reports.Add(new ReportViewModel(new ProductReport()));
		}
		public ReportFilterViewModel ReportFilterViewModel { get; set; }
		public static ReportFilter Filter { get; set; }
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
		private ReportType? _selectedReportType;
 		public ReportType? SelectedReportType
		{
			get { return _selectedReportType;}
			set
			{
				_selectedReportType = value;
				if (Model != null)
					Model.ClosePreview();
				ShowPreviewReportCommand.Execute();
				OnPropertyChanged(() => SelectedReportType);
			}
		}
		public RelayCommand ChangeFilterCommand { get; private set; }
		private void OnChangeFilter()
		{
			if (DialogService.ShowModalWindow(ReportFilterViewModel))
			{
				Filter = ReportFilterViewModel.Filter;
				BuildReport();
			}
		}
		private bool CanChangeFilter()
		{
			return SelectedReportType != null && SelectedReportType != ReportType.Debtors;
		}
		public RelayCommand OpenReportDesignerCommand { get; private set; }
		private void OnOpenReportDesigner()
		{
			var reportDesignerViewModel = new ReportDesignerViewModel(Model);
			DialogService.ShowModalWindow(reportDesignerViewModel);
		}
		private bool CanOpenReportDesigner()
		{
			return SelectedReportType != null;
		}
		public RelayCommand ShowPreviewReportCommand { get; private set; }
		private void OnShowPreviewReport()
		{
			BuildReport();
		}
		private bool CanShowPreviewReport()
		{
			return SelectedReportType != null && IsFilterValidate();
		}
		private void BuildReport()
		{
			Model = ReportHelper.GetDefaultReport(SelectedReportType.Value);
			Model.CreateDocument();
		}
		private bool IsFilterValidate()
		{
			switch (SelectedReportType)
			{
				case ReportType.ChangeFlow:
					{
						if (Filter.Device == null)
						{
							MessageBoxService.ShowError("В системе нет счетчиков.");
							return false;
						}
						break;
					}
			}
			return true;
		}
	}
}