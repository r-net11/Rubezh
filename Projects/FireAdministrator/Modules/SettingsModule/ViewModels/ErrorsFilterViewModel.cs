using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class ErrorsFilterViewModel : SaveCancelDialogViewModel
	{
		public ErrorsFilterViewModel()
		{
			Title = "Настройка фильтра ошибок";
			ErrorFilters = new ObservableCollection<ErrorFilterViewModel>(Enum.GetValues(typeof(ValidationErrorType)).Cast<ValidationErrorType>().Select(item => new ErrorFilterViewModel(item)));
		}

		public ObservableCollection<ErrorFilterViewModel> ErrorFilters { get; private set; }

		protected override bool Save()
		{
			GlobalSettingsHelper.GlobalSettings.IgnoredErrors = 0;
			foreach (var errorFilter in ErrorFilters)
				if (errorFilter.IsChecked)
					GlobalSettingsHelper.GlobalSettings.IgnoredErrors |= errorFilter.ErrorType;
			GlobalSettingsHelper.Save();
			return true;
		}
	}
}