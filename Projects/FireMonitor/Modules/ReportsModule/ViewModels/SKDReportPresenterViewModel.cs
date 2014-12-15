using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using DevExpress.Xpf.Printing;
using Infrastructure.Common.Windows;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using DevExpress.DocumentServices.ServiceModel.Client;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common;

namespace ReportsModule.ViewModels
{
	public class SKDReportPresenterViewModel : BaseViewModel
	{
		public SKDReportPresenterViewModel()
		{
			ChangeFilterCommand = new RelayCommand(OnChangeFilter, CanChangeFilter);
			FitPageSizeCommand = new RelayCommand<ZoomFitMode>(OnFitPageSize, CanFitPageSize);
			Model = new XReportServicePreviewModel("http://127.0.0.1:2323/FiresecReportService/");
			Model.IsParametersPanelVisible = false;
			Model.AutoShowParametersPanel = false;
			Model.IsDocumentMapVisible = false;
			Model.CreateDocumentError += Model_CreateDocumentError;
		}

		private SKDReportBaseViewModel _selectedReport;
		public SKDReportBaseViewModel SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				if (value != SelectedReport)
				{
					_selectedReport = value;
					OnPropertyChanged(() => SelectedReport);
					BuildReport();
				}
			}
		}

		private XReportServicePreviewModel _model;
		public XReportServicePreviewModel Model
		{
			get { return _model; }
			set
			{
				_model = value;
				OnPropertyChanged(() => Model);
			}
		}

		private void BuildReport()
		{
			var reportProvider = SelectedReport != null && SelectedReport is SKDReportViewModel ? ((SKDReportViewModel)SelectedReport).ReportProvider : null;
			if (reportProvider != null)
			{
				Model.ReportName = reportProvider.Name;
				Model.Build(reportProvider is IFilteredSKDReportProvider ? ((IFilteredSKDReportProvider)reportProvider).FilterObject : null);
			}
		}

		private void Model_CreateDocumentError(object sender, FaultEventArgs e)
		{
			e.Handled = true;
			MessageBoxService.ShowException(e.Fault);
		}

		public RelayCommand ChangeFilterCommand { get; private set; }
		private void OnChangeFilter()
		{
			if (((IFilteredSKDReportProvider)((SKDReportViewModel)SelectedReport).ReportProvider).ChangeFilter())
				BuildReport();
		}
		private bool CanChangeFilter()
		{
			return SelectedReport != null && SelectedReport is SKDReportViewModel && ((SKDReportViewModel)SelectedReport).ReportProvider is IFilteredSKDReportProvider;
		}

		public RelayCommand<ZoomFitMode> FitPageSizeCommand { get; private set; }
		private void OnFitPageSize(ZoomFitMode fitMode)
		{
			Model.ZoomMode = new ZoomFitModeItem(fitMode);
		}
		private bool CanFitPageSize(ZoomFitMode fitMode)
		{
			return Model != null && !Model.IsEmptyDocument;
		}
	}
	public class XReportServicePreviewModel : ReportServicePreviewModel
	{
		public XReportServicePreviewModel(string s)
			: base(s)
		{
		}
		public void Build(object args)
		{
			CreateDocument(args);
		}
		public IReportServiceClient ServiceClient
		{
			get { return Client; }
		}
	}
}
