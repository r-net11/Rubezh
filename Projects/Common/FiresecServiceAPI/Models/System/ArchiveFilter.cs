using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum SubsystemType
    {
        Fire,
        Guard,
        Other
    };

    [DataContract]
    public class ArchiveFilter
    {
        public ArchiveFilter()
        {
            Descriptions = new List<string>();
            Subsystems = new List<SubsystemType>();
            DeviceDatabaseIds = new List<string>();
        }

        [DataMember]
        public bool UseSystemDate { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public List<string> Descriptions { get; set; }

        [DataMember]
        public List<string> DeviceDatabaseIds { get; set; }

        [DataMember]
        public bool IsDeviceFilterOn { get; set; }

        [DataMember]
        public List<SubsystemType> Subsystems { get; set; }
    }
}