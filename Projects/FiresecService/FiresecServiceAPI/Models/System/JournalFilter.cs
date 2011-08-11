using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class JournalFilter
    {
        public static readonly int MaxRecordsCount = 100;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int LastRecordsCount { get; set; }

        [DataMember]
        public int LastDaysCount { get; set; }

        [DataMember]
        public bool IsLastDaysCountActive { get; set; }

        [DataMember]
        public List<State> Events { get; set; }

        [DataMember]
        public List<DeviceCategory> Categories { get; set; }
    }
}