using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using OpcClientSdk;
using OpcClientSdk.Da;

namespace AutomationModule.ViewModels
{
	public class OpcTechnosoftwareViewModel : MenuViewPartViewModel
	{
		#region Constructors

		public OpcTechnosoftwareViewModel()
		{
			Menu = new OpcTechnosoftwareMenuViewModel(this);

			GetHostNamesCommand = new RelayCommand(OnGetHostNames);
			GetOpcServerListCommand = new RelayCommand(OnGetServerList, CanGetServerList);
			ConnectCommand = new RelayCommand(OnConnect, CanConnect);
			DisconnectCommand = new RelayCommand(OnDisconnect, CanDisconnect);
			GetTagsAndGroupsCommand = new RelayCommand(OnGetTagsAndGroups, CanGetTagsAndGroups);
		}

		#endregion

		#region Fields And Properties
		public const string ROOT = @".";
		public const string SPLITTER = @"\";
		TsCDaServer _activeOpcServer;

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

		ObservableCollection<TsCDaBrowseElement> _tagsAndGroups = new ObservableCollection<TsCDaBrowseElement>();
		public ObservableCollection<TsCDaBrowseElement> TagsAndGroups
		{
			get { return _tagsAndGroups; }
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

		void BrowseChildren(OpcItem opcItem, TsCDaBrowseFilters filters, IList<TsCDaBrowseElement> elementList)
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
							path = new OpcItem(item.ItemPath, item.Name);
							BrowseChildren(path, filters, elementList);
						//}
					}
				}
			}
		}

		#endregion

		#region Commands

		public RelayCommand GetHostNamesCommand { get; private set; }
		void OnGetHostNames()
		{
			HostNames = GetHostNames();
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
			_activeOpcServer.Connect(SelectedOpcServer.Url.ToString());
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
 			TagsAndGroups.Clear();
			foreach (var item in elements)
			{
				TagsAndGroups.Add(item);
			}
		}
		bool CanGetTagsAndGroups()
		{
			return _activeOpcServer != null && _activeOpcServer.IsConnected;
		}

		#endregion

	}
}