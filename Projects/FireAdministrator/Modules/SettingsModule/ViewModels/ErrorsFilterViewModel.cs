using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class ErrorsFilterViewModel : SaveCancelDialogViewModel
	{
		public ErrorsFilterViewModel()
		{
			Title = "Настройка фильтра ошибок";
			ErrorFilters = new ObservableCollection<ErrorFilterViewModel>();
			if (GlobalSettingsHelper.GlobalSettings.IgnoredErrors == null)
				GlobalSettingsHelper.GlobalSettings.IgnoredErrors = new List<string>();
			
			IsLogicAllowed = GlobalSettingsHelper.GlobalSettings.IsLogicAllowed;

			ErrorFilters.Add(new ErrorFilterViewModel("Устройство не подключено к зоне"));
			ErrorFilters.Add(new ErrorFilterViewModel("Отсутствует логика срабатывания исполнительного устройства"));
			ErrorFilters.Add(new ErrorFilterViewModel("Устройство должно содержать подключенные устройства"));
			ErrorFilters.Add(new ErrorFilterViewModel("В направлении отсутствуют входные устройства или зоны"));
			ErrorFilters.Add(new ErrorFilterViewModel("В направлении отсутствуют выходные устройства"));
			ErrorFilters.Add(new ErrorFilterViewModel("Количество подключенных к зоне датчиков"));
			
			foreach (var ignoredError in GlobalSettingsHelper.GlobalSettings.IgnoredErrors)
			{
				var errorFilter = ErrorFilters.FirstOrDefault(x => x.Name == ignoredError);
				if (errorFilter != null)
				{
					errorFilter.IsChecked = true;
				}
			}
		}

		public ObservableCollection<ErrorFilterViewModel> ErrorFilters { get; private set; }

		protected override bool Save()
		{
			GlobalSettingsHelper.GlobalSettings.IgnoredErrors = new List<string>();
			foreach (var errorFilter in ErrorFilters)
			{
				if (errorFilter.IsChecked)
				{
					GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Add(errorFilter.Name);
				}
			}
			GlobalSettingsHelper.GlobalSettings.IsLogicAllowed = IsLogicAllowed;
			GlobalSettingsHelper.Save();
			return true;
		}

		bool isLogicAllowed;
		public bool IsLogicAllowed
		{
			get { return isLogicAllowed; }
			set
			{
				isLogicAllowed = value;
				OnPropertyChanged(() => IsLogicAllowed);
			}
		}
	}
}