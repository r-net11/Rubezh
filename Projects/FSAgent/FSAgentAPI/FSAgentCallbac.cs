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
        public string CoreCongig { get; set; }

        [DataMember]
		public string CoreDeviceParams { get; set; }

		[DataMember]
		public FSProgressInfo FSProgressInfo { get; set; }
    }
}