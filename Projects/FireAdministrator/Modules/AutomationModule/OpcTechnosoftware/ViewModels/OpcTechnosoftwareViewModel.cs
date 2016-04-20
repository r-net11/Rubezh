using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common.Windows;
using OpcClientSdk;
using OpcClientSdk.Da;
using Infrastructure.Common.Windows.Windows;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class OpcTechnosoftwareViewModel : MenuViewPartViewModel, IDisposable
	{
		#region Constructors

		public OpcTechnosoftwareViewModel()
		{
			Menu = new OpcTechnosoftwareMenuViewModel(this);

			_dataChangeEventHandler =
				new TsCDaDataChangedEventHandler(EventHandler_activeSubscription_DataChangedEvent);
			_serverShutdownEventHandler =
				new OpcServerShutdownEventHandler(EventHandler_activeOpcServer_ServerShutdownEvent);

			GetHostNamesCommand = new RelayCommand(OnGetHostNames);
			GetOpcServerListCommand = new RelayCommand(OnGetServerList, CanGetServerList);
			ConnectCommand = new RelayCommand(OnConnect, CanConnect);
			DisconnectCommand = new RelayCommand(OnDisconnect, CanDisconnect);
			GetTagsAndGroupsCommand = new RelayCommand(OnGetTagsAndGroups, CanGetTagsAndGroups);
			GetCheckedTagsCommand = new RelayCommand(OnGetCheckedTags, CanGetCheckedTags);
			ReadTagsCommand = new RelayCommand(OnReadTags, CanReadTags);
			WriteTagsCommand = new RelayCommand(OnWriteTags, CanWriteTags);
			CreateSubscriptionCommand = new RelayCommand(OnCreateSubscription, CanCreateSubscription);
			CancelSubscriptionCommand = new RelayCommand(OnCancelSubscription, CanCancelSubscription);
			GetServerStatusCommand = new RelayCommand(OnGetServerStatus, CanGetServerStatus);
		}

		#endregion

		#region Fields And Properties

		public const string ROOT = @".";
		public const string SPLITTER = @"\";
		TsCDaServer _activeOpcServer;
		TsCDaSubscription _activeSubscription;
		TsCDaDataChangedEventHandler _dataChangeEventHandler;
		OpcServerShutdownEventHandler _serverShutdownEventHandler;

		OpcServer[] _servers;
		public OpcServer[] Servers
		{
			get { return _servers; }
			private set 
			{
				_servers = value;
				OnPropertyChanged(() => Servers);
			}
		}

		OpcServer _selectedOpcServer;
		public OpcServer SelectedOpcServer 
		{
			get { return _selectedOpcServer; }
			set
			{
				_selectedOpcServer = value;
				OnPropertyChanged(() => SelectedOpcServer);
			}
		}

		string[] _hostNames;
		public string[] HostNames 
		{
			get { return _hostNames; }
			private set 
			{
				_hostNames = value;
				OnPropertyChanged(() => HostNames);
			}
		}

		string _selectedHost;
		public string SelectedHost
		{
			get { return _selectedHost; }
			set
			{
				_selectedHost = value;
				OnPropertyChanged(() => SelectedHost);
			}
		}
		 
		TsOpcTagsStructure _tagsAndGroups;
		public TsOpcTagsStructure TagsAndGroups
		{
			get { return _tagsAndGroups; }
			private set
			{
				_tagsAndGroups = value;
				OnPropertyChanged(() => TagsAndGroups);
			}
		}

		IEnumerable<TsOpcElement> _checkedTags;
		public IEnumerable<TsOpcElement> CheckedTags 
		{
			get { return _checkedTags; }
			private set
			{
				_checkedTags = value;
				OnPropertyChanged(() => CheckedTags);
			}
		}

		TsOpcElement _selectedElement;
		public TsOpcElement SelectedElement
		{
			get { return _selectedElement; }
			set
			{
				_selectedElement = value;
				OnPropertyChanged(() => SelectedElement);
			}
		}

		bool _mode;
		public bool AsyncReadingEnbaled
		{
			get { return _mode; }
			set
			{
				_mode = value;
				OnPropertyChanged(() => AsyncReadingEnbaled);
			}
		}

		TsCDaItemValueResult[] _readingResult;
		public TsCDaItemValueResult[] ReadingResult
		{
			get { return _readingResult; }
			set 
			{
				_readingResult = value;
				OnPropertyChanged(() => ReadingResult);
			}
		}

		TsOpcElement _selectedTagForWrite;
		public TsOpcElement SelectedTagForWrite
		{
			get { return _selectedTagForWrite; }
			set 
			{
				_selectedTagForWrite = value;
				OnPropertyChanged(() => SelectedTagForWrite);
			}
		}

		OpcServerStatus _serverStatus;
		public OpcServerStatus ServerStatus 
		{
			get { return _serverStatus; }
			private set 
			{
				_serverStatus = value;
				OnPropertyChanged(() => ServerStatus);
			}
		}

		#endregion

		#region Methods

		public void Initialize()
		{
		}

		OpcServer[] GetRegistredServers(OpcSpecification specification, string url)
		{
			return OpcDiscovery.GetServers(specification, url).ToArray();
		}

		string[] GetHostNames()
		{
			return OpcDiscovery.GetHostNames().ToArray();
		}

		TsCDaBrowseElement[] Browse()
		{
			TsCDaBrowseFilters filters;
			List<TsCDaBrowseElement> elementList; 
			TsCDaBrowseElement[] elements;
			TsCDaBrowsePosition position;
			OpcItem path = new OpcItem();

			filters = new TsCDaBrowseFilters();
			filters.BrowseFilter = TsCDaBrowseFilter.All;
			filters.ReturnAllProperties = true;
			filters.ReturnPropertyValues = true;

			elementList = new List<TsCDaBrowseElement>();

			elements = _activeOpcServer.Browse(path, filters, out position);
		
			foreach(var item in elements)
			{
				//!!! OpcItem.ItemPath всегда равно null и видимо не участвует в коде. Буду заполнять руками
				item.ItemPath = ROOT + SPLITTER + item.ItemName;
				elementList.Add(item);
				
				if (!item.IsItem)
				{
					//if (item.HasChildren) На данный момент это не работает. Если элемент это группа, то всега true
					//{
						path = new OpcItem(item.ItemPath, item.Name);
						BrowseChildren(path, filters, elementList);
					//}
				}
				
			}
			return elementList.ToArray();
		}

		void BrowseChildren(OpcItem opcItem, TsCDaBrowseFilters filters, 
			IList<TsCDaBrowseElement> elementList)
		{
			TsCDaBrowsePosition position;
			OpcItem path;

			var elements = _activeOpcServer.Browse(opcItem, filters, out position);

			if (elements != null)
			{
				foreach (var item in elements)
				{
					item.ItemPath = opcItem.ItemPath + SPLITTER + item.ItemName;
					elementList.Add(item);

					if (!item.IsItem)
					{
						//if (item.HasChildren)
						//{
						path = new OpcItem(item.ItemPath, item.ItemName);
						BrowseChildren(path, filters, elementList);
						//}
					}
				}
			}
		}

		public void Dispose()
		{
			if (Servers != null)
			{
				foreach (var server in Servers)
				{
					if (server.IsConnected)
					{
						server.Disconnect();
					}
					server.Dispose();
				}
			}
		}

		#endregion

		#region Event Handlers

		void EventHandler_activeOpcServer_ServerShutdownEvent(string reason)
		{
			DisconnectCommand.Execute();
		}

		void EventHandler_activeSubscription_DataChangedEvent(object subscriptionHandle,
			object requestHandle, TsCDaItemValueResult[] values)
		{
			// Через параметр subscriptionHandle передаётся subscriptionState.ClientHandle
			// Обновляем данные
			ReadingResult = values;
		}

		#endregion

		#region Commands

		public RelayCommand GetHostNamesCommand { get; private set; }
		void OnGetHostNames()
		{
			WaitHelper.Execute(() => HostNames = GetHostNames());
		}

		public RelayCommand GetOpcServerListCommand { get; private set; }
		void OnGetServerList()
		{
			Servers = GetRegistredServers(OpcSpecification.OPC_DA_20, SelectedHost);
		}
		bool CanGetServerList()
		{
			return SelectedHost != null;
		}

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			_activeOpcServer = new TsCDaServer();
			var opcUrl = new OpcUrl(OpcSpecification.OPC_DA_20, OpcUrlScheme.DA, SelectedOpcServer.Url.ToString());
			_activeOpcServer.Connect(opcUrl, null); // во второй параметр передаются данные для 
													// авторизации пользователя на удалённом сервере
			_activeOpcServer.ServerShutdownEvent += EventHandler_activeOpcServer_ServerShutdownEvent;
		}

		bool CanConnect()
		{
			return _activeOpcServer == null && SelectedOpcServer != null;
		}

		public RelayCommand DisconnectCommand { get; private set; }
		void OnDisconnect()
		{
			try
			{
				if (CancelSubscriptionCommand.CanExecute(null))
				{
					CancelSubscriptionCommand.Execute();
				}
				_activeOpcServer.ServerShutdownEvent -= EventHandler_activeOpcServer_ServerShutdownEvent;
				_activeOpcServer.Disconnect();
				_activeOpcServer = null;
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show(ex.Message);
			}
		}
		bool CanDisconnect()
		{
			return _activeOpcServer != null && _activeOpcServer.IsConnected;
		}

		public RelayCommand GetTagsAndGroupsCommand { get; private set; }
		void OnGetTagsAndGroups()
		{
			var elements = Browse();
			TagsAndGroups = new TsOpcTagsStructure(elements);
		}
		bool CanGetTagsAndGroups()
		{
			return _activeOpcServer != null && _activeOpcServer.IsConnected;
		}

		public RelayCommand GetCheckedTagsCommand { get; private set; }
		void OnGetCheckedTags()
		{
			CheckedTags = TagsAndGroups.AllElements.Where(x => x.IsChecked).ToArray();
		}
		bool CanGetCheckedTags()
		{
			return TagsAndGroups != null;
		}

		public RelayCommand ReadTagsCommand { get; private set; }
		void OnReadTags()
		{
			var tags = CheckedTags.Select(x => new TsCDaItem(new OpcItem(x.Element.ItemName))).ToArray();
			var result = _activeOpcServer.Read(tags);
			ReadingResult = result;
		}
		bool CanReadTags()
		{
			return CheckedTags != null && CheckedTags.Count() > 0 && 
				_activeOpcServer != null && _activeOpcServer.IsConnected &&
				AsyncReadingEnbaled == false;
		}

		public RelayCommand WriteTagsCommand { get; private set; }
		void OnWriteTags()
		{
			var array = CheckedTags.Select(x => x.Element).ToArray();
			var vm = new OpcTechnosoftWriteTagsViewModel(_activeOpcServer, array);
			DialogService.ShowModalWindow(vm);
		}
		bool CanWriteTags()
		{
			return CheckedTags != null && CheckedTags.Count() > 0 &&
				_activeOpcServer != null && _activeOpcServer.IsConnected &&
				AsyncReadingEnbaled == false;
		}

		public RelayCommand CreateSubscriptionCommand { get; private set; }
		void OnCreateSubscription()
		{
			// Создаём объект подписки
			var subscriptionState = new TsCDaSubscriptionState
			{
				Name = "MySubscription",
				ClientHandle = "MySubscriptionId",
				Deadband = 0,
				UpdateRate = 1000,
				KeepAlive = 10000
			};
			_activeSubscription = (TsCDaSubscription)_activeOpcServer.CreateSubscription(subscriptionState);

			// Добавляем в объект подписки выбранные теги
			var x = 0;
			List<TsCDaItem> list = new List<TsCDaItem>();
			foreach (var item in CheckedTags)
			{
				var tag = new TsCDaItem
				{
					ItemName = item.Element.ItemName,
					ClientHandle = 100 + x // Уникальный Id определяемый пользователем
				};
				list.Add(tag);
				++x;
			}

			// Добавляем теги и проверяем результат данной операции
			var results = _activeSubscription.AddItems(list.ToArray());

			var errors = results.Where(result => result.Result.IsError());

			if (errors.Count() > 0)
			{
				StringBuilder msg = new StringBuilder();
				msg.Append("Не удалось добавить теги для подписки. Возникли ошибки в тегах:");
				foreach (var error in errors)
				{
 					msg.Append(String.Format("ItemName={0} ClientHandle={1} Description={2}; ",
						error.ItemName, error.ClientHandle, error.Result.Description()));
				}
				throw new InvalidOperationException(msg.ToString());
			}

			_activeSubscription.DataChangedEvent += _dataChangeEventHandler;
		}
		bool CanCreateSubscription()
		{
			return AsyncReadingEnbaled == true &&
				_activeOpcServer != null && _activeOpcServer.IsConnected
 				&& _activeSubscription == null
				&& CheckedTags != null && CheckedTags.Count() > 0;
		}

		public RelayCommand CancelSubscriptionCommand { get; private set; }
		void OnCancelSubscription()
		{
 			//_activeSubscription.Cancel()
			_activeOpcServer.CancelSubscription(_activeSubscription);
			_activeSubscription.DataChangedEvent -= _dataChangeEventHandler;
			_activeSubscription.Dispose();
			_activeSubscription = null;
		}
		bool CanCancelSubscription()
		{
			return AsyncReadingEnbaled == true && _activeSubscription != null;
		}

		public RelayCommand GetServerStatusCommand { get; private set; }
		void OnGetServerStatus()
		{
			ServerStatus = _activeOpcServer.GetServerStatus();
		}
		bool CanGetServerStatus()
		{
			return _activeOpcServer != null && _activeOpcServer.IsConnected;
		}

		#endregion
	}
}