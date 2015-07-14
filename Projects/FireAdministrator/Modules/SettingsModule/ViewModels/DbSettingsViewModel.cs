using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SettingsModule.ViewModels
{
	public class DbSettingsViewModel : BaseViewModel
	{
		public DbSettingsViewModel()
		{
			Server_EnableRemoteConnections = GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections;
			UseHasp = GlobalSettingsHelper.GlobalSettings.UseHasp;
			DbConnectionString = GlobalSettingsHelper.GlobalSettings.DbConnectionString;
			var dbTypes = Enum.GetValues(typeof(DbType));
			DbTypes = new ObservableCollection<DbType>();
			foreach (DbType dbType in dbTypes)
			{
				DbTypes.Add(dbType);
			}
			SelectedDbType = GlobalSettingsHelper.GlobalSettings.DbType;
			GetConnectionParams();
		}

		bool _server_EnableRemoteConnections;
		public bool Server_EnableRemoteConnections
		{
			get { return _server_EnableRemoteConnections; }
			set
			{
				_server_EnableRemoteConnections = value;
				OnPropertyChanged(() => Server_EnableRemoteConnections);
			}
		}

		bool _useHasp;
		public bool UseHasp
		{
			get { return _useHasp; }
			set
			{
				_useHasp = value;
				OnPropertyChanged(() => UseHasp);
			}
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
			}
		}
		public bool IsMsSQL { get { return SelectedDbType == DbType.MsSql; } }

		bool _IsCreateConnectionString;
		public bool IsCreateConnectionString
		{
			get { return _IsCreateConnectionString; }
			set
			{
				_IsCreateConnectionString = value;
				OnPropertyChanged(() => IsCreateConnectionString);
				OnPropertyChanged(() => IsSetConnectionString);
			}
		}
		public bool IsSetConnectionString { get { return !IsCreateConnectionString; } }

		string _IpAddress;
		public string DataSource
		{
			get { return _IpAddress; }
			set
			{
				_IpAddress = value;
				OnPropertyChanged(() => DataSource);
			}
		}

		string _InstanceName;
		public string InstanceName
		{
			get { return _InstanceName; }
			set
			{
				_InstanceName = value;
				OnPropertyChanged(() => InstanceName);
			}
		}

		bool _IsSQLAuthentication;
		public bool IsSQLAuthentication
		{
			get { return _IsSQLAuthentication; }
			set
			{
				_IsSQLAuthentication = value;
				OnPropertyChanged(() => IsSQLAuthentication);
			}
		}

		string _Login;
		public string Login
		{
			get { return _Login; }
			set
			{
				_Login = value;
				OnPropertyChanged(() => Login);
			}
		}

		string _Password;
		public string Password
		{
			get { return _Password; }
			set
			{
				_Password = value;
				OnPropertyChanged(() => Password);
			}
		}

		string _DbName;
		public string DbName
		{
			get { return _DbName; }
			set
			{
				_DbName = value;
				OnPropertyChanged(() => DbName);
			}
		}

		public void Save()
		{
			GlobalSettingsHelper.GlobalSettings.DbConnectionString = CreateConnectionString();
			GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections = Server_EnableRemoteConnections;
			GlobalSettingsHelper.GlobalSettings.UseHasp = UseHasp;
			GlobalSettingsHelper.GlobalSettings.DbType = SelectedDbType;
		}

		string CreateConnectionString()
		{
			if (!IsCreateConnectionString)
				return DbConnectionString;
			var builder = new SqlConnectionStringBuilder();
			builder.DataSource = DataSource;
			if (IsSQLAuthentication)
			{
				builder.UserID = Login;
				builder.Password = Password;
				builder.IntegratedSecurity = false;
				builder.InitialCatalog = _DbName;
			}
			else
			{
				builder.IntegratedSecurity = true;
			}
			return builder.ConnectionString;
		}

		void GetConnectionParams()
		{
			if(IsMsSQL)
			{
				var builder = new SqlConnectionStringBuilder(DbConnectionString);
				Login = builder.UserID;
				Password = builder.Password;
				IsSQLAuthentication = builder.IntegratedSecurity;
				DbName = builder.InitialCatalog;
				DataSource = builder.DataSource;
			}
		}
	}
}
