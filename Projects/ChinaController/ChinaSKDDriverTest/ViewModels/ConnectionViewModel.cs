using StrazhDeviceSDK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class ConnectionViewModel : BaseViewModel
	{
		private int _loginId;

		private string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				if (_address == value)
					return;
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		private int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				if (_port == value)
					return;
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		private string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				if (_login == value)
					return;
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				if (_password == value)
					return;
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		public RelayCommand ConnectCommand { get; private set; }
		private void OnConnect()
		{
			Wrapper.Initialize();
			string error;
			_loginId = MainViewModel.Wrapper.Connect(Address, Port, Login, Password, out error);
		}
		private bool CanConnect()
		{
			return _loginId == 0;
		}

		public RelayCommand DisconnectCommand { get; private set; }
		private void OnDisconnect()
		{
			MainViewModel.Wrapper.Disconnect();
			Wrapper.Deinitialize();
			_loginId = 0;
		}
		private bool CanDisconnect()
		{
			return _loginId != 0;
		}

		public ConnectionViewModel()
		{
			var connectionSettings = ConnectionSettingsHelper.Get();
			Address = connectionSettings.Address;
			Port = connectionSettings.Port;
			Login = connectionSettings.Login;
			Password = connectionSettings.Password;

			ConnectCommand = new RelayCommand(OnConnect, CanConnect);
			DisconnectCommand = new RelayCommand(OnDisconnect, CanDisconnect);
		}
	}
}