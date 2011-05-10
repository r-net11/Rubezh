using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using FiresecClient;

namespace AssadProcessor
{
    public class Helper
    {
        internal static Device ConvertDevice(AssadBase assadDevice)
        {
            try
            {
                return FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == assadDevice.Path);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
