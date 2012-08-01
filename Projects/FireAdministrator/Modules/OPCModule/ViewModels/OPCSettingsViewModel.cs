using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace OPCModule.ViewModels
{
	public class OPCSettingsViewModel : ViewPartViewModel
	{
		public OPCSettingsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			RegisterCommand = new RelayCommand(OnRegister);
			UnRegisterCommand = new RelayCommand(OnUnRegister);
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			FiresecManager.FiresecService.OPCRefresh();
		}

		public RelayCommand RegisterCommand { get; private set; }
		void OnRegister()
		{
			FiresecManager.FiresecService.OPCRegister();
		}

		public RelayCommand UnRegisterCommand { get; private set; }
		void OnUnRegister()
		{
			FiresecManager.FiresecService.OPCUnRegister();
		}
	}
}