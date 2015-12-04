using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using Infrastructure.Common;
using OPCDA.NET;
using OPC;
//using OPCDA.Interface;

namespace AutomationModule.ViewModels
{
	public class OpcAdvosolViewModel : MenuViewPartViewModel
	{
		#region Constructors

		public OpcAdvosolViewModel()
		{
			Menu = new OpcAdvosolMenuViewModel(this);

			ConnectCommand = new RelayCommand(OnConnect, CanConnect);
			DisconnectCommand = new RelayCommand(OnDisconnect, CanDisconnect);
		}

		#endregion

		#region Fields And Properties

		OpcServer _activeOpcServer;
		OpcDataBind _activeOpcServerBinding;

		string[] _opcServers;
		public string[] OpcServers
		{
			get { return _opcServers; }
			set 
			{
				_opcServers = value;
				OnPropertyChanged(() => OpcServers);
			}
		}

		string _selectedOpcServer;
		public string SelectedOpcServer
		{
			get { return _selectedOpcServer; }
			set 
			{
				_selectedOpcServer = value;
				OnPropertyChanged(() => SelectedOpcServer);
			}
		}

		/// <summary>
		/// Возвращает список компьютеров в локальной сети
		/// </summary>
		public string[] Computers
		{
			get { return OPC.Common.ComApi.EnumComputers(); }
		}

		string _selectedComputer;
		public string SelectedComputer
		{
			get { return _selectedComputer; }
			set 
			{
				_selectedComputer = value;
				OpcServers = GetOpcServerList(_selectedComputer);
				OnPropertyChanged(() => SelectedComputer);
			}
		}

		#endregion

		#region Methods
		
		public void Initialize()
		{
		}

		/// <summary>
		/// Возвращает именя зарегестированных сереров
		/// </summary>
		/// <param name="computerName">
		/// Имя удалённой машины для получения списка
		/// Если не указано, то запрашивается список на локальной машине.
		/// </param>
		/// <returns></returns>
		public string[] GetOpcServerList(string computerName = null)
		{
			string[] serverNames;
			var opcServerBrowser = computerName == null ?
				new OpcServerBrowser() :
				new OpcServerBrowser(computerName);
			opcServerBrowser.GetServerList(true, true, out serverNames);
			return serverNames;
		}

		#endregion

		#region Commands

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			try
			{
				_activeOpcServer = new OpcServer();
				_activeOpcServer.Connect(SelectedComputer, SelectedOpcServer);
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show(ex.Message);
			}
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
			return _activeOpcServer != null && _activeOpcServer.isConnectedDA;
		}

		#endregion
	}
}
