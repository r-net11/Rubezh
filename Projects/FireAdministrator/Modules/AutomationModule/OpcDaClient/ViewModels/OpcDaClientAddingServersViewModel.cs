using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcClientSdk;
using OpcClientSdk.Da;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientAddingServersViewModel : SaveCancelDialogViewModel
	{
		#region Constructors

		public OpcDaClientAddingServersViewModel(OpcDaClientViewModel vm)
		{
			Title = "Добавить OPC DA сервер";

			_opcDaServersViewModel = vm;
			GetOpcServerListCommand = new RelayCommand(OnGetOpcServerList, CanGetOpcServerList);

			WaitHelper.Execute(GetHostNames);
		}

		#endregion

		#region Fields And Properties

		OpcDaClientViewModel _opcDaServersViewModel;

		public string[] HostNames { get; private set; }

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

		public string Login { get; set; }
		public string Password { get; set; }

		public TsOpcServer[] _opcServers;
		public TsOpcServer[] OpcDaServers 
		{
			get { return _opcServers; }
			set
			{
				_opcServers = value;
				OnPropertyChanged(() => OpcDaServers);
				OnPropertyChanged(() => OpcServersTableIsVisible);
			}
		}

		public bool OpcServersTableIsVisible 
		{
			get { return OpcDaServers != null; }
		}

		#endregion

		#region Methods

		void GetHostNames()
		{
			HostNames = OpcDiscovery.GetHostNames().ToArray(); 
		}

		protected override bool Save()
		{
			foreach (var item in OpcDaServers)
			{
				if (item.IsChecked)
				{
					_opcDaServersViewModel.OpcDaServers.Add(item);
					ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.Add(item);
				}
			}
			ServiceFactory.SaveService.AutomationChanged = true;
			return base.Save();
		}

		#endregion

		#region Commands

		public RelayCommand GetOpcServerListCommand { get; private set; }
		void OnGetOpcServerList()
		{
			List<TsOpcServer> serverList = new List<TsOpcServer>();

			var login = Login == null ? String.Empty : Login.Trim();
			var pswd = Password == null ? String.Empty : Password.Trim();

			var servers = OpcDiscovery.GetServers(OpcSpecification.OPC_DA_20, SelectedHost,
				new OpcUserIdentity(login, pswd))
				.Select(srv => new TsOpcServer
					{ 
						IsChecked = false,
						Login = login,
						Password = pswd,
						ServerName = srv.ServerName,
						Url = srv.Url.ToString()
					});

			foreach(var server in servers)
			{
				if (!_opcDaServersViewModel.OpcDaServers
					.Any(x => x.Url == server.Url && 
						x.ServerName == server.ServerName))
				{
					serverList.Add(server);
				}
			}
			OpcDaServers = serverList.ToArray();
		}
		bool CanGetOpcServerList()
		{
			return !string.IsNullOrEmpty(SelectedHost);
		}

		#endregion
	}
}