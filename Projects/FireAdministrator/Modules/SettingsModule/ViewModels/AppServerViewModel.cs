using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class AppServerViewModel : BaseViewModel
	{
		/// <summary>
		/// Список доступных ip-адресов для хоста, где размещен сервер приложений
		/// </summary>
		public ObservableCollection<string> AvailableIpAddresses { get; private set; }

		private void InitializeAvailableIpAddresses()
		{
			var hostIpAdresses = FiresecManager.FiresecService.GetHostAvailableIpAdresses().Result;
			hostIpAdresses.Add("localhost");
			AvailableIpAddresses = new ObservableCollection<string>(hostIpAdresses);
		}

		private string _serviceAddress;
		/// <summary>
		/// Ip-адрес сервера приложений
		/// </summary>
		public string ServiceAddress
		{
			get { return _serviceAddress; }
			set
			{
				if (_serviceAddress == value)
					return;
				_serviceAddress = value;
				OnPropertyChanged(() => ServiceAddress);
			}
		}

		private int _servicePort;
		/// <summary>
		/// Порт
		/// </summary>
		public int ServicePort
		{
			get { return _servicePort; }
			set
			{
				if (_servicePort == value)
					return;
				_servicePort = value;
				OnPropertyChanged(() => ServicePort);
			}
		}

		private int _reportServicePort;
		/// <summary>
		/// Порт сервиса генерации отчетов
		/// </summary>
		public int ReportServicePort
		{
			get { return _reportServicePort; }
			set
			{
				if (_reportServicePort == value)
					return;
				_reportServicePort = value;
				OnPropertyChanged(() => ReportServicePort);
			}
		}

		private bool _enableRemoteConnections;
		public bool EnableRemoteConnections
		{
			get { return _enableRemoteConnections; }
			set
			{
				if (_enableRemoteConnections == value)
					return;
				_enableRemoteConnections = value;
				OnPropertyChanged(() => EnableRemoteConnections);

				InvalidateServiceAddress();
			}
		}

		private void InvalidateServiceAddress()
		{
			if (!EnableRemoteConnections)
				ServiceAddress = "localhost";
		}

		private string _dbServerName;
		public string DBServerName
		{
			get { return _dbServerName; }
			set
			{
				if (_dbServerName == value)
					return;
				_dbServerName = value;
				OnPropertyChanged(() => DBServerName);
			}
		}

		private bool _createNewDBOnOversize;
		public bool CreateNewDBOnOversize
		{
			get { return _createNewDBOnOversize; }
			set
			{
				if (_createNewDBOnOversize == value)
					return;
				_createNewDBOnOversize = value;
				OnPropertyChanged(() => CreateNewDBOnOversize);
			}
		}

		private string _dbServerAddress;
		public string DBServerAddress
		{
			get { return _dbServerAddress; }
			set
			{
				if (_dbServerAddress == value)
					return;
				_dbServerAddress = value;
				OnPropertyChanged(() => DBServerAddress);
			}
		}

		private int _dbServerPort;
		public int DBServerPort
		{
			get { return _dbServerPort; }
			set
			{
				if (_dbServerPort == value)
					return;
				_dbServerPort = value;
				OnPropertyChanged(() => DBServerPort);
			}
		}

		private SqlServerAuthenticationMode _sqlServerAuthenticationMode;
		public SqlServerAuthenticationMode SqlServerAuthenticationMode
		{
			get { return _sqlServerAuthenticationMode; }
			set
			{
				if (_sqlServerAuthenticationMode == value)
					return;
				_sqlServerAuthenticationMode = value;
				OnPropertyChanged(() => SqlServerAuthenticationMode);
				OnPropertyChanged(() => UseSqlServerAuthentication);
			}
		}

		public ObservableCollection<SqlServerAuthenticationMode> AvailableSqlServerAuthenticationModes { get; private set; }

		private void InitializeAvailableSqlServerAuthenticationModes()
		{
			AvailableSqlServerAuthenticationModes = new ObservableCollection<SqlServerAuthenticationMode>
			{
				SqlServerAuthenticationMode.Windows,
				SqlServerAuthenticationMode.SqlServer
			};
		}

		public bool UseSqlServerAuthentication {
			get { return SqlServerAuthenticationMode == SqlServerAuthenticationMode.SqlServer; }
		}

		private string _dbUserID;
		public string DBUserID
		{
			get { return _dbUserID; }
			set
			{
				if (_dbUserID == value)
					return;
				_dbUserID = value;
				OnPropertyChanged(() => DBUserID);
			}
		}

		private string _dbUserPwd;
		public string DBUserPwd
		{
			get { return _dbUserPwd; }
			set
			{
				if (_dbUserPwd == value)
					return;
				_dbUserPwd = value;
				OnPropertyChanged(() => DBUserPwd);
			}
		}

		public RelayCommand CheckSqlServerConnectionCommand { get; private set; }
		private void OnCheckSqlServerConnection()
		{
			var operationResult = FiresecManager.FiresecService.CheckSqlServerConnection(DBServerAddress, DBServerPort,
				DBServerName, SqlServerAuthenticationMode == SqlServerAuthenticationMode.Windows, DBUserID, DBUserPwd);

			var msg = String.Format("Соединение с сервером {0} {1}", DBServerName, operationResult.HasError ? String.Format("установить не удалось по причине ошибки: \n\n{0}", operationResult.Error) : "успешно установлено");

			if (operationResult.HasError)
				MessageBoxService.ShowWarning(msg);
			else
				MessageBoxService.Show(msg);
		}

		public AppServerViewModel()
		{
			CheckSqlServerConnectionCommand = new RelayCommand(OnCheckSqlServerConnection);
			InitializeAvailableIpAddresses();
			InitializeAvailableSqlServerAuthenticationModes();
			ReadFromServer();
		}

		private void ReadFromServer()
		{
			var result = FiresecManager.FiresecService.GetAppServerSettings();
			if (result.HasError)
				return;
			
			var settings = result.Result;

			ServiceAddress = settings.ServiceAddress;
			ServicePort = settings.ServicePort;
			ReportServicePort = settings.ReportServicePort;
			EnableRemoteConnections = settings.EnableRemoteConnections;
			CreateNewDBOnOversize = settings.CreateNewDBOnOversize;

			// Параметры соединения с СУБД
			DBServerAddress = settings.DBServerAddress;
			DBServerPort = settings.DBServerPort;
			DBServerName = settings.DBServerName;
			SqlServerAuthenticationMode = settings.DBUseIntegratedSecurity
				? SqlServerAuthenticationMode.Windows
				: SqlServerAuthenticationMode.SqlServer;
			DBUserID = settings.DBUserID;
			DBUserPwd = settings.DBUserPwd;
		}

		private void WriteToServer()
		{
			var settings = new AppServerSettings
			{
				ServiceAddress = ServiceAddress,
				ServicePort = ServicePort,
				ReportServicePort = ReportServicePort,
				EnableRemoteConnections = EnableRemoteConnections,
				CreateNewDBOnOversize = CreateNewDBOnOversize,
				DBServerAddress = DBServerAddress,
				DBServerPort = DBServerPort,
				DBServerName = DBServerName,
				DBUseIntegratedSecurity = SqlServerAuthenticationMode == SqlServerAuthenticationMode.Windows,
				DBUserID = DBUserID,
				DBUserPwd = DBUserPwd
			};
			FiresecManager.FiresecService.SetAppServerSettings(settings);
		}

		public void Save()
		{
			WriteToServer();
		}
	}

	public enum SqlServerAuthenticationMode
	{
		[Description("Windows")]
		Windows,
		[Description("SQL Server")]
		SqlServer
	}
}
