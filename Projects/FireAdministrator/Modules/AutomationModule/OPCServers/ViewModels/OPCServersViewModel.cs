using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Microsoft.Win32;

namespace AutomationModule.ViewModels
{
	public class OPCServersViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static OPCServersViewModel Current { get; private set; }
		public OPCUAClient client;

		public OPCServersViewModel()
		{
			Current = this;
			Menu = new OPCServersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);

			client = new OPCUAClient();
			ConnectionString = "opc.tcp://localhost:51510/UA/DemoServer";
			ServerUrl = "ServerUrl";
			AddSubscription = "AddSubscription";

			GetEndpontsCommand = new RelayCommand(OnGetEndponts);
			CreateOpcTcpSessionWithNoSecurityCommand = new RelayCommand(OnCreateOpcTcpSessionWithNoSecurity);
			DisconnectSessionCommand = new RelayCommand(OnDisconnectSession);
			ConfigurationCommand = new RelayCommand(OnConfiguration);
			BrowseMainCommand = new RelayCommand(OnBrowseMain);
			BrowseAllCommand = new RelayCommand(OnBrowseAll);
			AddSubscriptionCommand = new RelayCommand(OnAddSubscription);
			DeleteSubscriptionCommand = new RelayCommand(OnDeleteSubscription);
			GetSubscriptionListCommand = new RelayCommand(OnGetSubscriptionList);
		}

		public void Initialize()
		{
			OPCServers = new ObservableCollection<OPCServerViewModel>();
			foreach (var opcServer in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.OPCServers)
			{
				var opcServerViewModel = new OPCServerViewModel(opcServer);
				OPCServers.Add(opcServerViewModel);
			}
			SelectedOPCServer = OPCServers.FirstOrDefault();
		}

		ObservableCollection<OPCServerViewModel> _opcServers;
		public ObservableCollection<OPCServerViewModel> OPCServers
		{
			get { return _opcServers; }
			set
			{
				_opcServers = value;
				OnPropertyChanged(() => OPCServers);
			}
		}

		OPCServerViewModel _selectedOPCServer;
		public OPCServerViewModel SelectedOPCServer
		{
			get { return _selectedOPCServer; }
			set
			{
				_selectedOPCServer = value;
				OnPropertyChanged(() => SelectedOPCServer);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var opcServerDetailsViewModel = new OPCServerDetailsViewModel();
			if (DialogService.ShowModalWindow(opcServerDetailsViewModel))
			{
				var opcServerViewModel = new OPCServerViewModel(opcServerDetailsViewModel.OPCServer);
				OPCServers.Add(opcServerViewModel);
				SelectedOPCServer = OPCServers.FirstOrDefault();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = OPCServers.IndexOf(SelectedOPCServer);
			FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.OPCServers.Remove(SelectedOPCServer.OPCServer);
			OPCServers.Remove(SelectedOPCServer);
			index = Math.Min(index, OPCServers.Count - 1);
			if (index > -1)
				SelectedOPCServer = OPCServers[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var opcServerDetailsViewModel = new OPCServerDetailsViewModel(SelectedOPCServer.OPCServer);
			if (DialogService.ShowModalWindow(opcServerDetailsViewModel))
			{
				SelectedOPCServer.OPCServer = opcServerDetailsViewModel.OPCServer;
				SelectedOPCServer.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedOPCServer != null;
		}

		public void Select(Guid opcServerUID)
		{
			if (opcServerUID != Guid.Empty)
			{
				SelectedOPCServer = OPCServers.FirstOrDefault(item => item.OPCServer.Uid == opcServerUID);
			}
		}

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		string _connectionString;
		public string ConnectionString
		{
			get { return _connectionString; }
			set
			{
				_connectionString = value;
				OnPropertyChanged(() => ConnectionString);
			}
		}

		string _serverUrl;
		public string ServerUrl
		{
			get { return _serverUrl; }
			set
			{
				_serverUrl = value;
				OnPropertyChanged(() => ServerUrl);
			}
		}

		string _addSubscription;
		public string AddSubscription
		{
			get { return _addSubscription; }
			set
			{
				_addSubscription = value;
				OnPropertyChanged(() => AddSubscription);
			}
		}

		public RelayCommand GetEndpontsCommand { get; private set; }
		void OnGetEndponts()
		{
			try
			{
				var l = client.GetEndpoints(ConnectionString);
				if (l.Count == 0)
				{
					Text += "NO endpoints" + "\n";
					return;
				}
				ServerUrl = l[0];
				foreach (var name in l)
				{
					Text += name + "\n";
				}
			}
			catch (Exception mes)
			{
				Text += "Ошибка получения списка серверов " + mes.Message + "\n";
			}
		}

		public RelayCommand CreateOpcTcpSessionWithNoSecurityCommand { get; private set; }
		void OnCreateOpcTcpSessionWithNoSecurity()
		{
			if (client.m_session == null)
			{
				try
				{
					client.CreateOpcTcpSessionWithNoSecurity(ServerUrl);
					Text += "Session created " + ServerUrl + "\n";

				}
				catch (Exception mes)
				{
					Text += "Ошибка создания сессии " + mes.Message + "\n";
				}
			}
			else
				Text += "Session already created\n";
		}

		public RelayCommand DisconnectSessionCommand { get; private set; }
		void OnDisconnectSession()
		{
			if (client.m_session == null)
			{
				Text += "No created sessions\n";
			}
			else
			{
				var s = client.m_session.Url;
				client.DisconnectSession();
				Text += "Session disconnected " + s + "\n";
			}
		}

		public RelayCommand ConfigurationCommand { get; private set; }
		void OnConfiguration()
		{
			var cc = new ConfigClass();
			var t = cc.LoadApplicationConfiguration();
			if (t) Text += "Configuratrion loaded\n";
		}

		public RelayCommand BrowseMainCommand { get; private set; }
		void OnBrowseMain()
		{
			try
			{
				var l = client.BrowseMain();
				if (l.Count == 0)
				{
					Text += "NO nodes" + "\n";
					return;
				}
				//ServerUrl = l[0];
				foreach (var name in l)
				{
					Text += name + "\n";
				}
			}
			catch (Exception mes)
			{
				Text += "Ошибка получения дерева данных " + mes.Message + "\n";
			}
		}

		public RelayCommand BrowseAllCommand { get; private set; }
		void OnBrowseAll()
		{
			try
			{
				var l = client.BrowseAll(null, "");
				if (l.Count == 0)
				{
					Text += "NO nodes" + "\n";
					return;
				}
				//ServerUrl = l[0];
				foreach (var name in l)
				{
					Text += name + "\n";
				}
			}
			catch (Exception mes)
			{
				Text += "Ошибка получения дерева данных " + mes.Message + "\n";
			}
		}

		public RelayCommand AddSubscriptionCommand { get; private set; }
		void OnAddSubscription()
		{
			try
			{
				client.CreateSubscription(AddSubscription);
			}
			catch (Exception mes)
			{
				Text += "Error creating subscription " + mes.Message + "\n";
			}
		}

		public RelayCommand DeleteSubscriptionCommand { get; private set; }
		void OnDeleteSubscription()
		{
			try
			{
				client.DeleteSubscription(AddSubscription);
			}
			catch (Exception mes)
			{
				Text += "Error deleting subscription " + mes.Message + "\n";
			}
		}

		public RelayCommand GetSubscriptionListCommand { get; private set; }
		void OnGetSubscriptionList()
		{
			try
			{
				var l = client.GetSubscriptionList();
				if (l.Count == 0)
				{
					Text += "NO subscriptions" + "\n";
					return;
				}
				//ServerUrl = l[0];
				foreach (var name in l)
				{
					Text += name + "\n";
				}
			}
			catch (Exception mes)
			{
				Text += "ErrorGettingSubscriptionList " + mes.Message + "\n";
			}			
		}
	}
}