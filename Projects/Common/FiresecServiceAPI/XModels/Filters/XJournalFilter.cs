using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI;

namespace XFiresecAPI
{
    [DataContract]
    public class XJournalFilter
    {
        public XJournalFilter()
        {
            LastRecordsCount = 100;
            StateClasses = new List<XStateClass>();
            EventNames = new List<string>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int LastRecordsCount { get; set; }

        [DataMember]
        public List<XStateClass> StateClasses { get; set; }

        [DataMember]
        public List<string> EventNames { get; set; }
    }
}