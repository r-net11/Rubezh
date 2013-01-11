using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace FireMonitor.Multiclient.ViewModels
{
	public class PasswordViewModel : DialogViewModel
	{
		public PasswordViewModel()
		{
			Title = "Ввод пароля";
			SaveCommand = new RelayCommand(OnSave);
		}

		string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged("Password");
			}
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			Close(true);
		}
	}
}