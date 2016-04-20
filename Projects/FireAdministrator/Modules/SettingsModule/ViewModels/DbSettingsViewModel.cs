using RubezhAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SettingsModule.ViewModels
{
	public class DbSettingsViewModel : BaseViewModel
	{
		public DbSettingsViewModel()
		{
			DbTypes = new ObservableCollection<DbType>(Enum.GetValues(typeof(DbType)) as IEnumerable<DbType>);
			GetConnectionParams();
		}

		string _dbConnectionString;
		public string DbConnectionString
		{
			get { return _dbConnectionString; }
			set
			{
				_dbConnectionString = value;
				OnPropertyChanged(() => DbConnectionString);
			}
		}

		public ObservableCollection<DbType> DbTypes { get; private set; }

		DbType _dbType;
		public DbType SelectedDbType
		{
			get { return _dbType; }
			set
			{
				_dbType = value;
				OnPropertyChanged(() => SelectedDbType);
				OnPropertyChanged(() => IsMsSQL);
				OnPropertyChanged(() => IsPostgres);
			}
		}
		public bool IsMsSQL { get { return SelectedDbType == DbType.MsSql; } }
		public bool IsPostgres { get { return SelectedDbType == DbType.Postgres; } }

		bool _isCreateConnectionString;
		public bool IsCreateConnectionString
		{
			get { return _isCreateConnectionString; }
			set
			{
				_isCreateConnectionString = value;
				OnPropertyChanged(() => IsCreateConnectionString);
				OnPropertyChanged(() => IsSetConnectionString);
				OnPropertyChanged(() => IsCanSetLogin);
			}
		}
		public bool IsSetConnectionString { get { return !IsCreateConnectionString; } }

		string _dataSource;
		public string DataSource
		{
			get { return _dataSource; }
			set
			{
				_dataSource = value;
				OnPropertyChanged(() => DataSource);
			}
		}

		bool _isSQLAuthentication;
		public bool IsSQLAuthentication
		{
			get { return _isSQLAuthentication; }
			set
			{
				_isSQLAuthentication = value;
				OnPropertyChanged(() => IsSQLAuthentication);
				OnPropertyChanged(() => IsCanSetLogin);
			}
		}
		public bool IsCanSetLogin { get { return IsSQLAuthentication && IsCreateConnectionString; } }

		string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		string _dbName;
		public string DbName
		{
			get { return _dbName; }
			set
			{
				_dbName = value;
				OnPropertyChanged(() => DbName);
			}
		}

		string _server;
		public string Server
		{
			get { return _server; }
			set
			{
				_server = value;
				OnPropertyChanged(() => Server);
			}
		}

		int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		public void Save()
		{
			GlobalSettingsHelper.GlobalSettings.DbSettings.DbType = SelectedDbType;
			GlobalSettingsHelper.GlobalSettings.DbSettings.IsFullConnectionString = !IsCreateConnectionString;
			if (IsMsSQL)
			{
				GlobalSettingsHelper.GlobalSettings.DbSettings.ConnectionString = DbConnectionString;
				GlobalSettingsHelper.GlobalSettings.DbSettings.UserName = Login;
				GlobalSettingsHelper.GlobalSettings.DbSettings.Password = Password;
				GlobalSettingsHelper.GlobalSettings.DbSettings.IsSQLAuthentication = IsSQLAuthentication;
				GlobalSettingsHelper.GlobalSettings.DbSettings.DbName = DbName;
				GlobalSettingsHelper.GlobalSettings.DbSettings.DataSource = DataSource;
			}
			if (IsPostgres)
			{
				GlobalSettingsHelper.GlobalSettings.DbSettings.ConnectionString = DbConnectionString;
				GlobalSettingsHelper.GlobalSettings.DbSettings.UserName = Login;
				GlobalSettingsHelper.GlobalSettings.DbSettings.Password = Password;
				GlobalSettingsHelper.GlobalSettings.DbSettings.IsSQLAuthentication = IsSQLAuthentication;
				GlobalSettingsHelper.GlobalSettings.DbSettings.DbName = DbName;
				GlobalSettingsHelper.GlobalSettings.DbSettings.Server = Server;
				GlobalSettingsHelper.GlobalSettings.DbSettings.Port = Port;
			}
		}

		void GetConnectionParams()
		{
			SelectedDbType = GlobalSettingsHelper.GlobalSettings.DbSettings.DbType;
			IsCreateConnectionString = !GlobalSettingsHelper.GlobalSettings.DbSettings.IsFullConnectionString;
			if (IsMsSQL)
			{
				DbConnectionString = GlobalSettingsHelper.GlobalSettings.DbSettings.ConnectionString;
				Login = GlobalSettingsHelper.GlobalSettings.DbSettings.UserName;
				Password = GlobalSettingsHelper.GlobalSettings.DbSettings.Password;
				IsSQLAuthentication = GlobalSettingsHelper.GlobalSettings.DbSettings.IsSQLAuthentication;
				DbName = GlobalSettingsHelper.GlobalSettings.DbSettings.DbName;
				DataSource = GlobalSettingsHelper.GlobalSettings.DbSettings.DataSource;
			}
			if (IsPostgres)
			{
				DbConnectionString = GlobalSettingsHelper.GlobalSettings.DbSettings.ConnectionString;
				Login = GlobalSettingsHelper.GlobalSettings.DbSettings.UserName;
				Password = GlobalSettingsHelper.GlobalSettings.DbSettings.Password;
				IsSQLAuthentication = GlobalSettingsHelper.GlobalSettings.DbSettings.IsSQLAuthentication;
				DbName = GlobalSettingsHelper.GlobalSettings.DbSettings.DbName;
				Server = GlobalSettingsHelper.GlobalSettings.DbSettings.Server;
				Port = GlobalSettingsHelper.GlobalSettings.DbSettings.Port;
			}
		}
	}
}