using Common;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.Xpf.Printing;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.SKD.ReportFilters;
using RubezhClient;
using System;
using System.ServiceModel;
using System.Windows.Input;
using System.Windows.Threading;
using DialogService = Infrastructure.Common.Windows.DialogService;

namespace ReportsModule.ViewModels
{
	public class SKDReportPresenterViewModel : BaseViewModel
	{
		ReportServiceClientFactory Factory { get; set; }
		ISKDReportProvider ReportProvider
		{
			get { return SelectedReport is SKDReportViewModel ? ((SKDReportViewModel)SelectedReport).ReportProvider : null; }
		}
		private DispatcherTimer _timer;
		public SKDReportPresenterViewModel()
		{
			ChangeFilterCommand = new RelayCommand(OnChangeFilter, CanChangeFilter);
			RefreshReportCommand = new RelayCommand(OnRefreshReport, CanRefreshReport);
			FitPageSizeCommand = new RelayCommand<ZoomFitMode>(OnFitPageSize, CanFitPageSize);

			_timer = new DispatcherTimer();
			_timer.Tick += (s, e) => ElapsedTime++;
			_timer.Interval = TimeSpan.FromSeconds(1);
		}

		public void CreateClient()
		{
			var address = GlobalSettingsHelper.GlobalSettings.RemoteAddress;
			if (string.IsNullOrEmpty(address) || address == "localhost" || address == "127.0.0.1")
				address = ConnectionSettingsManager.GetIPAddress();
			var reportServerAddress = "net.tcp://" + address + ":" + GlobalSettingsHelper.GlobalSettings.ReportRemotePort.ToString() + "/ReportFiresecService/";
			var endpoint = new EndpointAddress(reportServerAddress);
			var binding = BindingHelper.CreateBindingFromAddress(reportServerAddress);
			Factory = new ReportServiceClientFactory(endpoint, binding);
			CreateModel();
		}
		void CreateModel()
		{
			Model = new XReportServicePreviewModel()
			{
				ServiceClientFactory = Factory,
				IsParametersPanelVisible = false,
				AutoShowParametersPanel = false,
				IsDocumentMapVisible = false,
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
					var provider = ((IFilteredSKDReportProvider)ReportProvider);
					var model = provider.GetFilterModel();
					var filterViewModel = new SKDReportFilterViewModel(provider.GetFilter(), model);
					filterViewModel.UpdateFilter(filterViewModel.Filter);
					provider.UpdateFilter(filterViewModel.Filter);
					BuildReport();
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
			if (ReportProvider != null)
				try
				{
					using (new WaitWrapper())
					{
						CreateModel();
						Model.ReportName = ReportProvider.GetType().Name;
						var filter = ReportProvider is IFilteredSKDReportProvider ? ((IFilteredSKDReportProvider)ReportProvider).GetFilter() : null;
						FilterName = filter == null ? null : filter.Name;
						if (filter != null)
							filter.UserUID = ClientManager.CurrentUser.UID;
						var filterPeriod = filter as IReportFilterPeriod;
						IsPeriodReport = filterPeriod != null;
						if (IsPeriodReport)
							PeriodType = filterPeriod.PeriodType;
						Model.Build(filter);
					}
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "SKDReportPresenterViewModel.BuidlReport");
					if (ApplicationService.ApplicationActivated)
						MessageBoxService.ShowError("Возникла ошибка при построении отчета.");
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
			if (e.Fault.Message.Contains("Could not connect to"))
				MessageBoxService.ShowError("Не удалось подключиться к серверу отчетов.");
			else
				MessageBoxService.ShowError("Возникла ошибка при построении отчета.");
			Logger.Error(e.Fault, "SKDReportPresenterViewModel.Model_CreateDocumentError");
		}
		private void Model_GetPageError(object sender, SimpleFaultEventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
			MessageBoxService.ShowError("Возникла ошибка при получении страницы отчета.");
			Logger.Error(e.Fault, "SKDReportPresenterViewModel.Model_GetPageError");
		}

		public RelayCommand ChangeFilterCommand { get; private set; }
		private void OnChangeFilter()
		{
			var provider = ((IFilteredSKDReportProvider)ReportProvider);
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
			return ReportProvider is IFilteredSKDReportProvider;
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
	}
}