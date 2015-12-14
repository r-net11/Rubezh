using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using RubezhClient;
using Infrastructure.Common.Windows;

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

		ObservableCollection<RubezhAPI.Automation.TsOpcServer> _opcDaServers =
			new ObservableCollection<RubezhAPI.Automation.TsOpcServer>();
		public ObservableCollection<RubezhAPI.Automation.TsOpcServer> OpcDaServers
		{
			get { return _opcDaServers; }
			private set
			{
				_opcDaServers = value;
				OnPropertyChanged(() => OpcDaServers);
			}
		}

		RubezhAPI.Automation.OpcDaServer _selectedOpcServer;
		public RubezhAPI.Automation.OpcDaServer SelectedOpcServer
		{
			get { return _selectedOpcServer; }
			set 
			{ 
				_selectedOpcServer = value;
				OnPropertyChanged(() => SelectedOpcServer);
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
		void OnRemoveOpcServer() { }
		bool CanRemoveOpcServer() { return false; }

		public RelayCommand EditTagListCommand { get; private set; }
		void OnEditTagList() { }
		bool CanEditTagList() { return false; }

		#endregion
	}
}