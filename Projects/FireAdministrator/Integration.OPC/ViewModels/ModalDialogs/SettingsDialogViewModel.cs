using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;
using Integration.OPC.Properties;

namespace Integration.OPC.ViewModels
{
	public class SettingsViewModel : SaveCancelDialogViewModel
	{
		public OPCSettings Settings { get; set; }

		public RelayCommand PingCommand { get; set; }

		private readonly bool _isActiveNow;

		public SettingsViewModel(bool isActiveNow)
		{
			Title = "Настройки";
			_isActiveNow = isActiveNow;
			PingCommand = new RelayCommand(OnPing);
		}

		public void OnPing()
		{
			//TODO: Ping connection with httpIntegrationClient
			if (!_isActiveNow)
				MessageBoxService.ShowWarning(Resources.MessagePingWithNotActiveServerContent);

			if (true)
			{
				MessageBoxService.ShowExtended(Resources.MessagePingSuccessfulContent);
			}
			else
			{
				MessageBoxService.ShowWarning(Resources.MessagePingTimeoutContent);
			}
		}

		protected override bool CanSave()
		{
			return Settings.IsValid;
		}

	}
}
