using System.Reflection;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.MessageBox;

namespace FireAdministrator.ViewModels
{
	public class LoginViewModel : DialogContent
	{
		public LoginViewModel()
		{
			Title = "Администратор. Авторизация";
			ConnectCommand = new RelayCommand(OnConnect);
			CancelCommand = new RelayCommand(OnCancel);

			UserName = ServiceFactory.AppSettings.UserName;
			Password = ServiceFactory.AppSettings.SavePassword ? ServiceFactory.AppSettings.Password : string.Empty;
			SavePassword = ServiceFactory.AppSettings.SavePassword;
			IsConnected = false;
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

		public RelayCommand ConnectCommand { get; private set; }
		void OnConnect()
		{
			Close(true);
			DoConnect(ServiceFactory.AppSettings.ServiceAddress, UserName, Password);
		}

		void OnCancel()
		{
			Close(false);
		}

		void DoConnect(string serverAddress, string userName, string password)
		{
			Surface.Hide();
			string message = null;
			using (new WaitWrapper())
			{
				var preLoadWindow = new PreLoadWindow()
				{
					PreLoadText = "Соединение с сервером..."
				};
				preLoadWindow.Show();
				message = FiresecManager.Connect("Администратор", serverAddress, userName, password);
				preLoadWindow.Close();
			}
			IsConnected = string.IsNullOrEmpty(message);
			if (IsConnected)
			{
				ServiceFactory.AppSettings.UserName = UserName;
				ServiceFactory.AppSettings.Password = SavePassword ? password : string.Empty;
				ServiceFactory.AppSettings.SavePassword = SavePassword;
				AppSettingsHelper.SaveAppSettings();
			}
			else
				MessageBoxService.Show(message);
		}
	}
}