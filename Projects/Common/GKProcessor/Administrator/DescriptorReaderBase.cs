using FiresecAPI.GK;

namespace GKProcessor
{
	public abstract class DescriptorReaderBase
	{
		public abstract bool ReadConfiguration(GKDevice device);
		public GKDeviceConfiguration DeviceConfiguration { get; protected set; }
		public string Error { get; protected set; }
	}
}