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
        public string DeviceId { get; set; }

        [DataMember]
        public List<string> StateNames { get; set; }
    }
}