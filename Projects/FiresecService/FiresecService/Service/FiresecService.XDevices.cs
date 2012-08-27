using FiresecService.Configuration;
using XFiresecAPI;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public void SetXDeviceConfiguration(XDeviceConfiguration xDeviceConfiguration)
		{
			ConfigurationFileManager.SetXDeviceConfiguration(xDeviceConfiguration);
		}

		public XDeviceConfiguration GetXDeviceConfiguration()
		{
			return ConfigurationFileManager.GetXDeviceConfiguration();
		}
	}
}