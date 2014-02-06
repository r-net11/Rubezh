using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
