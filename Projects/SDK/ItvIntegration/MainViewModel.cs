using FiresecClient.Itv;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace ItvIntegration
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }
		public JournalsViewModel JournalsViewModel { get; private set; }

		public MainViewModel()
		{
			ShowImitatorCommand = new RelayCommand(OnShowImitator);
			var message = ItvManager.Connect("net.pipe://127.0.0.1/FiresecService/", GlobalSettingsHelper.GlobalSettings.Login, GlobalSettingsHelper.GlobalSettings.Password);
			if (message != null)
			{
				MessageBoxService.Show(message);
				return;
			}

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			JournalsViewModel = new JournalsViewModel();
		}

		public RelayCommand ShowImitatorCommand { get; private set; }
		void OnShowImitator()
		{
			ItvManager.ShowImitator();
		}
	}
}