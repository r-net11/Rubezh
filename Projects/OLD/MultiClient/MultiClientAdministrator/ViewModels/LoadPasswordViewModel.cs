using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows;
using MuliclientAPI;
using Infrastructure.Common.Windows.Windows;

namespace MultiClientAdministrator.ViewModels
{
	public class LoadPasswordViewModel : DialogViewModel
	{
		public LoadPasswordViewModel()
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
			if (!string.IsNullOrEmpty(Password))
			{
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
}