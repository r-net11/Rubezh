using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class ControllerLockPasswordViewModel : SaveCancelDialogViewModel
	{
		private ControllerLocksPasswordsViewModel _controllerLocksPasswordsViewModel;

		public ControllerLockPasswordViewModel(ControllerLocksPasswordsViewModel controllerLocksPasswordsViewModel)
		{
			_controllerLocksPasswordsViewModel = controllerLocksPasswordsViewModel;

			Title = "Пароль замка";
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

		protected override bool Save()
		{
			var result = Validate();
			if (!result)
				MessageBoxService.ShowWarning("Пароль замка должен соответствовать следующим ограничениям:\n\nНе может быть пустым\nКоличество знаков от 3 до 9\nНе должен начинаться на 0\nНе должен совпадать с уже имеющимся паролем");
			return result;
		}

		private bool Validate()
		{
			return !String.IsNullOrEmpty(Password) && ValidatePasswordContent() && !ValidatePasswordAlreadyExists();
		}

		private bool ValidatePasswordContent()
		{
			const string pattern1 = @"^\d{3,9}$"; // Строка из цифр в количестве от 3 до 9 символов
			const string pattern2 = @"^[^0]"; // Строка первая цифра которой не начинается на 0
			var regex = new Regex(pattern1);
			var matches = regex.Matches(Password);
			return matches.Count == 1 && matches[0].Value == Password && new Regex(pattern2).IsMatch(Password);
		}

		private bool ValidatePasswordAlreadyExists()
		{
			return false;
		}
	}
}
