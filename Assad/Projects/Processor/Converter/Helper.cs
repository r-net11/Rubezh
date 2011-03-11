using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using ServiceApi;
using ClientApi;

namespace Processor
{
    public class Helper
    {
        internal static AssadBase ConvertDevice(Device device)
        {
            try
            {
                return AssadConfiguration.Devices.FirstOrDefault(x => x.Path == device.Path);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static Device ConvertDevice(AssadBase assadDevice)
        {
            try
            {
                return ServiceClient.Configuration.Devices.FirstOrDefault(x => x.Path == assadDevice.Path);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
