using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class Schedule
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ScheduleScheme ScheduleScheme { get; set; }
        [DataMember]
        public List<RegisterDevice> RegisterDevices { get; set; }
	}
}
