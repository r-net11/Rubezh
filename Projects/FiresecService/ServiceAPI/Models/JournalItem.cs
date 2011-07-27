using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FiresecClient.Models;

namespace ServiceAPI.Models
{
    [DataContract]
    public class JournalItem
    {
        [DataMember]
        public int No { get; set; }

        [DataMember]
        public DateTime DeviceTime { get; set; }

        [DataMember]
        public DateTime SystemTime { get; set; }

        [DataMember]
        public string ZoneName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string DeviceName { get; set; }

        [DataMember]
        public string PanelName { get; set; }

        [DataMember]
        public string DeviceDatabaseId { get; set; }

        [DataMember]
        public string PanelDatabaseId { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public State State { get; set; }
    }
}
