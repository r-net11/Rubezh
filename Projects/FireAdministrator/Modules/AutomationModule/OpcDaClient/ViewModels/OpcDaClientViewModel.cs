using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using RubezhClient;
using Infrastructure.Common.Windows;
using RubezhAPI.Automation;
using AutomationModule.Models;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientViewModel : MenuViewPartViewModel, IDisposable
	{
		#region Constructors
		
		public OpcDaClientViewModel()
		{
			Menu = new OpcDaClientMenuViewModel(this);

			AddOpcServerCommand = new RelayCommand(OnAddOpcServer, CanAddOpcServer);
			RemoveOpcServerCommand = new RelayCommand(OnRemoveOpcServer, CanRemoveOpcServer);
			EditTagListCommand = new RelayCommand(OnEditTagList, CanEditTagList);

			LoadConfig();
		}
		
		#endregion

		#region Fields And Properties

		ObservableCollection<OpcDaServer> _opcDaServers =
			new ObservableCollection<OpcDaServer>();
		public ObservableCollection<OpcDaServer> OpcDaServers
		{
			get { return _opcDaServers; }
			private set
			{
				_opcDaServers = value;
				OnPropertyChanged(() => OpcDaServers);
			}
		}

		OpcDaServer _selectedOpcServer;
		public OpcDaServer SelectedOpcServer
		{
			get { return _selectedOpcServer; }
			set 
			{ 
				_selectedOpcServer = value;
				OnPropertyChanged(() => SelectedOpcServer);
				SelectedTags = _selectedOpcServer.Tags;
			}
		}

		TsOpcTagsStructure _tags;

		public TsOpcTagsStructure Tags
		{
			get { return _tags; }
			set 
			{
				_tags = value;
				OnPropertyChanged(() => Tags);
			}
		}

		OpcDaTag[] _selectedTags;
		public OpcDaTag[] SelectedTags
		{
			get { return _selectedTags; }
			set 
			{ 
				_selectedTags = value;
				OnPropertyChanged(() => SelectedTags);
			}
		}

		#endregion

		#region Methods

		void LoadConfig()
		{
			OpcDaServers.Clear();

			foreach (var opcServer in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers)
			{
				OpcDaServers.Add(opcServer);
			}
		}
		void SaveConfig()
		{
			throw new NotImplementedException();
		}

		public void Initialize() { }

		public void Dispose() { }

		#endregion

		#region Commands
		
		public RelayCommand AddOpcServerCommand { get; private set; }
		void OnAddOpcServer() 
		{
			var addingServersDialog = new OpcDaClientAddingServersViewModel(this);
			DialogService.ShowModalWindow(addingServersDialog);
		}
		bool CanAddOpcServer() { return OpcDaServers != null; }

		public RelayCommand RemoveOpcServerCommand { get; private set; }
		void OnRemoveOpcServer() 
		{
			ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.Remove(SelectedOpcServer);
			OpcDaServers.Remove(SelectedOpcServer);
			ServiceFactory.SaveService.AutomationChanged = true;
			SelectedOpcServer = OpcDaServers.FirstOrDefault();
		}
		bool CanRemoveOpcServer() { return SelectedOpcServer != null; }

		public RelayCommand EditTagListCommand { get; private set; }
		void OnEditTagList() 
		{
			var editingTagList = new OpcDaClientEditingTagsViewModel(this);
			DialogService.ShowModalWindow(editingTagList);

			var server = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers
				.FirstOrDefault(x => x == SelectedOpcServer);
			server.Tags = SelectedTags;
			ServiceFactory.SaveService.AutomationChanged = true;

		}
		bool CanEditTagList() { return SelectedOpcServer != null; }

		#endregion
	}
}