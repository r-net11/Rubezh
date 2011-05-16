using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using AssadProcessor.Devices;

namespace AssadProcessor
{
    public class Helper
    {
        internal static Device ConvertDevice(AssadDevice assadDevice)
        {
            try
            {
                return FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == assadDevice.Id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
