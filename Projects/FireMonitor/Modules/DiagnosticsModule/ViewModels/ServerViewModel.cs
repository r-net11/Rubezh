using System.Collections.ObjectModel;
using System.Linq;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class ServerViewModel : ViewPartViewModel
	{
		public ServerViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			OnRefresh();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
		}
	}
}