using XFiresecAPI;

namespace GKModule.Converter
{
	public class GKBinaryObject : BinaryObjectBase
	{
		XDevice Device;

		public GKBinaryObject(XDevice device)
		{
			IsGk = true;
			Device = device;

			DeviceType = ToBytes(device.Driver.DriverTypeNo);
		}
	}
}