using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhService.Monitor.ViewModels
{
	/// <summary>
	/// Описывает настройки параметров СУБД
	/// </summary>
	public class DatabaseSettingsViewModel : BaseViewModel
	{
		private string _dbServerName;
		private string _dbServerAddress;
		private int _dbServerPort;
		private SqlServerAuthenticationMode _sqlServerAuthenticationMode;
		private string _dbUserID;
		private string _dbUserPwd;

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

		/// <summary>
		/// IP-адрес сервера
		/// </summary>
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

		/// <summary>
		/// Порт сервера
		/// </summary>
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

		/// <summary>
		/// Метод аутентификации
		/// </summary>
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

		public bool UseSqlServerAuthentication
		{
			get { return SqlServerAuthenticationMode == SqlServerAuthenticationMode.SqlServer; }
		}

		/// <summary>
		/// Логин
		/// </summary>
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

		/// <summary>
		/// Пароль
		/// </summary>
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

		public RelayCommand BackupDatabaseCommand { get; private set; }

		public RelayCommand CheckSqlServerConnectionCommand { get; private set; }

		public RelayCommand ApplyCommand { get; private set; }

		public DatabaseSettingsViewModel()
		{
			BackupDatabaseCommand = new RelayCommand(OnBackupDatabase);
			CheckSqlServerConnectionCommand = new RelayCommand(OnCheckSqlServerConnection);
			ApplyCommand = new RelayCommand(OnApply);
			InitializeAvailableSqlServerAuthenticationModes();
			ReadFromModel();
		}

		private void OnBackupDatabase()
		{
			var dlg = new FolderBrowserDialog();
			var dlgResult = dlg.ShowDialog();
			if (dlgResult != DialogResult.OK)
				return;

			Logger.Info(string.Format("Создание бэкапа базы с сохранением в папку '{0}'", dlg.SelectedPath));

			var sb = new StringBuilder();
			var backupResult = true;
			foreach (var dbName in new[] { "Journal_1", "PassJournal_1", "SKD" })
			{
				string errors;
				backupResult &= ServiceRepository.Instance.DatabaseService.CreateBackup(DBServerAddress, DBServerPort,
					DBServerName, SqlServerAuthenticationMode == SqlServerAuthenticationMode.Windows, DBUserID, DBUserPwd, dbName, Path.Combine(dlg.SelectedPath, string.Format("{0}.bak", dbName)), out errors);
				if (!string.IsNullOrEmpty(errors))
					sb.AppendLine(errors);
			}

			var msg = string.Format("Операция бэкапа базы завершилась {0}", backupResult ? "успешно" : string.Format("с ошибкой: \n\n{0}", sb));

			if (!backupResult)
			{
				Logger.Warn(msg);
				MessageBoxService.ShowWarning(msg);
			}
			else
			{
				Logger.Info(msg);
				MessageBoxService.Show(msg);
			}
		}

		private void InitializeAvailableSqlServerAuthenticationModes()
		{
			AvailableSqlServerAuthenticationModes = new ObservableCollection<SqlServerAuthenticationMode>
			{
				SqlServerAuthenticationMode.Windows,
				SqlServerAuthenticationMode.SqlServer
			};
		}
		
		private void OnCheckSqlServerConnection()
		{
			Logger.Info("Проверка соединения с СУБД");

			string errors;

			var checkResult = ServiceRepository.Instance.DatabaseService.CheckConnection(DBServerAddress, DBServerPort,
				DBServerName, SqlServerAuthenticationMode == SqlServerAuthenticationMode.Windows, DBUserID, DBUserPwd, out errors);

			var msg = string.Format("Соединение с сервером {0} {1}", DBServerName, checkResult ? "успешно установлено" : string.Format("установить не удалось по причине ошибки: \n\n{0}", errors));

			if (!checkResult)
			{
				Logger.Warn(msg);
				MessageBoxService.ShowWarning(msg);
			}
			else
			{
				Logger.Info(msg);
				MessageBoxService.Show(msg);
			}
		}

		private void OnApply()
		{
			WriteToModel();
			if (ServiceRepository.Instance.ServiceStateHolder.State != ServiceState.Stoped)
				MessageBoxService.ShowWarning("Параметры вступят в силу после перезапуска сервера приложений");
		}

		private void ReadFromModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;
			DBServerAddress = settings.DBServerAddress;
			DBServerPort = settings.DBServerPort;
			DBServerName = settings.DBServerName;
			SqlServerAuthenticationMode = settings.DBUseIntegratedSecurity
				? SqlServerAuthenticationMode.Windows
				: SqlServerAuthenticationMode.SqlServer;
			DBUserID = settings.DBUserID;
			DBUserPwd = settings.DBUserPwd;
		}

		private void WriteToModel()
		{
			var settings = AppServerSettingsHelper.AppServerSettings;
			settings.DBServerAddress = DBServerAddress;
			settings.DBServerPort = DBServerPort;
			settings.DBServerName = DBServerName;
			settings.DBUseIntegratedSecurity = SqlServerAuthenticationMode == SqlServerAuthenticationMode.Windows;
			settings.DBUserID = DBUserID;
			settings.DBUserPwd = DBUserPwd;
			AppServerSettingsHelper.Save();
		}
	}

	public enum SqlServerAuthenticationMode
	{
		[Description("Windows")]
		Windows,
		[Description("SQL Sever")]
		SqlServer
	}
}