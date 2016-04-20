using GKProcessor;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace FiresecService.Models
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }
		Dispatcher _dispatcher;
		public ServerTasksViewModel ServerTasksViewModel { get; private set; }

		public LicenseViewModel LicenseViewModel { get; private set; }

		public MainViewModel()
		{
			Current = this;
			_dispatcher = Dispatcher.CurrentDispatcher;
			Clients = new ObservableCollection<ClientViewModel>();
			ServerTasksViewModel = new Models.ServerTasksViewModel();
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);
			Logs = new ObservableCollection<LogViewModel>();
			GKLifecycles = new ObservableCollection<GKLifecycleViewModel>();
			LicenseViewModel = new LicenseViewModel();
			LicenseManager.LicenseChanged += LicenseHelper_LicenseChanged;
			GKLifecycleManager.GKLifecycleChangedEvent += On_GKLifecycleChangedEvent;
			ClientPolls = new ObservableCollection<ClientPollViewModel>();
			SetTitle();
			Icon = @"../Server.ico";
		}

		void SetTitle()
		{
			Title = LicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.Demonstration ?
				"Сервер приложений Глобал [Демонстрационный режим]" :
				"Сервер приложений Глобал";
		}

		void LicenseHelper_LicenseChanged()
		{
			SetTitle();
		}

		void MessageBoxHandler(MessageBoxViewModel viewModel, bool isModal)
		{
			_dispatcher.Invoke((Action)(() =>
			{
				var startupMessageBoxViewModel = new ServerMessageBoxViewModel(viewModel.Title, viewModel.Message, viewModel.MessageBoxButton, viewModel.MessageBoxImage, viewModel.IsException);
				if (isModal)
					DialogService.ShowModalWindow(startupMessageBoxViewModel);
				else
					DialogService.ShowWindow(startupMessageBoxViewModel);
				viewModel.Result = startupMessageBoxViewModel.Result;
			}));
		}

		string _status;
		string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged(() => Status);
			}
		}

		#region Clients
		public ObservableCollection<ClientViewModel> Clients { get; private set; }

		ClientViewModel _selectedClient;
		public ClientViewModel SelectedClient
		{
			get { return _selectedClient; }
			set
			{
				_selectedClient = value;
				OnPropertyChanged(() => SelectedClient);
			}
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = new ClientViewModel(clientCredentials);
				Clients.Add(connectionViewModel);
			}));
		}
		public void RemoveClient(Guid uid)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					Clients.Remove(connectionViewModel);
			}));
		}
		public void EditClient(Guid uid, string userName)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					connectionViewModel.FriendlyUserName = userName;
			}));
		}
		#endregion Clients

		#region Logs
		public void AddLog(string message, bool isError)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				LastLog = message;
				var logViewModel = new LogViewModel(message, isError);
				Logs.Add(logViewModel);
				if (Logs.Count > 1000)
					Logs.RemoveAt(0);
			}));
		}

		string _lastLog = "";
		public string LastLog
		{
			get { return _lastLog; }
			set
			{
				_lastLog = value;
				OnPropertyChanged(() => LastLog);
			}
		}

		public ObservableCollection<LogViewModel> Logs { get; private set; }

		LogViewModel _selectedLog;
		public LogViewModel SelectedLog
		{
			get { return _selectedLog; }
			set
			{
				_selectedLog = value;
				OnPropertyChanged(() => SelectedLog);
			}
		}
		#endregion Logs

		#region Address

		public static void SetLocalAddress(string address)
		{
			if (Current != null)
			{
				Current._dispatcher.BeginInvoke((Action)(() => { Current.LocalAddress = address; }));
			}
		}

		public static void SetRemoteAddress(string address)
		{
			if (Current != null)
			{
				Current._dispatcher.BeginInvoke((Action)(() => { Current.RemoteAddress = address; }));
			}
		}

		public static void SetReportAddress(string address)
		{
			if (Current != null)
			{
				Current._dispatcher.BeginInvoke((Action)(() => { Current.ReportAddress = address; }));
			}
		}

		string _localAddress;
		public string LocalAddress
		{
			get { return _localAddress; }
			set
			{
				_localAddress = value;
				OnPropertyChanged(() => LocalAddress);
			}
		}

		string _remoteAddress;
		public string RemoteAddress
		{
			get { return _remoteAddress; }
			set
			{
				_remoteAddress = value;
				OnPropertyChanged(() => RemoteAddress);
			}
		}

		string _reportAddress;
		public string ReportAddress
		{
			get { return _reportAddress; }
			set
			{
				_reportAddress = value;
				OnPropertyChanged(() => ReportAddress);
			}
		}
		#endregion Address

		#region GK Lifecycle
		ObservableCollection<GKLifecycleViewModel> _gkLifecycles;
		public ObservableCollection<GKLifecycleViewModel> GKLifecycles
		{
			get { return _gkLifecycles; }
			set
			{
				_gkLifecycles = value;
				OnPropertyChanged(() => GKLifecycles);
			}
		}

		GKLifecycleViewModel _selectedGKLifecycle;
		public GKLifecycleViewModel SelectedGKLifecycle
		{
			get { return _selectedGKLifecycle; }
			set
			{
				_selectedGKLifecycle = value;
				OnPropertyChanged(() => SelectedGKLifecycle);
			}
		}

		void On_GKLifecycleChangedEvent(GKLifecycleInfo gkLifecycleInfo)
		{
			_dispatcher.Invoke((Action)(() =>
			{
				var gkLifecycleViewModel = GKLifecycles.FirstOrDefault(x => x.GKLifecycleInfo.UID == gkLifecycleInfo.UID);
				if (gkLifecycleViewModel == null)
				{
					gkLifecycleViewModel = AddGKViewModel(gkLifecycleInfo);
				}
				else
				{
					gkLifecycleViewModel.Update(gkLifecycleInfo);
				}
				//SelectedGKViewModel = gkViewModel;
			}));
		}

		GKLifecycleViewModel AddGKViewModel(GKLifecycleInfo gkLifecycleInfo)
		{
			var gkViewModel = new GKLifecycleViewModel(gkLifecycleInfo);
			GKLifecycles.Insert(0, gkViewModel);
			if (GKLifecycles.Count > 20)
				GKLifecycles.RemoveAt(20);
			return gkViewModel;
		}
		#endregion GK

		#region Polling
		public ObservableCollection<ClientPollViewModel> ClientPolls { get; private set; }

		public void OnPoll(Guid uid)
		{
			_dispatcher.Invoke((Action)(() =>
				{
					var now = DateTime.Now;
					var clientInfo = FiresecService.Service.ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
					var client = clientInfo == null ? "" : string.Format("{0} / {1} / {2}", clientInfo.ClientCredentials.ClientType.ToDescription(), clientInfo.ClientCredentials.ClientIpAddress, clientInfo.ClientCredentials.FriendlyUserName);
					var clientPoll = ClientPolls.FirstOrDefault(x => x.Client == client && x.UID == uid);
					if (clientPoll == null)
					{
						clientPoll = new ClientPollViewModel { UID = uid, Client = client };
						clientPoll.FirstPollTime = now;
						ClientPolls.Add(clientPoll);
					}
					if (clientInfo != null)
						clientPoll.CallbackIndex = clientInfo.CallbackIndex;
					clientPoll.LastPollTime = now;

				}));
		}
		#endregion

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
	}
}