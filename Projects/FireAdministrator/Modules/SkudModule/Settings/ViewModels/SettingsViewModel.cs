using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public SettingsViewModel()
		{
			ShowVerificationSettingsCommand = new RelayCommand(OnShowVerificationSettings);
		}

		public RelayCommand ShowVerificationSettingsCommand { get; private set; }
		void OnShowVerificationSettings()
		{
			var verificationDetailsViewModel = new VerificationDetailsViewModel();
			if(DialogService.ShowModalWindow(verificationDetailsViewModel))
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}