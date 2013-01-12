using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using MuliclientAPI;
using Infrastructure.Common.Windows;

namespace MultiClientAdministrator.ViewModels
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

		public MulticlientConfiguration MulticlientConfiguration { get; private set; }

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			if (string.IsNullOrEmpty(Password))
			{
				return;
			}
			MulticlientConfiguration = MulticlientConfigurationHelper.LoadConfiguration(Password);
			if (MulticlientConfiguration == null)
			{
				MessageBoxService.ShowError("Ошибка. Неверный пароль");
				return;
			}

			Close(true);
		}
	}
}