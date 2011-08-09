using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ParentDeviceState
    {
        public Device ParentDevice { get; set; }
        public DriverState DriverState { get; set; }

        [DataMember]
        public string ParentDeviceId { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}
