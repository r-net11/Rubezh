using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Resurs.Processor;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Resurs.ViewModels
{
	public class StartupViewModel : DialogViewModel
	{
		public StartupViewModel()
		{
			DbCache.CheckConnection();
			HeaderCommandViewModel = new StartupHeaderViewModel(this);
			Title = "Вход в АРМ Ресурс";
			Sizable = false;
			TopMost = true;
			AllowClose = false;
			CloseOnEscape = true;
			HideInTaskbar = true;
			AllowClose = true;

			ConnectCommand = new RelayCommand(OnConnect);
			CancelCommand = new RelayCommand(OnCancel);
			SettingsCommand = new RelayCommand(OnSettings);

#if DEBUG
			Login = "adm";
#endif
		}

		public override void OnLoad()
		{
			Surface.WindowStyle = WindowStyle.None;
			Surface.AllowsTransparency = true;
			Surface.Background = new SolidColorBrush(Colors.Transparent);
			Surface.SizeToContent = SizeToContent.WidthAndHeight;
			Surface.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

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
		[ObfuscationAttribute(Exclude = true)]
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

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

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			Message = LoginService.Login(Login, Password);
			if(Message == null)
			{
				//ApplicationService.DoEvents(Dispatcher);
				Close(true);
				DbCache.AddJournalForUser(JournalType.System);
			}
		}
		public RelayCommand CancelCommand { get; private set; }
		private void OnCancel()
		{
			Message = "Отмена соединения";
		}
		public RelayCommand SettingsCommand { get; private set; }
		private void OnSettings()
		{
			var startupSettingsViewModel = new StartupSettingsViewModel();
			if (DialogService.ShowModalWindow(startupSettingsViewModel))
			{

			}
		}

		private string GetConnectingMessage()
		{
			var message = "Соединение с сервером";
			return message;
		}
	}
}