using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class SimulationDeviceViewModel : BaseViewModel
	{
		public SimulationDeviceViewModel(Device device)
		{
			Device = device;
		}

		public Device Device { get; private set; }
	}
}