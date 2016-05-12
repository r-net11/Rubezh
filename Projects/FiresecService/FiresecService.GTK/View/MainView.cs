using System;
using System.Collections.Generic;
using System.Linq;
using GKProcessor;
using Gtk;
using Infrastructure.Common.License;
using RubezhAPI;
using Application = Gtk.Application;
using DateTime = System.DateTime;
using RubezhAPI.License;
using RubezhAPI.Models;

namespace FiresecService.Views
{
	public class MainView
	{
		public Window MainWindow { get; private set; }
		static NodeView logNode;
		static NodeView gkNode;
		static NodeView connectionNode;
		static NodeView pollingNode;
		static NodeView operationNode;
		static StatusView statusView;

		public static MainView Current { get; private set; }
		readonly List<KeyValuePair<GKLifecycleInfo, DateTime>> GKLifecycles;
		readonly List<ClientCredentials> Clients;
		readonly List<ServerTask> ServerTasks; 

		public MainView()
		{
			MainWindow = new Window("Сервер приложений ГЛОБАЛ");
			statusView = new StatusView();

			MainWindow.DeleteEvent += Window_Delete;
			gkNode = new NodeView();
			gkNode.AppendColumn("Время", new CellRendererText(), "text", 0);
			gkNode.AppendColumn("Адрес", new CellRendererText(), "text", 1);
			gkNode.AppendColumn("Название", new CellRendererText(), "text", 2);
			gkNode.AppendColumn("Прогресс", new CellRendererText(), "text", 3);
			logNode = new NodeView();
			logNode.AppendColumn("Название", new CellRendererText(), "text", 0);
			logNode.AppendColumn("Время", new CellRendererText(), "text", 1);
			logNode.NodeStore = new NodeStore(typeof(LogTreeNode));
			connectionNode = new NodeView();
			connectionNode.AppendColumn("Тип", new CellRendererText(), "text", 0);
			connectionNode.AppendColumn("Адрес", new CellRendererText(), "text", 1);
			connectionNode.AppendColumn("Пользователь", new CellRendererText(), "text", 2);
			pollingNode = new NodeView();
			pollingNode.AppendColumn("Клиент", new CellRendererText(), "text", 0);
			pollingNode.AppendColumn("Идентификатор", new CellRendererText(), "text", 1);
			pollingNode.AppendColumn("Первый поллинг", new CellRendererText(), "text", 2);
			pollingNode.AppendColumn("Последний поллинг", new CellRendererText(), "text", 3);
			pollingNode.AppendColumn("Индекс", new CellRendererText(), "text", 4);
			pollingNode.NodeStore = new NodeStore(typeof(PollingTreeNode));
			operationNode = new NodeView();
			operationNode.AppendColumn("Название", new CellRendererText(), "text", 0);
			operationNode.NodeStore = new NodeStore(typeof(OperationTreeNode));
			Notebook notepadControl = new Notebook();
			notepadControl.AppendPage(connectionNode, new Label("Соединения"));
			notepadControl.AppendPage(logNode, new Label("Лог"));
			notepadControl.AppendPage(statusView.Create(), new Label("Статус"));
			notepadControl.AppendPage(gkNode, new Label("ГК"));
			notepadControl.AppendPage(pollingNode, new Label("Поллинг"));
			notepadControl.AppendPage(operationNode, new Label("Операции"));
			notepadControl.AppendPage(new LicenseView().Create(), new Label("Лицензирование"));
			MainWindow.Add(notepadControl);
			MainWindow.SetDefaultSize(500, 500);
			MainWindow.ShowAll();
			GKLifecycleManager.GKLifecycleChangedEvent += On_GKLifecycleChangedEvent;
			LicenseManager.LicenseChanged += On_LicenseChanged;

			GKLifecycles = new List<KeyValuePair<GKLifecycleInfo, DateTime>>();
			Clients = new List<ClientCredentials>();
			ClientPolls = new List<ClientPoll>();
			ServerTasks = new List<ServerTask>();
			Current = this;
		}

		void On_LicenseChanged()
		{
			MainWindow.Title = LicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.Demonstration ?
				"Сервер приложений Глобал [Демонстрационный режим]" : "Сервер приложений Глобал";
		}

		List<ClientPoll> ClientPolls;

		public void OnPoll(Guid uid)
		{
			Application.Invoke(delegate
			{
				var now = DateTime.Now;
				var clientInfo = Service.ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == uid);
				var client = clientInfo == null ? "" : string.Format("{0} / {1} / {2}", clientInfo.ClientCredentials.ClientType.ToDescription(), clientInfo.ClientCredentials.ClientIpAddress, clientInfo.ClientCredentials.FriendlyUserName);
				var clientPoll = ClientPolls.FirstOrDefault(x => x.Client == client && x.Uid == uid);
				if (clientPoll == null)
				{
					clientPoll = new ClientPoll {Uid = uid, Client = client};
                    clientPoll.FirstPollTime = DateTime.Now;
					ClientPolls.Add(clientPoll);
				}
				if (clientInfo != null)
					clientPoll.CallbackIndex = clientInfo.CallbackIndex; 
				clientPoll.LastPollTime = now;
				pollingNode.NodeStore = new NodeStore(typeof(PollingTreeNode));
				ClientPolls.ForEach(x => pollingNode.NodeStore.AddNode(new PollingTreeNode(x.Client, x.Uid.ToString(), x.FirstPollTime.ToString(), x.LastPollTime.ToString(), x.CallbackIndex.ToString())));
				pollingNode.ShowAll();
			});
		}

		void On_GKLifecycleChangedEvent(GKLifecycleInfo gkLifecycleInfo)
		{
			Application.Invoke(delegate
			{
				var gkLifecycle = GKLifecycles.FirstOrDefault(x => x.Key.UID == gkLifecycleInfo.UID);
				if (gkLifecycle.Equals(new KeyValuePair<GKLifecycleInfo, DateTime>()))
				{
					GKLifecycles.Insert(0, new KeyValuePair<GKLifecycleInfo, DateTime>(gkLifecycleInfo, DateTime.Now));
				}
				if (GKLifecycles.Count > 20)
					GKLifecycles.RemoveAt(20);
				gkNode.NodeStore = new NodeStore(typeof(GKTreeNode));
				GKLifecycles.ForEach(x => gkNode.NodeStore.AddNode(new GKTreeNode(x.Value.TimeOfDay.ToString(@"hh\:mm\:ss"), x.Key.Device.PresentationAddress, x.Key.Name, x.Key.Progress)));
				gkNode.ShowAll();
			});
		}

		public void AddLog(string message, bool isError)
		{
			var dateTime = DateTime.Now;
			Application.Invoke(delegate
			{
				logNode.NodeStore.AddNode(new LogTreeNode(message, dateTime.ToString()));
				logNode.ShowAll();
			});
		}

		static void Window_Delete(object o, DeleteEventArgs args)
		{
			Application.Quit();
			args.RetVal = true;
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			Application.Invoke(delegate
			{
				Clients.Add(clientCredentials);
				UpdateConnectionNode();
			});
		}
		public void RemoveClient(Guid uid)
		{
			Application.Invoke(delegate
			{
				var client = Clients.FirstOrDefault(x => x.ClientUID == uid);
				if (client != null)
				{
					Clients.Remove(client);
					UpdateConnectionNode();
				}
			});
		}

		void UpdateConnectionNode()
		{
			connectionNode.NodeStore = new NodeStore(typeof(ConnectionTreeNode));
			Clients.ForEach(x => connectionNode.NodeStore.AddNode(new ConnectionTreeNode(x.ClientType.ToDescription(),
				x.ClientIpAddress.StartsWith("127.0.0.1") ? "localhost" : x.ClientIpAddress, x.FriendlyUserName)));
			connectionNode.ShowAll();
		}

		void UpdateOperationNode()
		{
			operationNode.NodeStore = new NodeStore(typeof(OperationTreeNode));
			ServerTasks.ForEach(x => operationNode.NodeStore.AddNode(new OperationTreeNode(x.Name)));
			operationNode.ShowAll();
		}

		public void SetLocalAddress(string address)
		{
			Application.Invoke(delegate { statusView.LocalAddress = address; });
		}

		public void SetRemoteAddress(string address)
		{
			Application.Invoke(delegate { statusView.RemoteAddress = address; });
		}

		public void SetReportAddress(string address)
		{
			Application.Invoke(delegate { statusView.ReportAddress = address; });
		}

		public void AddTask(ServerTask serverTask)
		{
			Application.Invoke(delegate 
			{
				ServerTasks.Add(serverTask);
				UpdateOperationNode();
			});
		}
		public void RemoveTask(ServerTask serverTask)
		{
			Application.Invoke(delegate 
			{
				ServerTasks.Remove(serverTask);
				UpdateOperationNode();
			});
		}
	}

	public class GKTreeNode : TreeNode
	{
		public GKTreeNode(string time, string address, string name, string progress)
		{
			Time = time;
			Address = address;
			Name = name;
			Progress = progress;
		}

		[TreeNodeValue(Column = 0)]
		public string Time;

		[TreeNodeValue(Column = 1)]
		public string Address;

		[TreeNodeValue(Column = 2)]
		public string Name;

		[TreeNodeValue(Column = 3)]
		public string Progress;
	}

	public class LogTreeNode : TreeNode
	{
		public LogTreeNode(string name, string time)
		{
			Name = name;
			Time = time;
		}

		[TreeNodeValue(Column = 0)]
		public string Name;

		[TreeNodeValue(Column = 1)]
		public string Time;
	}

	public class ConnectionTreeNode : TreeNode
	{
		public ConnectionTreeNode(string type, string address, string user)
		{
			Type = type;
			Address = address;
			User = user;
		}

		[TreeNodeValue(Column = 0)]
		public string Type;

		[TreeNodeValue(Column = 1)]
		public string Address;

		[TreeNodeValue(Column = 2)]
		public string User;
	}

	public class PollingTreeNode : TreeNode
	{
		public PollingTreeNode(string client, string uid, string firstPollTime, string lastPollTime, string callbackIndex)
		{
			Client = client;
			Uid = uid;
			FirstPollTime = firstPollTime;
			LastPollTime = lastPollTime;
			CallbackIndex = callbackIndex;
		}

		[TreeNodeValue(Column = 0)]
		public string Client;

		[TreeNodeValue(Column = 1)]
		public string Uid;

		[TreeNodeValue(Column = 2)]
		public string FirstPollTime;

		[TreeNodeValue(Column = 3)]
		public string LastPollTime;

		[TreeNodeValue(Column = 4)]
		public string CallbackIndex;
	}

	public class OperationTreeNode : TreeNode
	{
		public OperationTreeNode(string name)
		{
			Name = name;
		}

		[TreeNodeValue(Column = 0)]
		public string Name;
	}

	public class ClientPoll
	{
		public string Client { get; set; }
		public Guid Uid { get; set; }
		public DateTime FirstPollTime { get; set; }
		public DateTime LastPollTime { get; set; }
		public int CallbackIndex { get; set; }
	}
}