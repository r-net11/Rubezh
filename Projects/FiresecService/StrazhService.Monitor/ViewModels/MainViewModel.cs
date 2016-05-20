using System.Collections.Generic;
using System.ComponentModel;
using Common;
using FiresecClient;
using KeyGenerator;
using KeyGenerator.Entities;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace StrazhService.Monitor.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel(ILicenseManager currentLicenseManager)
		{
			if (currentLicenseManager == null)
				throw new ArgumentNullException("currentLicenseManager");

			ServiceControlViewModel = new ServiceControlViewModel();
			AppServerSettingsViewModel = new AppServerSettingsViewModel();
			DatabaseSettingsViewModel = new DatabaseSettingsViewModel();

			_currentLicenseManager = currentLicenseManager;
			LoadLicenseCommand = new RelayCommand(OnLoadLicense);
			Current = this;
			Title = "Монитор сервера";
			_dispatcher = Dispatcher.CurrentDispatcher;
			Clients = new ObservableCollection<ClientViewModel>();
			LicenseItems = GetLicenseDictionary(currentLicenseManager.CurrentLicense);
			UserKey = currentLicenseManager.GetUserKey();
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);
			UpdateLicenseStatus();
			ReleaseClientsCommand = new RelayCommand(OnReleaseClients, CanReleaseClients);

			SafeFiresecService.NewJournalItemEvent -= SafeFiresecService_NewJournalItemEvent;
			SafeFiresecService.NewJournalItemEvent += SafeFiresecService_NewJournalItemEvent;
		}

		#region <Управление службой>

		/// <summary>
		/// Управление Windows-службой
		/// </summary>
		public ServiceControlViewModel ServiceControlViewModel { get; private set; }

		#endregion </Управление службой>

		#region <Соединения>

		public ObservableCollection<ClientViewModel> Clients { get; private set; }

		private ClientViewModel _selectedClient;

		public ClientViewModel SelectedClient
		{
			get { return _selectedClient; }
			set
			{
				_selectedClient = value;
				OnPropertyChanged(() => SelectedClient);
			}
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = new ClientViewModel(clientCredentials);
				Clients.Add(connectionViewModel);
			}));
		}

		public void RemoveClient(Guid uid)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					Clients.Remove(connectionViewModel);
			}));
		}

		public void EditClient(Guid uid, string userName)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
					connectionViewModel.FriendlyUserName = userName;
			}));
		}

		private void SafeFiresecService_NewJournalItemEvent(JournalItem journalItem)
		{
			if (journalItem.JournalEventNameType != JournalEventNameType.Вход_пользователя_в_систему &&
				journalItem.JournalEventNameType != JournalEventNameType.Выход_пользователя_из_системы)
				return;

			UpdateClients();
		}

		private void UpdateClients()
		{
			Logger.Info("Обновляем список Клиентов Сервера");
			var serverClients = ClientsManager.GetServerClients();
			_dispatcher.BeginInvoke((Action)(() =>
			{
				// Удаляем из списка Клиентов, которых уже нет
				var removedClients = Clients.Where(vm => serverClients.All(sc => sc.ClientUID != vm.UID)).ToList();
				foreach (var removedClient in removedClients)
				{
					RemoveClient(removedClient.UID);
				}

				// Добавляем в список новых Клиентов
				var addedClients = serverClients.Where(c => Clients.All(vm => vm.UID != c.ClientUID)).ToList();
				foreach (var addedClient in addedClients)
				{
					AddClient(addedClient);
				}
			}));
		}

		public RelayCommand ReleaseClientsCommand { get; private set; }
		private void OnReleaseClients()
		{
			var clientsToDisconnect = Clients.Where(x => x.IsChecked).ToList();
			foreach (var client in clientsToDisconnect)
			{
				// Просим Сервер посылать Клиенту команду на разрыв соединения
				FiresecManager.FiresecService.SendDisconnectClientCommand(client.ClientCredentials.ClientUID);
			}
			foreach (var client in clientsToDisconnect)
			{
				// Т.к. "зависший" Клиент не вызовет на Сервере Disconnect, то удаляем информацию о нем (освобождаем занятую лицензию)
				//ClientsManager.Remove(client.ClientCredentials.ClientUID);
			}
		}
		private bool CanReleaseClients()
		{
			return Clients.Any(x => x.IsChecked);
		}

		#endregion </Соединения>

		#region <Настройки>

		/// <summary>
		/// Настройки параметров запуска Серевера приложений
		/// </summary>
		public AppServerSettingsViewModel AppServerSettingsViewModel { get; private set; }

		#endregion </Настройки>

		#region <База данных>

		/// <summary>
		/// Настройки параметров СУБД
		/// </summary>
		public DatabaseSettingsViewModel DatabaseSettingsViewModel { get; private set; }

		#endregion </База данных>

		#region <Лицензирование>

		private const string LicLoadAccept = "Лицензия загружена";
		private const string LicLoadFailed = "Лицензия отстутствует";

		private Dictionary<string, string> _licenseItems;

		public Dictionary<string, string> LicenseItems
		{
			get { return _licenseItems; }
			private set
			{
				_licenseItems = value;
				OnPropertyChanged(() => LicenseItems);
			}
		}
		public string UserKey { get; private set; }
		private string _licenseStatusText;

		public string LicenseStatusText
		{
			get { return _licenseStatusText; }
			set
			{
				_licenseStatusText = value;
				OnPropertyChanged(() => LicenseStatusText);
				OnPropertyChanged(() => HasLicense);
			}
		}

		public bool HasLicense
		{
			get { return LicenseItems.Any(); }
		}

		public RelayCommand LoadLicenseCommand { get; set; }
		private readonly ILicenseManager _currentLicenseManager;

		private void UpdateLicenseStatus()
		{
			LicenseStatusText = LicenseItems.Any() ? LicLoadAccept : LicLoadFailed;
		}

		public static Dictionary<string, string> GetLicenseDictionary(LicenseEntity currentLicense)
		{
			var dict = new Dictionary<string, string>();

			if (currentLicense == null) return dict;

			var props = currentLicense.GetType().GetProperties();
			foreach (var prop in props)
			{
				var attrs = prop.GetCustomAttributes(true);
				foreach (var attr in attrs)
				{
					var authAttr = attr as DescriptionAttribute;
					if (authAttr != null)
					{
						var propName = PropertyConverter(prop.GetValue(currentLicense, null));
						var auth = authAttr.Description;

						dict.Add(auth, propName);
					}
				}
			}

			return dict;
		}

		private static string PropertyConverter(object propertyValue)
		{
			if (propertyValue is bool)
				return (bool)propertyValue ? "Включено" : "Отсутствует";
			if (propertyValue is int)
				return (int)propertyValue != default(int) ? ((int)propertyValue).ToString() : "Отсутствует";

			return string.Empty;
		}

		private void OnLoadLicense()
		{
			var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".lic", Filter = "License files (.lic)|*.lic" };

			var result = dlg.ShowDialog();

			if (result != true) return;

			if (_currentLicenseManager.LoadLicenseFromFile(dlg.FileName))
			{
				LicenseItems = GetLicenseDictionary(_currentLicenseManager.CurrentLicense);
				UpdateLicenseStatus();
				Logger.Info("Уведомляем Сервер об изменении лицензии");
				FiresecManager.FiresecService.NotifyLicenseChanged();
			}
			else
				MessageBoxService.ShowError(
					"Выбранный файл не является файлом лицензии и не может быть использован для активации сервера. Выберите другой файл",
					"Ошибка чтения файла лицензии");
		}

		#endregion <Лицензирование>

		#region <Лог>

		public void AddLog(string message)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				LastLog = message;
				InfoLog += message + "\n";
			}));
		}

		private string _lastLog;

		public string LastLog
		{
			get { return _lastLog; }
			set
			{
				_lastLog = value;
				OnPropertyChanged(() => LastLog);
			}
		}

		private string _infoLog;

		public string InfoLog
		{
			get { return _infoLog; }
			set
			{
				_infoLog = value;
				OnPropertyChanged(() => InfoLog);
			}
		}

		#endregion </Лог>

		private readonly Dispatcher _dispatcher;

		private void MessageBoxHandler(MessageBoxViewModel viewModel, bool isModal)
		{
			_dispatcher.Invoke((Action)(() =>
			{
				var startupMessageBoxViewModel = new ServerMessageBoxViewModel(viewModel.Title, viewModel.Message, viewModel.MessageBoxButton, viewModel.MessageBoxImage, viewModel.IsException);
				if (isModal)
					DialogService.ShowModalWindow(startupMessageBoxViewModel);
				else
					DialogService.ShowWindow(startupMessageBoxViewModel);
				viewModel.Result = startupMessageBoxViewModel.Result;
			}));
		}

		public override int GetPreferedMonitor()
		{
			return MonitorHelper.PrimaryMonitor;
		}

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
	}
}