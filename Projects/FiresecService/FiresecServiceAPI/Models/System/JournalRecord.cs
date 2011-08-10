using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class JournalRecord
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
        public StateType StateType { get; set; }
    }
}