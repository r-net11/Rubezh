using XFiresecAPI;

namespace GKProcessor
{
	public abstract class DescriptorReaderBase
	{
		public abstract bool ReadConfiguration(XDevice device);
		public XDeviceConfiguration DeviceConfiguration { get; protected set; }
		public string Error { get; protected set; }
	}
}