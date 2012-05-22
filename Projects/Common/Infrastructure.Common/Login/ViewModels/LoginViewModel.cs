using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Infrastructure.Common.MessageBox;

namespace Infrastructure.Common.Login.ViewModels
{
	public class LoginViewModel : DialogContent
	{
		public LoginViewModel()
		{
			ConnectCommand = new RelayCommand(OnConnect);
			CancelCommand = new RelayCommand(OnCancel);

			//UserName = ServiceFactory.AppSettings.UserName;
			//Password = ServiceFactory.AppSettings.SavePassword ? ServiceFactory.AppSettings.Password : string.Empty;
			//SavePassword = ServiceFactory.AppSettings.SavePassword;
			IsConnected = false;
			IsCanceled = false;
			Message = null;
		}

		string _userName;
		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				OnPropertyChanged("UserName");
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
				OnPropertyChanged("Password");
			}
		}
		private bool _savePassword;
		public bool SavePassword
		{
			get { return _savePassword; }
			set
			{
				_savePassword = value;
				OnPropertyChanged("SavePassword");
			}
		}

		public bool IsConnected { get; private set; }
		public bool IsCanceled { get; private set; }
		public string Message { get; private set; }

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			IsCanceled = false;
			Close(true);
			//DoConnect(ServiceFactory.AppSettings.ServiceAddress, UserName, Password);
		}

		void OnCancel()
		{
			IsCanceled = true;
			Close(false);
		}

		void DoConnect(string serverAddress, string userName, string password)
		{
			Surface.Hide();
			using (new WaitWrapper())
			{
				var preLoadWindow = new PreLoadWindow()
				{
					PreLoadText = "Соединение с сервером..."
				};
				preLoadWindow.Show();
				//Message = FiresecManager.Connect("Администратор", serverAddress, userName, password);
				IsConnected = string.IsNullOrEmpty(Message);
				preLoadWindow.Close();
			}
			if (IsConnected)
			{
				//ServiceFactory.AppSettings.UserName = UserName;
				//ServiceFactory.AppSettings.Password = SavePassword ? password : string.Empty;
				//ServiceFactory.AppSettings.SavePassword = SavePassword;
				//AppSettingsHelper.SaveAppSettings();
			}
		}
	}

}
