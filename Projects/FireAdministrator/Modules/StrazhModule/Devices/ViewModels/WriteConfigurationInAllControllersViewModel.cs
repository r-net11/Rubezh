using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Strazh.Common;

namespace StrazhModule.ViewModels
{
	class WriteConfigurationInAllControllersViewModel : SaveCancelDialogViewModel
	{
		public WriteConfigurationInAllControllersViewModel()
		{
			Title = CommonResources.WriteConfigAllControllers;

			IsTimeSchedules = true;
			IsLockPasswords = true;
			IsCards = true;
		}

		private bool _isTimeSchedules;
		public bool IsTimeSchedules
		{
			get { return _isTimeSchedules; }
			set
			{
				if (_isTimeSchedules == value)
					return;
				_isTimeSchedules = value;
				OnPropertyChanged(() => IsTimeSchedules);
			}
		}

		private bool _isLockPasswords;
		public bool IsLockPasswords {
			get { return _isLockPasswords; }
			set
			{
				if (_isLockPasswords == value)
					return;
				_isLockPasswords = value;
				OnPropertyChanged(() => IsLockPasswords);
			}
		}

		private bool _isCards;
		public bool IsCards {
			get { return _isCards; }
			set
			{
				if (_isCards == value)
					return;
				_isCards = value;
				OnPropertyChanged(() => IsCards);
			}
		}
	}
}
