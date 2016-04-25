using Infrastructure.Client.Properties;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupViewModel : DialogViewModel
	{
		ClientType _clientType;
		AutoResetEvent _sync;
		StartupLoginViewModel _startupLoginViewModel;
		StartupLoadingViewModel _startupLoadingViewModel;

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
			MessageFontSize = 12;
			Dispatcher = Dispatcher.CurrentDispatcher;
		}

		public override void OnLoad()
		{
			Surface.WindowStyle = WindowStyle.None;
			Surface.AllowsTransparency = true;
			Surface.Background = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
			Surface.SizeToContent = SizeToContent.WidthAndHeight;
			Surface.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}
		public override bool OnClosing(bool isCanceled)
		{
			var result = base.OnClosing(isCanceled);
			if (!result && _sync != null)
				_sync.Set();
			return result;
		}

		public Dispatcher Dispatcher { get; private set; }
		bool _isUIEnabled;
		public bool IsUIEnabled
		{
			get { return _isUIEnabled; }
			set
			{
				_isUIEnabled = value;
				OnPropertyChanged(() => IsUIEnabled);
			}
		}

		bool _showButtons;
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
		string _message;
		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				OnPropertyChanged(() => Message);
			}
		}

		int _messageFontSize;
		public int MessageFontSize
		{
			get { return _messageFontSize; }
			set
			{
				_messageFontSize = value;
				OnPropertyChanged(() => MessageFontSize);
			}
		}

		FontWeight _messageFontWeight;
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
				_startupLoadingViewModel.Text = text;
				_startupLoadingViewModel.StepCount = stepCount;
				Content = _startupLoadingViewModel;
				OnPropertyChanged(() => Content);
				ShowButtons = false;
				IsUIEnabled = true;
			}), DispatcherPriority.Send);
		}
		public void DoStep(string text)
		{
			Dispatcher.Invoke((Action)(() => _startupLoadingViewModel.DoStep(text)), DispatcherPriority.Send);
		}
		public void AddCount(int count)
		{
			Dispatcher.Invoke((Action)(() => _startupLoadingViewModel.StepCount += count), DispatcherPriority.Send);
		}

		void Login(string login, string password)
		{
			bool isAutoconnect;
			switch (_clientType)
			{
				case ClientType.Administrator:
					isAutoconnect = GlobalSettingsHelper.GlobalSettings.AdminAutoConnect;
					break;
				case ClientType.Monitor:
					isAutoconnect = GlobalSettingsHelper.GlobalSettings.MonitorAutoConnect;
					break;
				default:
					isAutoconnect = false;
					break;
			}

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
					switch (_clientType)
					{
						case ClientType.Administrator:
							_startupLoginViewModel.UserName = GlobalSettingsHelper.GlobalSettings.AdminLogin;
							_startupLoginViewModel.Password = GlobalSettingsHelper.GlobalSettings.AdminPassword;
							break;
						case ClientType.Monitor:
							_startupLoginViewModel.UserName = GlobalSettingsHelper.GlobalSettings.MonitorLogin;
							_startupLoginViewModel.Password = GlobalSettingsHelper.GlobalSettings.MonitorPassword;
							break;
						default:
							break;
					}

				}
				else
				{
					_startupLoginViewModel.UserName = Settings.Default.UserName;
					_startupLoginViewModel.Password = Settings.Default.Password;
				}
				if (isAutoconnect)
					ConnectCommand.Execute();
			}
		}

		public RelayCommand ConnectCommand { get; private set; }

		void OnConnect()
		{
			IsUIEnabled = false;
			Message = GetConnectingMessage();
			MessageFontSize = 14;
			MessageFontWeight = FontWeights.Black;
			OnPropertyChanged(() => Content);
			ApplicationService.DoEvents(Dispatcher);

			var result = ClientManager.Connect(_clientType, ConnectionSettingsManager.ServerAddress, _startupLoginViewModel.UserName, _startupLoginViewModel.Password);
			if (string.IsNullOrEmpty(result))
			{
				for (int i = 1; i < 100; i++)
				{
					var serverState = ClientManager.FiresecService.GetServerState();
					if (!serverState.HasError)
					{
						switch (serverState.Result)
						{
							case ServerState.Starting:
								Message = "Сервер запускается";
								break;

							case ServerState.Restarting:
								Message = "Сервер перезапускается";
								break;

							case ServerState.Ready:
								Message = null;
								SetConnected(true);
								return;
						}
					}
					else
					{
						Message = serverState.Error;
					}
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}
				Message = "Превышено время ожидания запуска сервера";
			}
			else
			{
				Message = result;
			}

			MessageFontSize = 12;
			MessageFontWeight = FontWeights.Regular;
			IsUIEnabled = true;
		}
		public RelayCommand CancelCommand { get; private set; }

		void OnCancel()
		{
			SetConnected(false);
		}
		public RelayCommand SettingsCommand { get; private set; }

		void OnSettings()
		{
			StartupSettingsWaitHandler = new AutoResetEvent(false);
			ApplicationService.BeginInvoke(() =>
			{
				if (StartupSettingsWaitHandler != null)
				{
					StartupSettingsWaitHandler.WaitOne();
					StartupSettingsWaitHandler.Dispose();
				}
				StartupSettingsWaitHandler = null;
			});
			DialogService.ShowModalWindow(new StartupSettingsViewModel(_clientType));
			StartupSettingsWaitHandler.Set();
		}

		public AutoResetEvent StartupSettingsWaitHandler { get; private set; }

		void SetConnected(bool isConnected)
		{
			IsConnected = isConnected;
			SaveSettings();
			_sync.Set();
		}

		void SaveSettings()
		{
			bool isCurrentClientAutoconnect;
			switch (_clientType)
			{
				case ClientType.Administrator:
					isCurrentClientAutoconnect = GlobalSettingsHelper.GlobalSettings.AdminAutoConnect;
					break;
				case ClientType.Monitor:
					isCurrentClientAutoconnect = GlobalSettingsHelper.GlobalSettings.MonitorAutoConnect;
					break;
				default:
					isCurrentClientAutoconnect = false;
					break;
			}

			if (IsConnected && !isCurrentClientAutoconnect && (Settings.Default.UserName != _startupLoginViewModel.UserName || Settings.Default.Password != (_startupLoginViewModel.SavePassword ? _startupLoginViewModel.Password : string.Empty) || Settings.Default.SavePassword != _startupLoginViewModel.SavePassword))
			{
				Settings.Default.UserName = _startupLoginViewModel.UserName;
				Settings.Default.Password = _startupLoginViewModel.SavePassword ? _startupLoginViewModel.Password : string.Empty;
				Settings.Default.SavePassword = _startupLoginViewModel.SavePassword;
				Settings.Default.Save();
			}
		}

		string GetConnectingMessage()
		{
			var message = "Соединение с сервером";
			if (ConnectionSettingsManager.IsRemote && !string.IsNullOrEmpty(ConnectionSettingsManager.RemoteAddress))
				message += " - " + ConnectionSettingsManager.RemoteAddress;
			return message;
		}
	}
}