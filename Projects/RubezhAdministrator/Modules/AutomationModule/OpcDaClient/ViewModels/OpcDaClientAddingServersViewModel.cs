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

			WaitHelper.Execute(OnGetOpcServerList);
		}

		#endregion

		#region Fields And Properties

		OpcDaClientViewModel _opcDaServersViewModel;

		public string Login { get; set; }
		public string Password { get; set; }

		public OpcDaServer[] _opcServers;
		public OpcDaServer[] OpcDaServers 
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

		private bool _remoteConnectionIsEnabled;
		public bool RemoteConnectionIsEnabled
		{
			get { return _remoteConnectionIsEnabled; }
			set 
			{
				_remoteConnectionIsEnabled = value;
				OnPropertyChanged(() => RemoteConnectionIsEnabled);
			}
		} 

		#endregion

		#region Methods

		protected override bool Save()
		{
			foreach (var item in OpcDaServers)
			{
				if (item.IsChecked)
				{
					item.Uid = Guid.NewGuid();
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
			List<OpcDaServer> serverList = new List<OpcDaServer>();

			var login = Login == null ? String.Empty : Login.Trim();
			var pswd = Password == null ? String.Empty : Password.Trim();

			try
			{
				var result = ClientManager.RubezhService.GetOpcDaServers(RubezhServiceFactory.UID);
				var servers = result.HasError ? new OpcDaServer[0] : result.Result;

				foreach (var server in servers)
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
			catch (Exception ex)
			{
				string msg;
				msg = ex.InnerException != null ?
					String.Format("Исключение: {0} ; Внутреннее исключение: {1}", ex.Message, ex.InnerException.Message) :
					ex.Message;
					
				System.Windows.MessageBox.Show(msg, "Ошибка", 
					System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
			}
		}
		bool CanGetOpcServerList() { return true; }

		#endregion
	}
}