using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Common;
using System.Collections.Generic;

namespace SettingsModule.ViewModels
{
	public class ErrorsFilterViewModel : SaveCancelDialogViewModel
	{
		public ErrorsFilterViewModel()
		{
			Title = "Настройка фильтра ошибок";
			ErrorFilters = new ObservableCollection<ErrorFilterViewModel>(Enum.GetValues(typeof(ValidationErrorType)).Cast<ValidationErrorType>().Select(item => new ErrorFilterViewModel(item) { IsChecked = GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(item)}));
		}

		public ObservableCollection<ErrorFilterViewModel> ErrorFilters { get; private set; }

		protected override bool Save()
		{
			GlobalSettingsHelper.GlobalSettings.IgnoredErrors = new List<ValidationErrorType>();
			ErrorFilters.ForEach(x=> 
			{
				if (x.IsChecked)
					GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Add(x.ErrorType);
			});
			GlobalSettingsHelper.Save();
			return true;
		}
	}
}