using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FSAgentAPI
{
    [DataContract]
    public class FSAgentCallbac
    {
		public FSAgentCallbac()
		{
			JournalRecords = new List<JournalRecord>();
		}

        [DataMember]
        public List<JournalRecord> JournalRecords { get; set; }

        [DataMember]
        public string CoreCongig { get; set; }

        [DataMember]
		public string CoreDeviceParams { get; set; }

		[DataMember]
		public FSProgressInfo FSProgressInfo { get; set; }

		[DataMember]
		public bool IsConnectionLost { get; set; }
    }
}