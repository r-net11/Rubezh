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

namespace FiresecService.Presenters
{
	public class MainPresenter : ApplicationPresenter
	{
		public MainPresenter(IMainView view)
		{
			View = view;

			Logs = new ObservableCollection<LogViewModel>();
			LastLog = String.Empty;

			_bindingSourceClients = new BindingSource();
			_clients = new List<ClientViewModel>();
			//Clients = new ObservableCollection<ClientViewModel>();
			//Clients.CollectionChanged += Clients_CollectionChanged;
			_bindingSourceClients.DataSource = null;
			_bindingSourceClients.DataSource = _clients;
			_bindingSourceClients.ListChanged += EventHandler_bindingSourceClients_ListChanged;
			ClientPolls = new ObservableCollection<ClientPollViewModel>();

			View.Title = "Сервер приложений Глобал";
			View.CommandDisconnectActivated += EventHandler_View_CommandDisconnectActivated;
			View.Clients = _bindingSourceClients;
			View.EnableMenuDisconnect = false;

			Current = this;
		}

		#region Fields And Properties

		public IMainView View { get; private set; }

		public ObservableCollection<LogViewModel> Logs { get; private set; }

		public string LastLog
		{
			get { return View.LastLog; }
			set { View.LastLog = value; }
		}

		public ServerTasksViewModel ServerTasksViewModel { get; private set; }

		#endregion

		#region Methods
		
		public void AddLog(string message, bool isError)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				LastLog = message;
				var logViewModel = new LogViewModel(message, isError);
				Logs.Add(logViewModel);
				if (Logs.Count > 1000)
					Logs.RemoveAt(0);
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
				//OnPropertyChanged(() => ReportAddress);
			}
		}
		#endregion Address

		#region Clients

		BindingSource _bindingSourceClients;
		List<ClientViewModel> _clients;

		//public ObservableCollection<ClientViewModel> Clients { get; private set; }

		ClientViewModel _selectedClient;
		public ClientViewModel SelectedClient
		{
			get 
			{
				//return _selectedClient;
				return (ClientViewModel)_bindingSourceClients.Current;
			}
			set
			{
				_selectedClient = value;
				//OnPropertyChanged(() => SelectedClient);
			}
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = new ClientViewModel(clientCredentials);
				//Clients.Add(connectionViewModel);
				_bindingSourceClients.Add(connectionViewModel);
			}));
		}
		public void RemoveClient(Guid uid)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				//var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
				//if (connectionViewModel != null)
				//	Clients.Remove(connectionViewModel);

				var connectionViewModel = _clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					_bindingSourceClients.Remove(connectionViewModel);
			}));
		}
		public void EditClient(Guid uid, string userName)
		{
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				//var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
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
							_bindingSourceClients.Add((ClientViewModel)item);
						}
						break;
					}
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					{
						foreach (var item in e.OldItems)
						{
							_bindingSourceClients.Remove((ClientViewModel)item);
						}
						break;
					}
			}

		}

		#endregion Clients

		#region GK Lifecycle

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

		//void On_GKLifecycleChangedEvent(GKLifecycleInfo gkLifecycleInfo)
		//{
		//	_dispatcher.Invoke((Action)(() =>
		//	{
		//		var gkLifecycleViewModel = GKLifecycles.FirstOrDefault(x => x.GKLifecycleInfo.UID == gkLifecycleInfo.UID);
		//		if (gkLifecycleViewModel == null)
		//		{
		//			gkLifecycleViewModel = AddGKViewModel(gkLifecycleInfo);
		//		}
		//		else
		//		{
		//			gkLifecycleViewModel.Update(gkLifecycleInfo);
		//		}
		//		//SelectedGKViewModel = gkViewModel;
		//	}));
		//}

		//GKLifecycleViewModel AddGKViewModel(GKLifecycleInfo gkLifecycleInfo)
		//{
		//	var gkViewModel = new GKLifecycleViewModel(gkLifecycleInfo);
		//	GKLifecycles.Insert(0, gkViewModel);
		//	if (GKLifecycles.Count > 20)
		//		GKLifecycles.RemoveAt(20);
		//	return gkViewModel;
		//}

		#endregion GK

		#region Polling

		public ObservableCollection<ClientPollViewModel> ClientPolls { get; private set; }

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
	}
}