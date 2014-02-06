using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class RegisterDevice
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public bool CanControl { get; set; }
	}
}
