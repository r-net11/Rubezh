using System;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows.Threading;
using Common;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.Xpf.Printing;
using FiresecAPI.SKD.ReportFilters;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using DialogService = Infrastructure.Common.Windows.DialogService;

namespace ReportsModule.ViewModels
{
	public class SKDReportPresenterViewModel : BaseViewModel
	{
		private ISKDReportProvider _reportProvider;
		private DispatcherTimer _timer;
		public SKDReportPresenterViewModel()
		{
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
			ChangeFilterCommand = new RelayCommand(OnChangeFilter, CanChangeFilter);
			RefreshReportCommand = new RelayCommand(OnRefreshReport, CanRefreshReport);
			FitPageSizeCommand = new RelayCommand<ZoomFitMode>(OnFitPageSize, CanFitPageSize);

			_timer = new DispatcherTimer();
			_timer.Tick += (s, e) => ElapsedTime++;
			_timer.Interval = TimeSpan.FromSeconds(1);
		}

		public void CreateClient()
		{
			var reportServerAddress = ConnectionSettingsManager.ReportServerAddress;
			var endpoint = new EndpointAddress(reportServerAddress);
			var binding = BindingHelper.CreateBindingFromAddress(reportServerAddress);
			var factory = new ReportServiceClientFactory(endpoint, binding);
			Model = new XReportServicePreviewModel
			{
				ServiceClientFactory = factory,
				IsParametersPanelVisible = false,
				AutoShowParametersPanel = false,
				IsDocumentMapVisible = false
			};
			Model.PropertyChanged += (s, e) =>
			{
				CommandManager.InvalidateRequerySuggested();
				if (e.PropertyName == "ProgressVisibility")
					OnProgressChanged();
			};
			Model.CreateDocumentError += Model_CreateDocumentError;
			Model.ExportError += Model_CreateDocumentError;
			Model.PrintError += Model_CreateDocumentError;
			Model.GetPageError += Model_GetPageError;
			Model.UnhandledError += Model_GetPageError;
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
					if (SelectedReport != null)
					{
						SelectedReport.Reset();
						OnFitPageSize(ZoomFitMode.WholePage); // SKDDEV-409 пункт 1 - приведение масштаба каждого вновь открытого отчета к исходному размеру (100%)
					}
				}
				CommandManager.InvalidateRequerySuggested();
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

		private string _filterName;
		public string FilterName
		{
			get { return _filterName; }
			set
			{
				_filterName = value;
				OnPropertyChanged(() => FilterName);
			}
		}
		private bool _isPeriodReport;
		public bool IsPeriodReport
		{
			get { return _isPeriodReport; }
			set
			{
				_isPeriodReport = value;
				OnPropertyChanged(() => IsPeriodReport);
			}
		}
		private ReportPeriodType _periodType;
		public ReportPeriodType PeriodType
		{
			get { return _periodType; }
			set
			{
				_periodType = value;
				OnPropertyChanged(() => PeriodType);
			}
		}

		private void BuildReport()
		{
			_reportProvider = SelectedReport is SKDReportViewModel ? ((SKDReportViewModel)SelectedReport).ReportProvider : null;
			if (_reportProvider != null)
				try
				{
					Model.ReportName = _reportProvider.GetType().Name;
					var filter = _reportProvider is IFilteredSKDReportProvider ? ((IFilteredSKDReportProvider)_reportProvider).GetFilter() : null;
					FilterName = filter == null ? null : filter.Name;
					if (filter != null)
						filter.UserUID = FiresecManager.CurrentUser.UID;
					var filterPeriod = filter as IReportFilterPeriod;
					IsPeriodReport = filterPeriod != null;
					if (IsPeriodReport)
						PeriodType = filterPeriod.PeriodType;
					Model.Build(filter);
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
					if (ApplicationService.ApplicationActivated)
						MessageBoxService.ShowException(ex, "Возникла ошибка при построении отчета");
					if (ex is CommunicationException)
						CreateClient();
				}
			else
			{
				IsPeriodReport = false;
				FilterName = null;
			}
		}

		private void Model_CreateDocumentError(object sender, FaultEventArgs e)
		{
			e.Handled = true;
			CommandManager.InvalidateRequerySuggested();
			MessageBoxService.ShowException(e.Fault);
		}
		private void Model_GetPageError(object sender, SimpleFaultEventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
			MessageBoxService.ShowException(e.Fault);
		}

		public RelayCommand ChangeFilterCommand { get; private set; }
		private void OnChangeFilter()
		{
			var provider = ((IFilteredSKDReportProvider)_reportProvider);
			var model = provider.GetFilterModel();
			var filterViewModel = new SKDReportFilterViewModel(provider.GetFilter(), model);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				provider.UpdateFilter(filterViewModel.Filter);
				BuildReport();
			}
		}
		private bool CanChangeFilter()
		{
			return _reportProvider is IFilteredSKDReportProvider;
		}

		public RelayCommand RefreshReportCommand { get; private set; }
		private void OnRefreshReport()
		{
			BuildReport();
		}
		private bool CanRefreshReport()
		{
			return SelectedReport != null && SelectedReport is SKDReportViewModel && ((SKDReportViewModel)SelectedReport).ReportProvider != null;
		}

		public RelayCommand<ZoomFitMode> FitPageSizeCommand { get; private set; }
		private void OnFitPageSize(ZoomFitMode fitMode)
		{
			// SKDDEV-409 пункт 2 - приведение масштаба отчета к исходному размеру (100%)
			if (fitMode == ZoomFitMode.WholePage)
			{
				Model.ZoomMode = null;
				Model.SetZoom(100);
				return;
			}

			Model.ZoomMode = new ZoomFitModeItem(fitMode);
		}
		private bool CanFitPageSize(ZoomFitMode fitMode)
		{
			return Model.PrintCommand.CanExecute(null);
		}

		private int _elapsedTime;
		public int ElapsedTime
		{
			get { return _elapsedTime; }
			set
			{
				_elapsedTime = value;
				OnPropertyChanged(() => ElapsedTime);
			}
		}
		private void OnProgressChanged()
		{
			if (Model.ProgressVisibility)
			{
				ElapsedTime = 0;
				_timer.Start();
			}
			else
				_timer.Stop();
		}

		public void OnUserChanged(UserChangedEventArgs args)
		{
			Model.Clear();
			IsPeriodReport = false;
			FilterName = null;
		}
	}
}
