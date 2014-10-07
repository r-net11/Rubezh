using System.Windows;
using System.Windows.Media;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Client.Login.ViewModels;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Client.Properties;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupViewModel : DialogViewModel
	{
		private ClientType _clientType;
		private AutoResetEvent _sync;
		private StartupLoginViewModel _startupLoginViewModel;
		private StartupLoadingViewModel _startupLoadingViewModel;

		public StartupViewModel(ClientType clientType)
		{
			HeaderCommandViewModel = new StartupHeaderViewModel(this);
			_clientType = clientType;
			Title = _clientType.ToDescription();
			Sizable = false;
			TopMost = true;
			AllowClose = false;
			CloseOnEscape = true;
			HideInTaskbar = true;
			AllowClose = true;
			IsUIEnabled = false;
			_startupLoginViewModel = new StartupLoginViewModel(_clientType);
			_startupLoadingViewModel = new StartupLoadingViewModel();
			Content = _startupLoginViewModel;
			ConnectCommand = new RelayCommand(OnConnect);
			CancelCommand = new RelayCommand(OnCancel);
			SettingsCommand = new RelayCommand(OnSettings);
			ShowButtons = true;
		}

		public override void OnLoad()
		{
			Surface.WindowStyle = WindowStyle.None;
			Surface.AllowsTransparency = true;
			Surface.Background = new SolidColorBrush(Colors.Transparent);
			Surface.SizeToContent = SizeToContent.WidthAndHeight;
			Surface.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}
		public override bool OnClosing(bool isCanceled)
		{
			var result = base.OnClosing(isCanceled);
			if (!result && _sync != null)
			{
				_sync.Set();
				result = true;
			}
			_sync = null;
			return result;
		}

		private bool _isUIEnabled;
		public bool IsUIEnabled
		{
			get { return _isUIEnabled; }
			set
			{
				_isUIEnabled = value;
				OnPropertyChanged(() => IsUIEnabled);
			}
		}
		private bool _showButtons;
		public bool ShowButtons
		{
			get { return _showButtons; }
			set
			{
				_showButtons = value;
				OnPropertyChanged(() => ShowButtons);
			}
		}

		public BaseViewModel Content { get; private set; }
		public bool IsConnected { get; private set; }
		private string _message;
		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				OnPropertyChanged(() => Message);
			}
		}
		private int _messageFontSize;
		public int MessageFontSize
		{
			get { return _messageFontSize; }
			set
			{
				_messageFontSize = value;
				OnPropertyChanged(() => MessageFontSize);
			}
		}
		private FontWeight _messageFontWeight;
		public FontWeight MessageFontWeight
		{
			get { return _messageFontWeight; }
			set
			{
				_messageFontWeight = value;
				OnPropertyChanged(() => MessageFontWeight);
			}
		}
		
		

		public bool PerformLogin(string login, string password)
		{
			using (_sync = new AutoResetEvent(false))
			{
				IsUIEnabled = true;
				Login(login, password);
				_sync.WaitOne();
			}
			_sync = null;
			return IsConnected;
		}
		public void ShowLoading(string text, int stepCount)
		{
			Dispatcher.BeginInvoke((Action)(() =>
			{
				ShowButtons = false;
				_startupLoadingViewModel.Text = text;
				_startupLoadingViewModel.StepCount = stepCount;
				Content = _startupLoadingViewModel;
				OnPropertyChanged(() => Content);
				IsUIEnabled = true;
			}));
		}
		public void DoStep(string text)
		{
			Dispatcher.BeginInvoke((Action)(() => _startupLoadingViewModel.DoStep(text)));
		}
		public void AddCount(int count)
		{
			Dispatcher.BeginInvoke((Action)(() => _startupLoadingViewModel.StepCount += count));
		}

		private void Login(string login, string password)
		{
			bool isAutoconnect = GlobalSettingsHelper.GlobalSettings.AutoConnect;
			if (login != null && password != null)
			{
				_startupLoginViewModel.UserName = login;
				_startupLoginViewModel.Password = password;
				ConnectCommand.Execute();
			}
			else
			{
				if (isAutoconnect)
				{
					_startupLoginViewModel.UserName = GlobalSettingsHelper.GlobalSettings.Login;
					_startupLoginViewModel.Password = GlobalSettingsHelper.GlobalSettings.Password;
				}
				else
				{
					_startupLoginViewModel.UserName = Settings.Default.UserName;
					_startupLoginViewModel.Password = Settings.Default.Password;
				}
				if (isAutoconnect && (_startupLoginViewModel.UserName != "adm" || !GlobalSettingsHelper.GlobalSettings.DoNotAutoconnectAdm))
					ConnectCommand.Execute();
			}
		}

		public RelayCommand ConnectCommand { get; private set; }
		private void OnConnect()
		{
			IsUIEnabled = false;
			Message = GetConnectingMessage();
			MessageFontSize = 14;
			MessageFontWeight = FontWeights.Black;
			OnPropertyChanged(() => Content);
			ApplicationService.DoEvents(Dispatcher);
			var result = FiresecManager.Connect(_clientType, ConnectionSettingsManager.ServerAddress, _startupLoginViewModel.UserName, _startupLoginViewModel.Password);
			if (string.IsNullOrEmpty(result))
			{
				Message = null;
				SetConnected(true);
			}
			else
			{
				Message = result;
				MessageFontSize = 12;
				MessageFontWeight = FontWeights.Regular;
				IsUIEnabled = true;
			}
		}
		public RelayCommand CancelCommand { get; private set; }
		private void OnCancel()
		{
			SetConnected(false);
		}
		public RelayCommand SettingsCommand { get; private set; }
		private void OnSettings()
		{
			var waitHandler = new AutoResetEvent(false);
			ApplicationService.BeginInvoke(() =>
			{
				waitHandler.WaitOne();
				waitHandler.Dispose();
			});
			DialogService.ShowModalWindow(new StartupSettingsViewModel(this));
			waitHandler.Set();
		}

		private void SetConnected(bool isConnected)
		{
			IsConnected = isConnected;
			SaveSettings();
			_sync.Set();
		}
		private void SaveSettings()
		{
			if (IsConnected && !GlobalSettingsHelper.GlobalSettings.AutoConnect && (Settings.Default.UserName != _startupLoginViewModel.UserName || Settings.Default.Password != (_startupLoginViewModel.SavePassword ? _startupLoginViewModel.Password : string.Empty)))
			{
				Settings.Default.UserName = _startupLoginViewModel.UserName;
				Settings.Default.Password = _startupLoginViewModel.SavePassword ? _startupLoginViewModel.Password : string.Empty;
				Settings.Default.SavePassword = _startupLoginViewModel.SavePassword;
				Settings.Default.Save();
			}
		}
		private string GetConnectingMessage()
		{
			var message = "Соединение с сервером";
			if (ConnectionSettingsManager.IsRemote && !string.IsNullOrEmpty(ConnectionSettingsManager.RemoteAddress))
				message += " - " + ConnectionSettingsManager.RemoteAddress;
			return message;
		}
	}
}
