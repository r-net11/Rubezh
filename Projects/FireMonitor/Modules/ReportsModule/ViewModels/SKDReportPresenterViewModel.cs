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
using System.Windows.Input;
using DialogService = Infrastructure.Common.Windows.DialogService;

namespace ReportsModule.ViewModels
{
	public class SKDReportPresenterViewModel : BaseViewModel
	{
		private ISKDReportProvider _reportProvider;
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
			Model.PropertyChanged += (s, e) => CommandManager.InvalidateRequerySuggested();
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
			_reportProvider = SelectedReport != null && SelectedReport is SKDReportViewModel ? ((SKDReportViewModel)SelectedReport).ReportProvider : null;
			if (_reportProvider != null)
				try
				{
					using (new WaitWrapper())
					{
						Model.ReportName = _reportProvider.Name;
						Model.Build(_reportProvider is IFilteredSKDReportProvider ? ((IFilteredSKDReportProvider)_reportProvider).FilterObject : null);
					}
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
					if (ApplicationService.ApplicationActivated)
						MessageBoxService.ShowException(ex, "Возникла ошибка при построении отчета");
					if (ex is CommunicationException)
						CreateClient();
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
			var provider = ((IFilteredSKDReportProvider)_reportProvider);
			var model = provider.CreateFilterModel();
			var filterViewModel = new SKDReportFilterViewModel(provider.FilterObject, model);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				provider.UpdateFilter(filterViewModel.Filter);
				BuildReport();
			}
		}
		private bool CanChangeFilter()
		{
			return _reportProvider != null && _reportProvider is IFilteredSKDReportProvider;
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
}
