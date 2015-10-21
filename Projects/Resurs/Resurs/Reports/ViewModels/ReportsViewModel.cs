using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using DevExpress.XtraReports.UI;
using Resurs.Reports;
using System;
using System.Linq;
using DevExpress.Xpf.Printing;

namespace Resurs.ViewModels
{
	public class ReportsViewModel : BaseViewModel
	{		
		public ReportFilterViewModel ReportFilterViewModel { get; set; }
		public static ReportFilter Filter { get; set; }
		public ReportsViewModel()
		{
			ChangeFilterCommand = new RelayCommand(OnChangeFilter, CanChangeFilter);
			OpenReportDesignerCommand = new RelayCommand(OnOpenReportDesigner, CanOpenReportDesigner);
			ShowPreviewReportCommand = new RelayCommand(OnShowPreviewReport, CanShowPreviewReport);
			FitPageSizeCommand = new RelayCommand<ZoomFitMode>(OnFitPageSize, CanFitPageSize);

			ReportTypes = new List<ReportType>(Enum.GetValues(typeof(ReportType)).Cast<ReportType>());
		}
		ReportPreviewModel _model;
		public ReportPreviewModel Model
		{
			get { return _model; }
			set
			{
				_model = value;
				OnPropertyChanged(() => Model);
			}
		}
		public List<ReportType> ReportTypes { get; private set; }
		ReportType? _selectedReportType;
 		public ReportType? SelectedReportType
		{
			get { return _selectedReportType;}
			set
			{
				_selectedReportType = value;
				switch (_selectedReportType)
				{
					case ReportType.ChangeFlow:
						ReportFilterViewModel = new ChangeFlowFilterViewModel();
						break;
					case ReportType.Debtors:
						ReportFilterViewModel = new DebtorsFilterViewModel();
						break;
				}						
				Filter = ReportFilterViewModel.Filter;
				ShowPreviewReportCommand.Execute();
				OnPropertyChanged(() => SelectedReportType);
			}
		}
		public RelayCommand ChangeFilterCommand { get; private set; }
		void OnChangeFilter()
		{
			if (Infrastructure.Common.Windows.DialogService.ShowModalWindow(ReportFilterViewModel))
			{
				Filter = ReportFilterViewModel.Filter;
				BuildReport();
			}
		}
		bool CanChangeFilter()
		{
			return SelectedReportType != null && SelectedReportType != ReportType.Receipts && SelectedReportType != ReportType.ChangeValue;
		}
		public RelayCommand OpenReportDesignerCommand { get; private set; }
		void OnOpenReportDesigner()
		{
			var reportDesignerViewModel = new ReportDesignerViewModel((XtraReport)Model.Report);
			Infrastructure.Common.Windows.DialogService.ShowModalWindow(reportDesignerViewModel);
		}
		bool CanOpenReportDesigner()
		{
			return SelectedReportType != null;
		}
		public RelayCommand ShowPreviewReportCommand { get; private set; }
		void OnShowPreviewReport()
		{
			BuildReport();
		}
		bool CanShowPreviewReport()
		{
			return SelectedReportType != null;
		}
		public RelayCommand<ZoomFitMode> FitPageSizeCommand { get; private set; }
		void OnFitPageSize(ZoomFitMode fitMode)
		{ 
			if (fitMode == ZoomFitMode.WholePage)
			{
				Model.ZoomMode = null;
				Model.SetZoom(100);
				return;
			}
			Model.ZoomMode = new ZoomFitModeItem(fitMode);
		}
		bool CanFitPageSize(ZoomFitMode fitMode)
		{
			return Model != null && Model.PrintCommand.CanExecute(null);
		}
		void BuildReport()
		{
			Model = CreateModel(SelectedReportType);
			Model.Report.CreateDocument();
		}
		ReportPreviewModel CreateModel(ReportType? reportType)
		{
			var report = ReportHelper.GetDefaultReport(reportType.Value);
			return new ReportPreviewModel(report)
			{
				AutoShowParametersPanel = false,
				IsParametersPanelVisible = false,
				IsDocumentMapVisible = false
			};
		}
	}
}