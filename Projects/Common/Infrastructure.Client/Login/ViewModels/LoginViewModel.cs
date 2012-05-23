using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Infrastructure.Common.MessageBox;
using FiresecClient;
using Infrastructure.Common;
using System.Configuration;
using Infrastructure.Client.Properties;
using Common;

namespace Infrastructure.Client.Login.ViewModels
{
	public class LoginViewModel : DialogContent
	{
		private PasswordViewType _passwordViewType;

		public LoginViewModel(string clientType)
			: this(clientType, PasswordViewType.Connect)
		{
		}
		public LoginViewModel(string clientType, PasswordViewType passwordViewType)
		{
			ConnectCommand = new RelayCommand(OnConnect);
			CancelCommand = new RelayCommand(OnCancel);

			ClientType = clientType;
			_passwordViewType = passwordViewType;

			switch (_passwordViewType)
			{
				case PasswordViewType.Connect:
					UserName = Settings.Default.UserName;
					CanEditUserName = true;
					break;

				case PasswordViewType.Reconnect:
					UserName = string.Empty;
					CanEditUserName = true;
					break;

				case PasswordViewType.Validate:
					UserName = FiresecManager.CurrentUser.Login;
					CanEditUserName = false;
					break;
			}

			Password = Settings.Default.SavePassword ? Settings.Default.Password : string.Empty;
			SavePassword = Settings.Default.SavePassword;

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
		bool _canEditUserName;
		public bool CanEditUserName
		{
			get { return _canEditUserName; }
			set
			{
				_canEditUserName = value;
				OnPropertyChanged("CanEditUserName");
			}
		}

		public bool IsConnected { get; private set; }
		public bool IsCanceled { get; private set; }
		public string Message { get; private set; }
		public string ClientType { get; private set; }

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			IsCanceled = false;
			Close(true);
			switch (_passwordViewType)
			{
				case PasswordViewType.Connect:
					DoConnect(UserName, Password);
					break;
				case PasswordViewType.Reconnect:
					Message = FiresecManager.Reconnect(UserName, Password);
					break;
				case PasswordViewType.Validate:
					Message = HashHelper.CheckPass(Password, FiresecManager.CurrentUser.PasswordHash) ? null : "Валидация не пройдена";
					break;
			}
			IsConnected = string.IsNullOrEmpty(Message);
		}
		void OnCancel()
		{
			Message = null;
			IsCanceled = true;
			Close(false);
		}

		private void DoConnect(string userName, string password)
		{
			Surface.Hide();
			//using (new WaitWrapper())
			{
				var preLoadWindow = new PreLoadWindow()
				{
					PreLoadText = "Соединение с сервером..."
				};
				preLoadWindow.Show();
				Message = FiresecManager.Connect(ClientType, GetServerAddress(), userName, password);
				preLoadWindow.Close();
			}
			if (IsConnected)
			{
				Settings.Default.UserName = UserName;
				Settings.Default.Password = SavePassword ? password : string.Empty;
				Settings.Default.SavePassword = SavePassword;
				Settings.Default.Save();
			}
		}

		private string GetServerAddress()
		{
			return ConfigurationManager.AppSettings["ServiceAddress"];
		}

		public enum PasswordViewType
		{
			Connect,
			Reconnect,
			Validate
		}
	}
}
