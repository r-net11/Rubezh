using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ResetItem
    {
        public ResetItem()
        {
            StateNames = new List<string>();
        }

        [DataMember]
        public Guid DeviceUID { get; set; }

        [DataMember]
        public List<string> StateNames { get; set; }
    }
}