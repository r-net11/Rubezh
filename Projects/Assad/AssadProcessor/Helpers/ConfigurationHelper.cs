using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace AssadProcessor
{
	public static class ConfigurationHelper
	{
		public static DeviceState GetDeviceState(string id)
		{
			var device = FiresecManager.Devices.FirstOrDefault(x => x.PathId == id);
			if (device != null)
			{
				return device.DeviceState;
			}
			return null;
		}
	}
}