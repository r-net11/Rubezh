using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Strazh.Common;
using Localization.Strazh.Errors;
using Localization.Strazh.ViewModels;

namespace StrazhModule.ViewModels
{
	public class ControllerPasswordViewModel : SaveCancelDialogViewModel
	{
		private readonly ControllerLocksPasswordViewModel _controllerLocksPasswordViewModel;

		public ControllerPasswordViewModel(ControllerLocksPasswordViewModel controllerLocksPasswordViewModel)
		{
			_controllerLocksPasswordViewModel = controllerLocksPasswordViewModel;

			Title = CommonResources.LockPassword;
			Password = _controllerLocksPasswordViewModel.Password;
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
			if (result)
				_controllerLocksPasswordViewModel.Password = Password;
			else
				MessageBoxService.ShowWarning(CommonErrors.ControllerPassword_Error, null, 350, 160);

			return result;
		}

		private bool Validate()
		{
			return !String.IsNullOrEmpty(Password) && ValidatePasswordContent() && !ValidateIfPasswordAlreadyExists();
		}

		private bool ValidatePasswordContent()
		{
			const string pattern1 = @"^\d{3,6}$"; // Строка из цифр в количестве от 3 до 6 символов
			const string pattern2 = @"^[^0]"; // Строка первая цифра которой не начинается на 0
			var regex = new Regex(pattern1);
			var matches = regex.Matches(Password);
			return matches.Count == 1 && matches[0].Value == Password && new Regex(pattern2).IsMatch(Password);
		}

		private bool ValidateIfPasswordAlreadyExists()
		{
			return _controllerLocksPasswordViewModel.ValidateIfPasswordAlreadyExists(Password);
		}
	}
}
