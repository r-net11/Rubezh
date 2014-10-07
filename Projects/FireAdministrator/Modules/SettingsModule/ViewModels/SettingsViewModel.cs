using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public ThemeViewModel ThemeContext { get; set; }

		public void Initialize()
		{
			ThemeContext = new ThemeViewModel();
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			ShowErrorsFilterCommand = new RelayCommand(OnShowErrorsFilter);
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			if (FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
				var gloobalSettingsViewModel = new GloobalSettingsViewModel();
				DialogService.ShowModalWindow(gloobalSettingsViewModel);
			}
			else
			{
				MessageBoxService.ShowErrorExtended("У Вас нет прав для изменения настроек");
			}
		}

		public RelayCommand ShowErrorsFilterCommand { get; private set; }
		void OnShowErrorsFilter()
		{
			var errorsFilterViewModel = new ErrorsFilterViewModel();
			DialogService.ShowModalWindow(errorsFilterViewModel);
		}
	}
}