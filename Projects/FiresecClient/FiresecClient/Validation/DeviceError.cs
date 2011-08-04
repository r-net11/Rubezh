using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI;

namespace FiresecClient.Validation
{
    public class DeviceError : BaseError
    {
        public DeviceError(Device device, string error, ErrorLevel level)
            : base(error, level)
        {
            Device = device;
        }

        public Device Device { get; set; }
    }
}
