using RubezhService.Models;
using RubezhService.Service;
using RubezhService.Views;
using GKProcessor;
using Infrastructure.Common.License;
using RubezhAPI;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RubezhService.Presenters
{
	public class MainPresenter
	{
		public MainPresenter(IMainView view)
		{
			SyncContext = SynchronizationContext.Current;

			View = view;

			_clients = new List<Client>();
			_bindingSourceClients = new BindingSource();
			_bindingSourceClients.DataSource = null;
			_bindingSourceClients.DataSource = _clients;

			_logs = new List<Log>();
			_bindingSourceLogs = new BindingSource();
			_bindingSourceLogs.DataSource = null;
			_bindingSourceLogs.DataSource = _logs;

			_gkLifecycles = new System.ComponentModel.BindingList<GKLifecycle>();
			_bindingSourceLifecycle = new BindingSource();
			_bindingSourceLifecycle.DataSource = null;
			_bindingSourceLifecycle.DataSource = _gkLifecycles;
			GKLifecycleManager.GKLifecycleChangedEvent += On_GKLifecycleChangedEvent;

			ClientPolls = new List<ClientPolling>();
			_bindingSourceClientPolls = new BindingSource();
			_bindingSourceClientPolls.DataSource = null;
			_bindingSourceClientPolls.DataSource = ClientPolls;

			ServerTasks = new List<ServerTaskModel>();
			_bindingSourceOperations = new BindingSource();
			_bindingSourceOperations.DataSource = null;
			_bindingSourceOperations.DataSource = ServerTasks;

			License = new License();
			License.LicenseChanged += EventHandler_License_LicenseChanged;

			View.Title = "Сервер приложений Глобал";
			View.CommandDisconnectActivated += EventHandler_View_CommandDisconnectActivated;
			View.ClientsContext = _bindingSourceClients;
			View.EnableMenuDisconnect = false;
			View.LogsContext = _bindingSourceLogs;
			View.GkLifecyclesContext = _bindingSourceLifecycle;
			View.ClientPollsContext = _bindingSourceClientPolls;
			View.OperationsContext = _bindingSourceOperations;
			View.LicenseMode = License.LicenseInfo.LicenseMode;
			View.RemoteClientsCount = License.LicenseInfo.RemoteClientsCount;
			View.HasFirefighting = License.LicenseInfo.HasFirefighting;
			View.HasGuard = License.LicenseInfo.HasGuard;
			View.HasSKD = License.LicenseInfo.HasSKD;
			View.HasVideo = License.LicenseInfo.HasVideo;
			View.HasOpcServer = License.LicenseInfo.HasOpcServer;
			View.InitialKey = License.InitialKey;
			View.ClickLoadLicense += EventHandler_View_ClickLoadLicense;

			LastLog = String.Empty;
			Current = this;
		}

		#region Fields And Properties

		public static MainPresenter Current { get; set; }

		public SynchronizationContext SyncContext { get; private set; }

		public IMainView View { get; private set; }

		#endregion

		#region Logs

		List<Log> _logs;
		BindingSource _bindingSourceLogs;

		public string LastLog
		{
			get { return View.LastLog; }
			set { View.LastLog = value; }
		}

		public void AddLog(string message, bool isError)
		{
			SyncContext.Post(state =>
			{
				LastLog = message;
				var logViewModel = new Log(message, isError);
				_bindingSourceLogs.Add(logViewModel);
				if (_bindingSourceLogs.Count > 1000)
					_bindingSourceLogs.RemoveAt(0);
			}, null);
		}

		#endregion

		#region Address

		public static void SetRemoteAddress(string address)
		{
			if (Current != null)
			{
				Current.SyncContext.Post(state => { Current.RemoteAddress = address; }, null);
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
			}
		}

		#endregion Address

		#region Clients

		BindingSource _bindingSourceClients;
		List<Client> _clients;

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
			SyncContext.Post(state =>
			{
				var connectionViewModel = new Client(clientCredentials);
				_bindingSourceClients.Add(connectionViewModel);
				View.EnableMenuDisconnect = _bindingSourceClients.Count > 0;
			}, null);
		}
		public void RemoveClient(Guid uid)
		{
			SyncContext.Post(state =>
			{
				var connectionViewModel = _clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					_bindingSourceClients.Remove(connectionViewModel);
				View.EnableMenuDisconnect = _bindingSourceClients.Count > 0;
			}, null);
		}
		public void EditClient(Guid uid, string userName)
		{
			SyncContext.Post(state =>
			{
				var connectionViewModel = _clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					connectionViewModel.FriendlyUserName = userName;
			}, null);
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

		#endregion Clients

		#region GK Lifecycle

		BindingSource _bindingSourceLifecycle;

		System.ComponentModel.BindingList<GKLifecycle> _gkLifecycles;
		public System.ComponentModel.BindingList<GKLifecycle> GKLifecycles
		{
			get { return _gkLifecycles; }
			set
			{
				_gkLifecycles = value;
			}
		}

		void On_GKLifecycleChangedEvent(GKLifecycleInfo gkLifecycleInfo)
		{
			SyncContext.Send(state =>
			{
				var gkLifecycle = GKLifecycles.FirstOrDefault(x => x.GKLifecycleInfo.UID == gkLifecycleInfo.UID);
				if (gkLifecycle == null)
				{
					gkLifecycle = AddGKViewModel(gkLifecycleInfo);
				}
				else
				{
					gkLifecycle.Update(gkLifecycleInfo);
				}
			}, null);
		}

		GKLifecycle AddGKViewModel(GKLifecycleInfo gkLifecycleInfo)
		{
			var gkViewModel = new GKLifecycle(gkLifecycleInfo);
			_bindingSourceLifecycle.Insert(0, gkViewModel);
			if (GKLifecycles.Count > 20)
				_bindingSourceLifecycle.RemoveAt(20);
			return gkViewModel;
		}

		#endregion GK

		#region Polling

		List<ClientPolling> ClientPolls { get; set; }
		BindingSource _bindingSourceClientPolls;

		public void OnPoll(Guid uid)
		{
			SyncContext.Send(state =>
			{
				var now = DateTime.Now;
				var clientInfo = RubezhService.Service.ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
				var client = clientInfo == null ? "" :
					string.Format("{0} / {1} / {2}", clientInfo.ClientCredentials.ClientType.ToDescription(),
					clientInfo.ClientCredentials.ClientIpAddress, clientInfo.ClientCredentials.FriendlyUserName);
				var clientPoll = ClientPolls.FirstOrDefault(x => x.Client == client && x.UID == uid);
				if (clientPoll == null)
				{
					clientPoll = new ClientPolling { UID = uid, Client = client };
					clientPoll.FirstPollTime = now;
					_bindingSourceClientPolls.Add(clientPoll);
				}
				if (clientInfo != null)
					clientPoll.CallbackIndex = clientInfo.CallbackIndex;
				clientPoll.LastPollTime = now;
				_bindingSourceClientPolls.EndEdit();
				_bindingSourceClientPolls.ResetBindings(false);
			}, null);
		}

		#endregion

		#region Operations

		List<ServerTaskModel> ServerTasks { get; set; }

		BindingSource _bindingSourceOperations;

		public void AddTask(ServerTask serverTask)
		{
			SyncContext.Post(state =>
			{
				var serverTaskModel = new ServerTaskModel(serverTask);
				_bindingSourceOperations.Add(serverTaskModel);
			}, null);
		}

		public void RemoveTask(ServerTask serverTask)
		{
			SyncContext.Post(state =>
			{
				var serverTaskViewModel = ServerTasks.FirstOrDefault(x => x.Task.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					_bindingSourceOperations.Remove(serverTaskViewModel);
			}, null);
		}
		public void EditTask(ServerTask serverTask)
		{
			SyncContext.Post(state =>
			{
				var serverTaskViewModel = ServerTasks.FirstOrDefault(x => x.Task.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					serverTaskViewModel.Task = serverTask;
			}, null);
		}

		#endregion

		#region License

		public License License { get; private set; }

		void EventHandler_View_ClickLoadLicense(object sender, EventArgs e)
		{
			License.OnLoadLicenseCommand();
		}

		void EventHandler_License_LicenseChanged(object sender, EventArgs e)
		{
			var target = (License)sender;

			View.LicenseMode = target.LicenseInfo.LicenseMode;
			View.RemoteClientsCount = target.LicenseInfo.RemoteClientsCount;
			View.HasFirefighting = target.LicenseInfo.HasFirefighting;
			View.HasGuard = target.LicenseInfo.HasGuard;
			View.HasSKD = target.LicenseInfo.HasSKD;
			View.HasVideo = target.LicenseInfo.HasVideo;
			View.HasOpcServer = target.LicenseInfo.HasOpcServer;

			View.Title = LicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.Demonstration ?
				"Сервер приложений Глобал [Демонстрационный режим]" :
				"Сервер приложений Глобал";
		}

		#endregion
	}
}