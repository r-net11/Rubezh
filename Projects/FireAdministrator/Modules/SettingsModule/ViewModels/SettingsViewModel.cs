using RubezhAPI.Models;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public ThemeViewModel ThemeContext { get; set; }

		public SettingsViewModel()
		{
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			ShowErrorsFilterCommand = new RelayCommand(OnShowErrorsFilter);
		}

		public void Initialize()
		{
			ThemeContext = new ThemeViewModel();
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			if (ClientManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
				var gloobalSettingsViewModel = new GlobalSettingsViewModel();
				DialogService.ShowModalWindow(gloobalSettingsViewModel);
			}
			else
			{
				MessageBoxService.ShowError("У Вас нет прав для изменения настроек");
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