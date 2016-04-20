using FiresecAPI.Models;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class EmailSettingsViewModel : BaseViewModel
	{
		public EmailSettingsViewModel()
		{
			EmailSettings = new EmailSettings();
		}

		public EmailSettingsViewModel(EmailSettings emailSettings)
		{
			EmailSettings = emailSettings;
		}

		EmailSettings _emailSettings;

		public EmailSettings EmailSettings
		{
			get { return _emailSettings; }
			set
			{
				_emailSettings = value;
				OnPropertyChanged(() => EmailSettings);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => EmailSettings);
		}
	}
}