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
using System.ServiceModel;
using Common;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using System.Collections.ObjectModel;

namespace ReportsModule.ViewModels
{
	public class SKDReportPresenterViewModel : BaseViewModel
	{
		public SKDReportPresenterViewModel()
		{
			ChangeFilterCommand = new RelayCommand(OnChangeFilter, CanChangeFilter);
			FitPageSizeCommand = new RelayCommand<ZoomFitMode>(OnFitPageSize, CanFitPageSize);
		}

		public void CreateClient()
		{
			var endpoint = new EndpointAddress(ConnectionSettingsManager.ReportServerAddress);
			var binding = BindingHelper.CreateBindingFromAddress(ConnectionSettingsManager.ReportServerAddress);
			var factory = new ReportServiceClientFactory(endpoint, binding);
			Model = new XReportServicePreviewModel()
			{
				ServiceClientFactory = factory,
				IsParametersPanelVisible = false,
				AutoShowParametersPanel = false,
				IsDocumentMapVisible = false,
			};
			Model.CreateDocumentError += Model_CreateDocumentError;
			Model.Clear();
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
		public XReportServicePreviewModel()
			: base()
		{
			ZoomValues = new ReadOnlyCollection<double>(new double[] { 10.0, 25.0, 50.0, 75.0, 100.0, 150.0, 200.0, 300.0, 400.0, 500.0 });
		}
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

		protected override CreateDocumentOperation ConstructCreateDocumentOperation(ReportBuildArgs buildArgs)
		{
			var operation = base.ConstructCreateDocumentOperation(buildArgs);
			return operation;
		}

		protected override ReadOnlyCollection<double> ZoomValues { get; private set; }
	}
}
