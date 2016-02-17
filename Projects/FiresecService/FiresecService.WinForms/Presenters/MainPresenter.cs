using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecService.Views;
using FiresecService.ViewModels;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using RubezhAPI;
using RubezhAPI.Models;
using FiresecService.Service;
using GKProcessor;
using RubezhAPI.License;

namespace FiresecService.Presenters
{
	public class MainPresenter : ApplicationPresenter
	{
		public MainPresenter(IMainView view)
		{
			View = view;

			_clients = new List<Client>();
			_bindingSourceClients = new BindingSource();
			_bindingSourceClients.DataSource = null;
			_bindingSourceClients.DataSource = _clients;
			_bindingSourceClients.ListChanged += EventHandler_bindingSourceClients_ListChanged;

			_logs = new List<LogViewModel>();
			_bindingSourceLogs = new BindingSource();
			_bindingSourceLogs.DataSource = null;
			_bindingSourceLogs.DataSource = _logs;

			_gkLifecycles = new List<GKLifecycle>();
			_bindingSourceLifecycle = new BindingSource();
			_bindingSourceLifecycle.DataSource = null;
			_bindingSourceLifecycle.DataSource = _gkLifecycles;
			GKLifecycleManager.GKLifecycleChangedEvent += On_GKLifecycleChangedEvent;

			ClientPolls = new List<ClientPolling>();
			_bindingSourceClientPolls = new BindingSource();
			_bindingSourceClientPolls.DataSource = null;
			_bindingSourceClientPolls.DataSource = ClientPolls;

			ServerTasks = new List<ServerTaskViewModel>();
			_bindingSourceOperations = new BindingSource();
			_bindingSourceOperations.DataSource = null;
			_bindingSourceOperations.DataSource = ServerTasks;

			LicenseViewModel = new LicenseViewModel();
			LicenseViewModel.PropertyChanged += EventHandler_LicenseViewModel_PropertyChanged;
			LicenseManager.LicenseChanged += EventHandler_LicenseManager_LicenseChanged;

			View.Title = "Сервер приложений Глобал";
			View.CommandDisconnectActivated += EventHandler_View_CommandDisconnectActivated;
			View.ClientsContext = _bindingSourceClients;
			View.EnableMenuDisconnect = false;
			View.LogsContext = _bindingSourceLogs;
			View.GkLifecyclesContext = _bindingSourceLifecycle;
			View.ClientPollsContext = _bindingSourceClientPolls;
			View.OperationsContext = _bindingSourceOperations;
			View.LicenseMode = LicenseViewModel.LicenseInfo.LicenseMode;
			View.RemoteClientsCount = LicenseViewModel.LicenseInfo.RemoteClientsCount;
			View.HasFirefighting = LicenseViewModel.LicenseInfo.HasFirefighting;
			View.HasGuard = LicenseViewModel.LicenseInfo.HasGuard;
			View.HasSKD = LicenseViewModel.LicenseInfo.HasSKD;
			View.HasVideo = LicenseViewModel.LicenseInfo.HasVideo;
			View.HasOpcServer = LicenseViewModel.LicenseInfo.HasOpcServer;
			View.InitialKey = LicenseViewModel.InitialKey;
			View.ClickLoadLicense += EventHandler_View_ClickLoadLicense;

			LastLog = String.Empty;
			Current = this;
		}

		#region Fields And Properties

		public IMainView View { get; private set; }

		#endregion

		#region Logs
		//public ObservableCollection<LogViewModel> Logs { get; private set; }
		List<LogViewModel> _logs;
		BindingSource _bindingSourceLogs;

		public string LastLog
		{
			get { return View.LastLog; }
			set { View.LastLog = value; }
		}

		public void AddLog(string message, bool isError)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				LastLog = message;
				var logViewModel = new LogViewModel(message, isError);
				//Logs.Add(logViewModel);
				//if (Logs.Count > 1000)
				//	Logs.RemoveAt(0);
				_bindingSourceLogs.Add(logViewModel);
				if (_bindingSourceLogs.Count > 1000)
					_bindingSourceLogs.RemoveAt(0);
			}));
		}

		#endregion

		#region Address

		public static void SetLocalAddress(string address)
		{
			if (Current != null)
			{
				Current.FormDispatcher.BeginInvoke((Action)(() => { Current.LocalAddress = address; }));
			}
		}

		public static void SetRemoteAddress(string address)
		{
			if (Current != null)
			{
				Current.FormDispatcher.BeginInvoke((Action)(() => { Current.RemoteAddress = address; }));
			}
		}

		public static void SetReportAddress(string address)
		{
			if (Current != null)
			{
				Current.FormDispatcher.BeginInvoke((Action)(() => { Current.ReportAddress = address; }));
			}
		}

		string _localAddress;
		public string LocalAddress
		{
			get { return _localAddress; }
			set
			{
				_localAddress = value;
				View.LocalAddress = _localAddress;
				//OnPropertyChanged(() => LocalAddress);
			}
		}

		string _remoteAddress;
		public string RemoteAddress
		{
			get { return _remoteAddress; }
			set
			{
				_remoteAddress = value;
				View.RemoteAddress = _remoteAddress;
				//OnPropertyChanged(() => RemoteAddress);
			}
		}

		string _reportAddress;
		public string ReportAddress
		{
			get { return _reportAddress; }
			set
			{
				_reportAddress = value;
				View.ReportAddress = _reportAddress;
				//OnPropertyChanged(() => ReportAddress);
			}
		}
		#endregion Address

		#region Clients

		BindingSource _bindingSourceClients;
		List<Client> _clients;

		//public ObservableCollection<ClientViewModel> Clients { get; private set; }

		Client _selectedClient;
		public Client SelectedClient
		{
			get 
			{
				return (Client)_bindingSourceClients.Current;
			}
			set
			{
				_selectedClient = value;
			}
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = new Client(clientCredentials);
				_bindingSourceClients.Add(connectionViewModel);
			}));
		}
		public void RemoveClient(Guid uid)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = _clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					_bindingSourceClients.Remove(connectionViewModel);
			}));
		}
		public void EditClient(Guid uid, string userName)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = _clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					connectionViewModel.FriendlyUserName = userName;
			}));
		}

		void EventHandler_View_CommandDisconnectActivated(object sender, EventArgs e)
		{
			//TODO: передалать в сервис
			var connection = SelectedClient;
			var text = string.Format("Вы действительно хотите отключить клиента <{0} / {1} / {2}> от сервера?",
				connection.ClientType, connection.IpAddress, connection.FriendlyUserName);
			var result = MessageBox.Show(text, "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				ClientsManager.Remove(connection.UID);
			}
		}

		void EventHandler_bindingSourceClients_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			BindingSource control = (BindingSource)sender;
			View.EnableMenuDisconnect = control.Count > 0;
		}

		void EventHandler_Clients_CollectionChanged(object sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					{
						foreach (var item in e.NewItems)
						{
							_bindingSourceClients.Add((Client)item);
						}
						break;
					}
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					{
						foreach (var item in e.OldItems)
						{
							_bindingSourceClients.Remove((Client)item);
						}
						break;
					}
			}

		}

		#endregion Clients

		#region GK Lifecycle

		BindingSource _bindingSourceLifecycle;

		List<GKLifecycle> _gkLifecycles;
		public List<GKLifecycle> GKLifecycles
		{
			get { return _gkLifecycles; }
			set
			{
				_gkLifecycles = value;
			}
		}

		//ObservableCollection<GKLifecycleViewModel> _gkLifecycles;
		//public ObservableCollection<GKLifecycleViewModel> GKLifecycles
		//{
		//	get { return _gkLifecycles; }
		//	set
		//	{
		//		_gkLifecycles = value;
		//		OnPropertyChanged(() => GKLifecycles);
		//	}
		//}

		//GKLifecycleViewModel _selectedGKLifecycle;
		//public GKLifecycleViewModel SelectedGKLifecycle
		//{
		//	get { return _selectedGKLifecycle; }
		//	set
		//	{
		//		_selectedGKLifecycle = value;
		//		OnPropertyChanged(() => SelectedGKLifecycle);
		//	}
		//}

		void On_GKLifecycleChangedEvent(GKLifecycleInfo gkLifecycleInfo)
		{
			FormDispatcher.Invoke((Action)(() =>
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

		GKLifecycle AddGKViewModel(GKLifecycleInfo gkLifecycleInfo)
		{
			var gkViewModel = new GKLifecycle(gkLifecycleInfo);
			//GKLifecycles.Insert(0, gkViewModel);
			_bindingSourceLifecycle.Insert(0, gkViewModel);
			if (GKLifecycles.Count > 20)
				//GKLifecycles.RemoveAt(20);
				_bindingSourceLifecycle.RemoveAt(20);
			return gkViewModel;
		}

		#endregion GK

		#region Polling

		//public ObservableCollection<ClientPollViewModel> ClientPolls { get; private set; }
		List<ClientPolling> ClientPolls { get; set; }
		BindingSource _bindingSourceClientPolls;

		public void OnPoll(Guid uid)
		{
			FormDispatcher.Invoke((Action)(() =>
			{
				var now = DateTime.Now;
				var clientInfo = FiresecService.Service.ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
				var client = clientInfo == null ? "" : 
					string.Format("{0} / {1} / {2}", clientInfo.ClientCredentials.ClientType.ToDescription(), 
					clientInfo.ClientCredentials.ClientIpAddress, clientInfo.ClientCredentials.FriendlyUserName);
				var clientPoll = ClientPolls.FirstOrDefault(x => x.Client == client && x.UID == uid);
				if (clientPoll == null)
				{
					clientPoll = new ClientPolling { UID = uid, Client = client };
					clientPoll.FirstPollTime = now;
					//ClientPolls.Add(clientPoll);
					_bindingSourceClientPolls.Add(clientPoll);
				}
				if (clientInfo != null)
					clientPoll.CallbackIndex = clientInfo.CallbackIndex;
				clientPoll.LastPollTime = now;

			}));
		}

		#endregion

		#region Operations
		
		//public ServerTasksViewModel ServerTasksViewModel { get; private set; }
		List<ServerTaskViewModel> ServerTasks { get; set; }

		BindingSource _bindingSourceOperations;

		public void AddTask(ServerTask serverTask)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = new ServerTaskViewModel(serverTask);
				_bindingSourceOperations.Add(serverTaskViewModel);
			}));
		}
		public void RemoveTask(ServerTask serverTask)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = ServerTasks.FirstOrDefault(x => x.ServerTask.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					//ServerTasks.Remove(serverTaskViewModel);
					_bindingSourceOperations.Remove(serverTaskViewModel);
			}));
		}
		public void EditTask(ServerTask serverTask)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = ServerTasks.FirstOrDefault(x => x.ServerTask.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					serverTaskViewModel.ServerTask = serverTask;
			}));
		}

		#endregion

		#region License
		
		public LicenseViewModel LicenseViewModel { get; private set; }

		void EventHandler_View_ClickLoadLicense(object sender, EventArgs e)
		{
			LicenseViewModel.OnLoadLicenseCommand();
		}

		void EventHandler_LicenseViewModel_PropertyChanged(object sender, 
			System.ComponentModel.PropertyChangedEventArgs e)
		{
			var target = (LicenseViewModel)sender;
			
			if (e.PropertyName == "LicenseInfo")
			{
				View.LicenseMode = target.LicenseInfo.LicenseMode;
				View.RemoteClientsCount = target.LicenseInfo.RemoteClientsCount;
				View.HasFirefighting = target.LicenseInfo.HasFirefighting;
				View.HasGuard = target.LicenseInfo.HasGuard;
				View.HasSKD = target.LicenseInfo.HasSKD;
				View.HasVideo = target.LicenseInfo.HasVideo;
				View.HasOpcServer = target.LicenseInfo.HasOpcServer;
			}
		}

		void EventHandler_LicenseManager_LicenseChanged()
		{
			View.Title = LicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.Demonstration ?
				"Сервер приложений Глобал [Демонстрационный режим]" :
				"Сервер приложений Глобал";
		}

		#endregion
	}
}