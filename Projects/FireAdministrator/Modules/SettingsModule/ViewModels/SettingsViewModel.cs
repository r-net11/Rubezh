using Localization.Settings.Errors;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
	//	public ThemeViewModel ThemeContext { get; set; }

		public SettingsViewModel()
		{
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
		}

		public void Initialize()
		{
		//	ThemeContext = new ThemeViewModel();
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
				MessageBoxService.ShowError(CommonErrors.Settings_Error);
			}
		}
	}
}