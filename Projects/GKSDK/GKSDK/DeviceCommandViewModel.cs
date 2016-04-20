using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKSDK
{
	public class DeviceCommandViewModel : BaseViewModel
	{
		GKDevice Device;

        public DeviceCommandViewModel(GKDevice device)
		{
			ExecuteCommand = new RelayCommand(OnExecute);
			Device = device;
		}

		public string ConmmandName { get; private set; }

		public RelayCommand ExecuteCommand { get; private set; }
		void OnExecute()
		{
		}
	}
}