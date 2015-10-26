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
		public ReportFilterViewModel ReportFilterViewModel { get; private set; }
		public ChangeFlowFilterViewModel SavedChangedFlowFilterViewModel { get; private set; }
		public DebtorsFilterViewModel SavedDebtorsFilterViewModel { get; private set; }
		public static ReportFilter Filter { get; set; }
		public ReportsViewModel()
		{
			ChangeFilterCommand = new RelayCommand(OnChangeFilter, CanChangeFilter);
			RefreshPreviewReportCommand = new RelayCommand(OnRefreshPreviewReport, CanRefreshPreviewReport);
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
			get { return _selectedReportType; }
			set
			{
				switch (_selectedReportType)
				{
					case ReportType.ChangeFlow:
					case ReportType.ChangeValue:
						SavedChangedFlowFilterViewModel = (ChangeFlowFilterViewModel)ReportFilterViewModel;
						break;
					case ReportType.Debtors:
						SavedDebtorsFilterViewModel = (DebtorsFilterViewModel)ReportFilterViewModel;
						break;
				}

				_selectedReportType = value;

				switch (_selectedReportType)
				{
					case ReportType.ChangeFlow:
						if (SavedChangedFlowFilterViewModel != null)
						{
							SavedChangedFlowFilterViewModel.IsNotShowCounters = false;
							ReportFilterViewModel = SavedChangedFlowFilterViewModel;
						}
						else
							ReportFilterViewModel = new ChangeFlowFilterViewModel { IsNotShowCounters = false };
						break;
					case ReportType.Debtors:
						ReportFilterViewModel = SavedDebtorsFilterViewModel ?? new DebtorsFilterViewModel();
						break;
					case ReportType.ChangeValue:
						if (SavedChangedFlowFilterViewModel != null)
						{
							SavedChangedFlowFilterViewModel.IsNotShowCounters = true;
							ReportFilterViewModel = SavedChangedFlowFilterViewModel;
						}
						else
							ReportFilterViewModel = new ChangeFlowFilterViewModel { IsNotShowCounters = true };
						break;
				}

				Filter = ReportFilterViewModel != null ? ReportFilterViewModel.Filter : null;
				RefreshPreviewReportCommand.Execute();
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
			return SelectedReportType != null;
		}
		public RelayCommand RefreshPreviewReportCommand { get; private set; }
		void OnRefreshPreviewReport()
		{
			BuildReport();
		}
		bool CanRefreshPreviewReport()
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