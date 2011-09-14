using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace AssadProcessor
{
    public static class ConfigurationHelper
    {
        public static DeviceState GetDeviceState(string id)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
            if (device != null)
            {
                var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
                return deviceState;
            }
            return null;
        }
    }
}
