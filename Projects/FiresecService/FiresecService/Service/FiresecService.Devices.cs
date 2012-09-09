using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Configuration;

namespace FiresecService.Service
{
    public partial class FiresecService
    {
        public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
            OperationResult<bool> result = new OperationResult<bool>();
            var thread = new Thread(new ThreadStart(() => { ClientsCash.OnConfigurationChanged(); }));
            thread.Start();
            return result;
        }
    }
}