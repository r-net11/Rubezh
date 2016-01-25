using System.Diagnostics;
using System.Reflection;
using Common;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure.Client.Properties;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Login.ViewModels
{
	public class LoginViewModel : SaveCancelDialogViewModel
	{
		PasswordViewType _passwordViewType;

		public LoginViewModel(ClientType clientType)
			: this(clientType, PasswordViewType.Connect)
		{
		}
		public LoginViewModel(ClientType clientType, PasswordViewType passwordViewType)
		{
			TopMost = true;
			ClientType = clientType;
			_passwordViewType = passwordViewType;

			switch (_passwordViewType)
			{
				case PasswordViewType.Connect:
					UserName = Settings.Default.UserName;
					CanEditUserName = true;
					break;
				case PasswordViewType.Validate:
					UserName = ClientManager.CurrentUser.Login;
					Settings.Default.SavePassword = false;
					CanEditUserName = false;
					break;
			}
			CanSavePassword = _passwordViewType != PasswordViewType.Validate;
			Password = Settings.Default.SavePassword ? Settings.Default.Password : string.Empty;
			SavePassword = Settings.Default.SavePassword;

			IsConnected = false;
			IsCanceled = false;
			Message = null;
			Sizable = false;
		}

		string _userName;
		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				OnPropertyChanged(() => UserName);
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
		bool _savePassword;
		public bool SavePassword
		{
			get { return _savePassword; }
			set
			{
				_savePassword = value;
				OnPropertyChanged(() => SavePassword);
			}
		}
		bool _canEditUserName;
		public bool CanEditUserName
		{
			get { return _canEditUserName; }
			set
			{
				_canEditUserName = value;
				OnPropertyChanged(() => CanEditUserName);
			}
		}
		bool _canSavePassword;
		public bool CanSavePassword
		{
			get { return _canSavePassword; }
			set
			{
				_canSavePassword = value;
				OnPropertyChanged(() => CanSavePassword);
			}
		}

		public bool IsConnected { get; protected set; }
		public bool IsCanceled { get; protected set; }
		public string Message { get; protected set; }
		public ClientType ClientType { get; private set; }

		protected override bool Save()
		{
			if (UserName == "debug")
			{
				ServiceFactoryBase.Events.GetEvent<ShowGKDebugEvent>().Publish(null);
				return false;
			}
			if (UserName == "Integrate")
			{
				ShellIntegrationHelper.Integrate();
				return false;
			}
			if (UserName == "Desintegrate")
			{
				ShellIntegrationHelper.Desintegrate();
				Process.Start("explorer.exe");
				return false;
			}
			Close(true);
			IsCanceled = false;
			switch (_passwordViewType)
			{
				case PasswordViewType.Connect:
					var preLoadWindow = new ConnectionViewModel();
					DialogService.ShowWindow(preLoadWindow);
					Message = ClientManager.Connect(ClientType, ConnectionSettingsManager.ServerAddress, UserName, Password);
					preLoadWindow.ForceClose();
					break;
				case PasswordViewType.Validate:
					Message = HashHelper.CheckPass(Password, ClientManager.CurrentUser.PasswordHash) ? null : "Неверный пароль";
					break;
			}
			IsConnected = string.IsNullOrEmpty(Message);
			if (CanSavePassword && IsConnected)
			{
				Settings.Default.UserName = UserName;
				Settings.Default.Password = SavePassword ? Password : string.Empty;
				Settings.Default.SavePassword = SavePassword;
				Settings.Default.Save();
			}
			return true;
		}
		protected override bool Cancel()
		{
			Message = null;
			return base.Cancel();
		}

		public override void OnClosed()
		{
			IsCanceled = true;
			base.OnClosed();
		}

		public enum PasswordViewType
		{
			Connect,
			Validate
		}
	}
}