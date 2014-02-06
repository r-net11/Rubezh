using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class ScheduleScheme
	{
        [DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ScheduleSchemeType Type { get; set; }
        [DataMember]
        public List<Day> Days { get; set; }
	}

    [DataContract]
    public enum ScheduleSchemeType
	{
		Week,
		Shift,
		Month
	}
}
