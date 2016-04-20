using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Npgsql;
using Resurs.Processor;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class StartupSettingsViewModel : SaveCancelDialogViewModel
	{
		public StartupSettingsViewModel()
		{
			Title = "Настройка соединения";
			ConnectionString = SettingsManager.ResursSettings.ConnectionString;
			DbTypes = new ObservableCollection<DbType>(Enum.GetValues(typeof(DbType)) as IEnumerable<DbType>);
			SelectedDbType = SettingsManager.ResursSettings.DbType;
			GetConnectionParams();
		}

		string _dbConnectionString;
		public string ConnectionString
		{
			get { return _dbConnectionString; }
			set
			{
				_dbConnectionString = value;
				OnPropertyChanged(() => ConnectionString);
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

		protected override bool Save()
		{
			SettingsManager.ResursSettings.DbType = SelectedDbType;
			if (IsCreateConnectionString)
			{
				if (IsMsSQL)
					SettingsManager.ResursSettings.ConnectionString = CreateMsSQLConnectionString();
				if (IsPostgres)
					SettingsManager.ResursSettings.ConnectionString = CreatePostgresConnectionString();
			}
			else
				SettingsManager.ResursSettings.ConnectionString = ConnectionString;
			SettingsManager.Save();
			LoginService.RestartApplication();
			return base.Save();
		}

		string CreateMsSQLConnectionString()
		{
			var builder = new SqlConnectionStringBuilder();
			builder.DataSource = DataSource;
			builder.InitialCatalog = _dbName;
			if (IsSQLAuthentication)
			{
				builder.UserID = Login;
				builder.Password = Password;
				builder.IntegratedSecurity = false;
			}
			else
			{
				builder.IntegratedSecurity = true;
			}
			return builder.ConnectionString;
		}

		string CreatePostgresConnectionString()
		{
			var builder = new NpgsqlConnectionStringBuilder();
			builder.Database = DbName;
			builder.Host = Server;
			builder.Port = Port;

			if (IsSQLAuthentication)
			{
				builder.IntegratedSecurity = false;
				builder.UserName = Login;
				builder.Password = Password;
			}
			return builder.ConnectionString;
		}

		void GetConnectionParams()
		{
			try
			{
				if (IsMsSQL)
				{
					var builder = new SqlConnectionStringBuilder(ConnectionString);
					Login = builder.UserID;
					Password = builder.Password;
					IsSQLAuthentication = !builder.IntegratedSecurity;
					DbName = builder.InitialCatalog;
					DataSource = builder.DataSource;
				}
				if (IsPostgres)
				{
					var builder2 = new NpgsqlConnectionStringBuilder(ConnectionString);
					Login = builder2.UserName;
					Password = builder2.Password;
					IsSQLAuthentication = !builder2.IntegratedSecurity;
					DbName = builder2.Database;
					Server = builder2.Host;
					Port = builder2.Port;
				}
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
			}
		}
	}
}