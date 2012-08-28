using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            StateTypes = new List<StateType>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int LastRecordsCount { get; set; }

        [DataMember]
        public List<StateType> StateTypes { get; set; }
    }
}