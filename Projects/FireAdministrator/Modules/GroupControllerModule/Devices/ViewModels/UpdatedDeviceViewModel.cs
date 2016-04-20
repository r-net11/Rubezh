using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class UpdatableDeviceViewModel : BaseViewModel
	{
		public string Name { get; private set; }
		public string Address { get; private set; }
		public GKDevice Device;
		public string ImageSource { get; private set; }
		public UpdatableDeviceViewModel(GKDevice device)
		{
			Device = device;
			Name = device.ShortName;
			Address = device.DottedPresentationAddress;
			ImageSource = device.Driver.ImageSource;
		}
	}
}