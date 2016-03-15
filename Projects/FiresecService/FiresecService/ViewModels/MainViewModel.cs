using System.Collections.Generic;
using System.ComponentModel;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using KeyGenerator;
using KeyGenerator.Entities;

namespace FiresecService.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

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

		public RelayCommand LoadLicenseCommand { get; set; }

		private readonly Dispatcher _dispatcher;
		private readonly ILicenseManager _currentLicenseManager;

		public MainViewModel(ILicenseManager currentLicenseManager)
		{
			if(currentLicenseManager == null)
				throw new ArgumentException("License Manager is null");

			_currentLicenseManager = currentLicenseManager;
			LoadLicenseCommand = new RelayCommand(OnLoadLicense);
			Current = this;
			Title = "Сервер приложений";
			_dispatcher = Dispatcher.CurrentDispatcher;
			Clients = new ObservableCollection<ClientViewModel>();
			LicenseItems = GetLicenseDictionary(currentLicenseManager.CurrentLicense);
			UserKey = currentLicenseManager.GetUserKey();
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);
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
				return (bool) propertyValue ? "Включено" : "Отсутствует";
			if (propertyValue is int)
				return (int) propertyValue != default (int) ? ((int) propertyValue).ToString() : "Отсутствует";

			return string.Empty;
		}

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

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}

		private void OnLoadLicense()
		{
			var dlg = new Microsoft.Win32.OpenFileDialog {DefaultExt = ".lic", Filter = "License files (.lic)|*.lic"};

			var result = dlg.ShowDialog();

			if (result != true) return;

			if (_currentLicenseManager.LoadLicenseFromFile(dlg.FileName))
				LicenseItems = GetLicenseDictionary(_currentLicenseManager.CurrentLicense);
			else
				MessageBoxService.ShowError("Лицензия не может быть загружена", "Ошибка чтения файла лицензии"); //TODO: detalisations of errors
		}
	}
}