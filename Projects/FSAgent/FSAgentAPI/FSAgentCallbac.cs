using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FSAgentAPI
{
    [DataContract]
    public class FSAgentCallbac
    {
        [DataMember]
        public List<JournalRecord> JournalRecords { get; set; }

        [DataMember]
        public List<DeviceState> DeviceStates { get; set; }

        [DataMember]
        public List<DeviceState> DeviceParameters { get; set; }

        [DataMember]
        public List<ZoneState> ZoneStates { get; set; }
    }
}