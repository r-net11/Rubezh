﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Defender;
using System.IO;
using Microsoft.Win32;

namespace FiresecService.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }
		Dispatcher _dispatcher;
		public ServerTasksViewModel ServerTasksViewModel { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "Сервер приложений Глобал";
			_dispatcher = Dispatcher.CurrentDispatcher;
			Clients = new ObservableCollection<ClientViewModel>();
			ServerTasksViewModel = new ViewModels.ServerTasksViewModel();
			MessageBoxService.SetMessageBoxHandler(MessageBoxHandler);
			Logs = new ObservableCollection<LogViewModel>();
			GKViewModels = new ObservableCollection<GKViewModel>();

            _initialKey = InitialKey.Generate();
            InitialKeyString = _initialKey.ToString();
            LoadLicenseCommand = new RelayCommand(OnLoadLicenseCommand);
            TryLoadLicense();
		}

		void MessageBoxHandler(MessageBoxViewModel viewModel, bool isModal)
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
		
		string _status;
		string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged(() => Status);
			}
		}

		public override int GetPreferedMonitor()
		{
			return MonitorHelper.PrimaryMonitor;
		}

		#region Clients
		public ObservableCollection<ClientViewModel> Clients { get; private set; }

		ClientViewModel _selectedClient;
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
		#endregion Clients

		#region Logs
		public void AddLog(string message, bool isError)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				LastLog = message;
				var logViewModel = new LogViewModel(message, isError);
				Logs.Add(logViewModel);
				if (Logs.Count > 1000)
					Logs.RemoveAt(0);
			}));
		}

		string _lastLog = "";
		public string LastLog
		{
			get { return _lastLog; }
			set
			{
				_lastLog = value;
				OnPropertyChanged(() => LastLog);
			}
		}

		public ObservableCollection<LogViewModel> Logs { get; private set; }

		LogViewModel _selectedLog;
		public LogViewModel SelectedLog
		{
			get { return _selectedLog; }
			set
			{
				_selectedLog = value;
				OnPropertyChanged(() => SelectedLog);
			}
		}
		#endregion Logs

		#region Address

		public static void SetLocalAddress(string address)
		{
			if(Current != null)
			{
				Current._dispatcher.BeginInvoke((Action)(() => { Current.LocalAddress = address; }));
			}
		}

		public static void SetRemoteAddress(string address)
		{
			if (Current != null)
			{
				Current._dispatcher.BeginInvoke((Action)(() => { Current.RemoteAddress = address; }));
			}
		}

		public static void SetReportAddress(string address)
		{
			if (Current != null)
			{
				Current._dispatcher.BeginInvoke((Action)(() => { Current.ReportAddress = address; }));
			}
		}

		string _localAddress;
		public string LocalAddress
		{
			get { return _localAddress; }
			set
			{
				_localAddress = value;
				OnPropertyChanged(() => LocalAddress);
			}
		}

		string _remoteAddress;
		public string RemoteAddress
		{
			get { return _remoteAddress; }
			set
			{
				_remoteAddress = value;
				OnPropertyChanged(() => RemoteAddress);
			}
		}

		string _reportAddress;
		public string ReportAddress
		{
			get { return _reportAddress; }
			set
			{
				_reportAddress = value;
				OnPropertyChanged(() => ReportAddress);
			}
		}
		#endregion Address

		#region GK
		ObservableCollection<GKViewModel> _gkViewModels;
		public ObservableCollection<GKViewModel> GKViewModels
		{
			get { return _gkViewModels; }
			set
			{
				_gkViewModels = value;
				OnPropertyChanged(() => GKViewModels);
			}
		}

		GKViewModel _selectedGKViewModel;
		public GKViewModel SelectedGKViewModel
		{
			get { return _selectedGKViewModel; }
			set
			{
				_selectedGKViewModel = value;
				OnPropertyChanged(() => SelectedGKViewModel);
			}
		}
		#endregion GK

        #region Licensing

        InitialKey _initialKey;

        string _initialKeyString;
        public string InitialKeyString
        {
            get { return _initialKeyString; }
            set
            {
                _initialKeyString = value;
                OnPropertyChanged(()=>InitialKeyString);
            }
        }

        LicenseViewModel _license;
        public LicenseViewModel License
        {
            get { return _license; }
            set
            {
                _license = value;
                OnPropertyChanged(() => License);
            }
        }

        string GetLicensePath()
        {
            return AppDataFolderHelper.GetFile("FiresecService.license");
        }

        bool TryLoadLicense()
        {
            License = new LicenseViewModel(LicenseProcessor.ProcessLoad(GetLicensePath(), _initialKey));
            return License != null;
        }

        public RelayCommand LoadLicenseCommand { get; private set; }
        void OnLoadLicenseCommand()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Файл лицензии (*.license)|*.license"
            };
            if (openFileDialog.ShowDialog().Value)
            {
                try
                {
                    File.Copy(openFileDialog.FileName, GetLicensePath(), true);
                }
                catch(Exception ex)
                {
                    MessageBoxService.ShowError("Ошибка копирования файла лицензии.\n" + ex.Message);
                }
                TryLoadLicense();
            }
        }
        #endregion

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
    }
}