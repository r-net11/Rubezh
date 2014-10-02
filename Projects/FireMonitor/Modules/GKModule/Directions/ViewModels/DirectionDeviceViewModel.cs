using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class DirectionDeviceViewModel : DeviceViewModel
	{
		public DirectionDeviceViewModel(GKDevice device)
			: base(device)
		{
		
		}

		public DirectionDeviceViewModel(GKDirectionDevice directionDevice):base(directionDevice.Device)
		{
			StateBit = directionDevice.StateBit;
		}
		public GKStateBit StateBit { get; set; }
	}
}