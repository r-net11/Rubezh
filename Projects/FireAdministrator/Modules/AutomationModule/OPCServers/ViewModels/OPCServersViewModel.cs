using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using RubezhAPI.Automation;
using RubezhAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using Microsoft.Win32;
using Softing.Opc.Ua.Toolkit;
using Softing.Opc.Ua.Toolkit.Client;
using System.Diagnostics;
using RubezhClient;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace AutomationModule.ViewModels
{
	public class OPCServersViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public OPCUAClient client;

		public OPCServersViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			MonitoringCommand = new RelayCommand(OnMonitoring, CanEditDelete);
			CheckTagsCommand = new RelayCommand(OnCheckTags, CanEditDelete);
			PrintTreeCommand = new RelayCommand(OnPrintTreeCommand, CanEditDelete);
			Menu = new OPCServersMenuViewModel(this);
			RegisterShortcuts();

			client = new OPCUAClient();
			ConnectionString = "opc.tcp://localhost:51510/UA/DemoServer";
			ServerUrl = "ServerUrl";
			SubscriptionName = "SubscriptionName";

			ConfigureCommand = new RelayCommand(OnConfigure);
			GetEndpontsCommand = new RelayCommand(OnGetEndponts);
			CreateOpcTcpSessionWithNoSecurityCommand = new RelayCommand(OnCreateOpcTcpSessionWithNoSecurity);
			DisconnectSessionCommand = new RelayCommand(OnDisconnectSession);
			BrowseMainCommand = new RelayCommand(OnBrowseMain);
			BrowseAllCommand = new RelayCommand(OnBrowseAll);
			AddSubscriptionCommand = new RelayCommand(OnAddSubscription);
			DeleteSubscriptionCommand = new RelayCommand(OnDeleteSubscription);
			GetSubscriptionListCommand = new RelayCommand(OnGetSubscriptionList);
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}

		public void Initialize()
		{
			OPCServers = new ObservableCollection<OPCServerViewModel>();
			OPCTags = new ObservableCollection<OPCTagViewModel>();
			foreach (var opcServer in ClientManager.SystemConfiguration.AutomationConfiguration.OPCServers)
			{
				var opcServerViewModel = new OPCServerViewModel(opcServer);
				OPCServers.Add(opcServerViewModel);
			}
			SelectedOPCServer = OPCServers.FirstOrDefault();

			OnConfigure();
			client.Sessions = new List<Session>();
		}

		private ObservableCollection<OPCServerViewModel> _opcServers;

		public ObservableCollection<OPCServerViewModel> OPCServers
		{
			get { return _opcServers; }
			set
			{
				_opcServers = value;
				OnPropertyChanged(() => OPCServers);
			}
		}

		private ObservableCollection<OPCTagViewModel> _opcTags;

		public ObservableCollection<OPCTagViewModel> OPCTags
		{
			get { return _opcTags; }
			set
			{
				_opcTags = value;
				OnPropertyChanged(() => OPCTags);
			}
		}

		private OPCServerViewModel _selectedOPCServer;

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

		private void OnAdd()
		{
			var opcServerDetailsViewModel = new OPCServerDetailsViewModel();
			if (DialogService.ShowModalWindow(opcServerDetailsViewModel))
			{
				var opcServerViewModel = new OPCServerViewModel(opcServerDetailsViewModel.OPCServer);
				ClientManager.SystemConfiguration.AutomationConfiguration.OPCServers.Add(opcServerDetailsViewModel.OPCServer);
				OPCServers.Add(opcServerViewModel);
				SelectedOPCServer = OPCServers.FirstOrDefault();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand PrintTreeCommand { get; private set; }

		private void OnPrintTreeCommand()
		{
			var opcServerDetailsViewModel = new OPCServerDetailsViewModel();
			if (DialogService.ShowModalWindow(opcServerDetailsViewModel))
			{
				var opcServerViewModel = new OPCServerViewModel(opcServerDetailsViewModel.OPCServer);
				ClientManager.SystemConfiguration.AutomationConfiguration.OPCServers.Add(opcServerDetailsViewModel.OPCServer);
				OPCServers.Add(opcServerViewModel);
				SelectedOPCServer = OPCServers.FirstOrDefault();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }

		private void OnDelete()
		{
			var index = OPCServers.IndexOf(SelectedOPCServer);
			ClientManager.SystemConfiguration.AutomationConfiguration.OPCServers.Remove(SelectedOPCServer.OPCServer);
			OPCServers.Remove(SelectedOPCServer);
			index = Math.Min(index, OPCServers.Count - 1);
			if (index > -1)
				SelectedOPCServer = OPCServers[index];
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }

		private void OnEdit()
		{
			var opcServerDetailsViewModel = new OPCServerDetailsViewModel(SelectedOPCServer.OPCServer);
			if (DialogService.ShowModalWindow(opcServerDetailsViewModel))
			{
				SelectedOPCServer.OPCServer = opcServerDetailsViewModel.OPCServer;
				SelectedOPCServer.Update();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand CheckTagsCommand { get; private set; }

		private void OnCheckTags()
		{
			var cs = client.DefaultConnectionString(SelectedOPCServer.Address);
			var session = client.Sessions.FirstOrDefault(x => x.Url == cs) ?? client.AddOpcTcpSessionWithNoSecurity(cs);
			var subscription = session.Subscriptions.FirstOrDefault() ??
							   client.AddSubscription(session, "DefaultSubscription");
			var opcSelectTagsViewModel = new OPCSelectTagsViewModel(session, subscription);
			if (DialogService.ShowModalWindow(opcSelectTagsViewModel))
			{
				var l = opcSelectTagsViewModel.AllTags.Where(x => x.IsTagUsed).ToList();
				foreach (var tag in l)
				{
					AddMonitoredItem(session, tag.Path, tag.Tag.DisplayName.ToString());
				}

				var tl = OPCTags.Where(x => x.OPCTag.SessionUrl == session.Url).ToList();
				foreach (var tag in tl.Where(tag => !l.Any(x => x.Path == tag.OPCTag.NodeNum)))
				{
					DeleteMonitoredItem(session, tag.OPCTag.NodeNum);
				}

				/*foreach (var tag in tl)
				{
					var ww = l.Where(x => x.Address == tag.OPCTag.NodeNum);
                    
					if (ww.Count() ==0 )
					DeleteMonitoredItem(session, tag.OPCTag.NodeNum);
				}*/
			}
		}

		public RelayCommand MonitoringCommand { get; private set; }

		private void OnMonitoring()
		{
			var cs = client.DefaultConnectionString(SelectedOPCServer.Address);
			var session = client.Sessions.FirstOrDefault(x => x.Url == cs) ?? client.AddOpcTcpSessionWithNoSecurity(cs);
			var subscription = session.Subscriptions.FirstOrDefault() ??
							   client.AddSubscription(session, "DefaultSubscription");

			if (subscription.MonitoredItems.Count > 0)
			{
				var l = subscription.MonitoredItems.ToList();
				foreach (var item in l)
				{
					var tag = OPCTags.FirstOrDefault(x => x.Uid == item.DisplayName);
					if (tag != null)
					{
						OPCTags.Remove(tag);
					}
					item.NotificationReceived -= Monitoreditem_NotificationReceived;
					item.Delete();
				}
			}
			else
			{
				NodeId node = new NodeId("ns=3;i=10846");
				var mi = new MonitoredItem(subscription, node, AttributeId.Value, null,
					Guid.NewGuid().ToString());
				var tag = new OPCTag();
				tag.Name = "Какая-то нода";
				tag.NodeNum = mi.NodeId.ToString();
				tag.Uid = mi.DisplayName;
				tag.SessionUrl = session.Url;
				OPCTags.Add(new OPCTagViewModel(tag));
				mi.NotificationReceived += Monitoreditem_NotificationReceived;
			}

		}

		private void DeleteMonitoredItem(Session session, string nodeId)
		{
			if (!OPCTags.Any(x => (x.OPCTag.SessionUrl == session.Url) && (x.OPCTag.NodeNum == nodeId))) return;
			var subscription = session.Subscriptions.FirstOrDefault() ??
							   client.AddSubscription(session, "DefaultSubscription");

			var tItem = OPCTags.FirstOrDefault(x => x.OPCTag.NodeNum == nodeId);
			if (tItem != null)
			{
				OPCTags.Remove(tItem);
			}

			var item = subscription.MonitoredItems.FirstOrDefault(x => x.NodeId.ToString() == nodeId);
			if (item != null)
			{
				item.NotificationReceived -= Monitoreditem_NotificationReceived;
				item.Delete();
			}
		}

		private void AddMonitoredItem(Session session, string nodeId, string name)
		{
			if (OPCTags.Any(x => (x.OPCTag.SessionUrl == session.Url) && (x.OPCTag.NodeNum == nodeId))) return;

			var subscription = session.Subscriptions.FirstOrDefault() ??
							   client.AddSubscription(session, "DefaultSubscription");
			NodeId node = new NodeId(nodeId);
			var mi = new MonitoredItem(subscription, node, AttributeId.Value, null,
				name);
			var tag = new OPCTag();
			tag.Name = name;
			tag.NodeNum = nodeId;
			tag.Uid = "";
			tag.SessionUrl = session.Url;
			OPCTags.Add(new OPCTagViewModel(tag));
			mi.NotificationReceived += Monitoreditem_NotificationReceived;
		}

		private void Monitoreditem_NotificationReceived(object sender,
			Softing.Opc.Ua.Toolkit.Client.MonitoredItemNotificationEventArgs e)
		{
			Text += (sender as MonitoredItem).DisplayName + " " + (sender as MonitoredItem).NodeId.ToString() + " " +
					(e.NotificationValue as MonitoredItemNotification).Value.Value + " " + "\n";
			var tags = OPCTags.Where(x => (x.OPCTag.NodeNum == (sender as MonitoredItem).NodeId.ToString()) && (x.OPCTag.SessionUrl == (sender as MonitoredItem).Subscription.Session.Url)).ToList();
			if (tags.Count > 1)
			{
				throw new Exception("Не могу идентифицировать тег " + (sender as MonitoredItem).DisplayName);
			}

			if (tags.Count == 1)
			{
				tags[0].Value = (e.NotificationValue as MonitoredItemNotification).Value.Value.ToString();
			}
		}

		private bool CanEditDelete()
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

		private string _text;

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		private string _connectionString;

		public string ConnectionString
		{
			get { return _connectionString; }
			set
			{
				_connectionString = value;
				OnPropertyChanged(() => ConnectionString);
			}
		}

		private string _serverUrl;

		public string ServerUrl
		{
			get { return _serverUrl; }
			set
			{
				_serverUrl = value;
				OnPropertyChanged(() => ServerUrl);
			}
		}

		private string _subscriptionName;

		public string SubscriptionName
		{
			get { return _subscriptionName; }
			set
			{
				_subscriptionName = value;
				OnPropertyChanged(() => SubscriptionName);
			}
		}

		public RelayCommand ConfigureCommand { get; private set; }

		[DebuggerStepThrough]
		private void OnConfigure()
		{
			try
			{
				var configClass = new ConfigClass();
				var result = configClass.LoadApplicationConfiguration();
				if (result)
					Text += "Configuratrion loaded\n";
			}
			catch { }
		}

		public RelayCommand GetEndpontsCommand { get; private set; }

		private void OnGetEndponts()
		{
			try
			{
				var endpoints = client.GetEndpoints(ConnectionString);
				if (endpoints.Count == 0)
				{
					Text += "NO endpoints" + "\n";
					return;
				}
				ServerUrl = endpoints[0];
				foreach (var name in endpoints)
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

		private void OnCreateOpcTcpSessionWithNoSecurity()
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

		private void OnDisconnectSession()
		{
			if (client.m_session == null)
			{
				Text += "No created sessions\n";
			}
			else
			{
				var sessionURL = client.m_session.Url;
				client.DisconnectSession();
				Text += "Session disconnected " + sessionURL + "\n";
			}
		}

		public RelayCommand BrowseMainCommand { get; private set; }

		private void OnBrowseMain()
		{
			try
			{
				var nodes = client.BrowseMain();
				if (nodes.Count == 0)
				{
					Text += "NO nodes" + "\n";
					return;
				}
				//ServerUrl = l[0];
				foreach (var name in nodes)
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

		private void OnBrowseAll()
		{
			try
			{
				var nodes = client.BrowseAll(null, "");
				if (nodes.Count == 0)
				{
					Text += "NO nodes" + "\n";
					return;
				}
				//ServerUrl = l[0];
				foreach (var name in nodes)
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

		private void OnAddSubscription()
		{
			try
			{
				client.CreateSubscription(SubscriptionName);
			}
			catch (Exception mes)
			{
				Text += "Error creating subscription " + mes.Message + "\n";
			}
		}

		public RelayCommand DeleteSubscriptionCommand { get; private set; }

		private void OnDeleteSubscription()
		{
			try
			{
				client.DeleteSubscription(SubscriptionName);
			}
			catch (Exception mes)
			{
				Text += "Error deleting subscription " + mes.Message + "\n";
			}
		}

		public RelayCommand GetSubscriptionListCommand { get; private set; }

		private void OnGetSubscriptionList()
		{
			try
			{
				var subscriptionNames = client.GetSubscriptionList();
				if (subscriptionNames.Count == 0)
				{
					Text += "NO subscriptions" + "\n";
					return;
				}
				//ServerUrl = l[0];
				foreach (var name in subscriptionNames)
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