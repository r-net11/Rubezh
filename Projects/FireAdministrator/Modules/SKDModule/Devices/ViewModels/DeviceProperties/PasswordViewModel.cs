using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class PasswordViewModel : DialogViewModel
	{
		public SKDDevice Device { get; private set; }

		public PasswordViewModel(SKDDevice device)
		{
			Title = "Изменение пароля контроллера";
			Device = device;
			GetPasswordCommand = new RelayCommand(OnGetPassword);
			SetPasswordCommand = new RelayCommand(OnSetPassword);
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		public RelayCommand GetPasswordCommand { get; private set; }
		void OnGetPassword()
		{
			var result = FiresecManager.FiresecService.SKDGetPassword(Device.UID);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				Password = result.Result;
			}
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		void OnSetPassword()
		{
			var result = FiresecManager.FiresecService.SKDSetPassword(Device.UID, Password);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
		}
	}
}