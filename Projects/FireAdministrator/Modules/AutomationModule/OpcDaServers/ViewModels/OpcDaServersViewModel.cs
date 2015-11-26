using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using RubezhClient;

namespace AutomationModule.ViewModels
{
	public class OpcDaServersViewModel : MenuViewPartViewModel
	{
		#region Constructors

		public OpcDaServersViewModel()
		{
			Menu = new OpcDaServersMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			OpcDaServers = new ObservableCollection<OpcDaServerViewModel>();

			// Загрузка сохранённого списка серверов
			foreach (var opcServer in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaServers)
			{
				var opcServerViewModel = new OpcDaServerViewModel(opcServer);
				OpcDaServers.Add(opcServerViewModel);
			}
			
			// Сейчас заглушка
			//_opcDaServers = new ObservableCollection<OpcDaServerViewModel>(
			//	OpcDaServer.OpcDaServer.GetRegistredServers()
			//	.Select(serv => new OpcDaServerViewModel(serv)));

			// Для отладки
			//_opcDaServers.Add(new OpcDaServerViewModel(
			//	new OpcDaServer.OpcDaServer{ Id = Guid.NewGuid(), ServerName = "TestServer" }));
		}

		#endregion

		#region Fields And Properties

		private ObservableCollection<OpcDaServerViewModel> _opcDaServers;

		public ObservableCollection<OpcDaServerViewModel> OpcDaServers
		{
			get { return _opcDaServers; }
			set 
			{
				_opcDaServers = value;
				OnPropertyChanged(() => OpcDaServers);
			}
		}

		private OpcDaServerViewModel _selectedOpcDaServer;

		public OpcDaServerViewModel SelectedOpcDaServer
		{
			get { return _selectedOpcDaServer; }
			set
			{
				_selectedOpcDaServer = value;
				OnPropertyChanged(() => SelectedOpcDaServer);
			}
		}

		#endregion

		#region Methods

		public void Initialize()
		{ 
		}

		#endregion

		#region Commands

		public RelayCommand AddCommand { get; private set; }

		private void OnAdd()
		{
			//Cоздаём и передаём список отсутствующих в конфигурационном списке серверов
			var list = OpcDaServer.OpcDaServer.GetRegistredServers();
			var notselectedServers = OpcDaServers.Count == 0 ?
									list.Select(x => new OpcDaServerViewModel(
										new RubezhAPI.Automation.OpcDaServer { Id = x.Id, ServerName= x.ServerName })) :
									from rs in list
									from ss in OpcDaServers
									where rs.Id != ss.Id
									select new OpcDaServerViewModel(
										new RubezhAPI.Automation.OpcDaServer { Id = rs.Id, ServerName = rs.ServerName });

			var addingDialog = new OpcDaServersAddingServersViewModel(notselectedServers);

			if (Infrastructure.Common.Windows.DialogService.ShowModalWindow(addingDialog))
			{
				foreach (var server in addingDialog.SelectedServers)
				{
					OpcDaServers.Add(server);
					ClientManager.SystemConfiguration.AutomationConfiguration
						.OpcDaServers.Add(server.ConvertTo());
					ServiceFactory.SaveService.AutomationChanged = true;
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			ClientManager.SystemConfiguration.AutomationConfiguration
				.OpcDaServers.Remove(SelectedOpcDaServer.Base);
			ServiceFactory.SaveService.AutomationChanged = true;

			OpcDaServers.Remove(SelectedOpcDaServer);
			SelectedOpcDaServer = OpcDaServers.FirstOrDefault();
		}

		private bool CanDelete()
		{
			return SelectedOpcDaServer != null;
		}

		#endregion
	}
}
