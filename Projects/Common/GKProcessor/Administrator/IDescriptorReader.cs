using XFiresecAPI;

namespace GKProcessor
{
	public interface IDescriptorReader
	{
		bool ReadConfiguration(XDevice device);
		XDeviceConfiguration DeviceConfiguration { get; }
		string ParsingError { get; }
	}
}