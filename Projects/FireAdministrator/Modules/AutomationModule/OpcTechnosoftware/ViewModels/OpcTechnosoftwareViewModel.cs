using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using OpcClientSdk;
using OpcClientSdk.Da;
using AutomationModule.Models;
using Infrastructure.Common.Windows;

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
			GetCheckedTagsCommand = new RelayCommand(OnGetCheckedTags, CanGetCheckedTags);
			ReadTagsCommand = new RelayCommand(OnReadTags, CanReadTags);
			WriteTagsCommand = new RelayCommand(OnWriteTags, CanWriteTags);
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

		OpcTechnosoftwareServerStructure _tagsAndGroups;
		public OpcTechnosoftwareServerStructure TagsAndGroups
		{
			get { return _tagsAndGroups; }
			private set
			{
				_tagsAndGroups = value;
				OnPropertyChanged(() => TagsAndGroups);
			}
		}

		IEnumerable<OpcTechnosoftwareElement> _checkedTags;
		public IEnumerable<OpcTechnosoftwareElement> CheckedTags 
		{
			get { return _checkedTags; }
			private set
			{
				_checkedTags = value;
				OnPropertyChanged(() => CheckedTags);
			}
		}


		OpcTechnosoftwareElement _selectedElement;
		public OpcTechnosoftwareElement SelectedElement
		{
			get { return _selectedElement; }
			set
			{ 
				_selectedElement = value;
				OnPropertyChanged(() => SelectedElement);
			}
		}

		bool _mode;
		public bool Mode
		{
			get { return _mode; }
			set
			{
				_mode = value;
				OnPropertyChanged(() => Mode);
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

		OpcTechnosoftwareElement _selectedTagForWrite;
		public OpcTechnosoftwareElement SelectedTagForWrite
		{
			get { return _selectedTagForWrite; }
			set 
			{
				_selectedTagForWrite = value;
				OnPropertyChanged(() => SelectedTagForWrite);
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
			TagsAndGroups = new OpcTechnosoftwareServerStructure(elements);
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
				Mode == false;
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
				Mode == false;
		}

		#endregion
	}
}