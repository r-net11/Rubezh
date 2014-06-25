using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class PasswordViewModel : DialogViewModel
	{
		public SKDDevice Device { get; private set; }

		public PasswordViewModel(SKDDevice device)
		{
			Device = device;
			Title = "Изменение пароля контроллера";
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

		}

		public RelayCommand SetPasswordCommand { get; private set; }
		void OnSetPassword()
		{

		}
	}
}