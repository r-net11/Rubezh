using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public SettingsViewModel()
		{
			ShowSettingsCommand = new RelayCommand(OnShowVerificationSettings);
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowVerificationSettings()
		{
			var settingsDetailsViewModel = new SettingsDetailsViewModel();
			if(DialogService.ShowModalWindow(settingsDetailsViewModel))
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}