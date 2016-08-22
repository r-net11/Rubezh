using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;
using Localization.IntegrationOPC.Common;
using Localization.IntegrationOPC.ViewModels;

namespace Integration.OPC.ViewModels
{
	public class SettingsViewModel : SaveCancelDialogViewModel
	{
		public OPCSettings Settings { get; set; }

		public RelayCommand PingCommand { get; set; }

		private readonly bool _isActiveNow;

		public SettingsViewModel(bool isActiveNow)
		{
			Title = CommonResources.Settings;
			_isActiveNow = isActiveNow;
			PingCommand = new RelayCommand(OnPing);
		}

		public void OnPing()
		{
			if (_isActiveNow)
			{
				var result = FiresecManager.FiresecService.PingOPCServer();

				if (result.Result)
					MessageBoxService.ShowExtended(CommonViewModels.MessagePingSuccessfulContent);
				else
                    MessageBoxService.ShowWarning(CommonViewModels.MessagePingTimeoutContent);
			}
			else
                MessageBoxService.ShowWarning(CommonViewModels.MessagePingWithNotActiveServerContent);
		}

		protected override bool CanSave()
		{
			return Settings.IsValid;
		}

	}
}
