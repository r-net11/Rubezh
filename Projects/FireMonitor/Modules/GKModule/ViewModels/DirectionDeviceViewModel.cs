using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DirectionDeviceViewModel : DeviceViewModel
	{
		public DirectionDeviceViewModel(XDevice device)
			: base(device)
		{
		
		}

		public DirectionDeviceViewModel(XDirectionDevice directionDevice):base(directionDevice.Device)
		{
			StateBit = directionDevice.StateBit;
		}
		public XStateBit StateBit { get; set; }
	}
}