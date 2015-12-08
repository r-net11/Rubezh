using System;
using System.Collections.Generic;
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

			ConnectCommand = new RelayCommand(OnConnect, CanConnect);
			DisconnectCommand = new RelayCommand(OnDisconnect, CanDisconnect);
		}

		#endregion

		#region Fields And Properties

		TsCDaServer _activeOpcServer;

		public string SelectedOpcServer { get; private set; }

		#endregion

		#region Methods

		public void Initialize()
		{
		}

		#endregion

		#region Commands

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			_activeOpcServer = new TsCDaServer();
			_activeOpcServer.Connect(SelectedOpcServer);
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

		#endregion

	}
}