using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Zone
    {
        [DataMember]
        public string No { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ZoneType { get; set; }

        [DataMember]
        public string DetectorCount { get; set; }

        [DataMember]
        public string EvacuationTime { get; set; }

        [DataMember]
        public string AutoSet { get; set; }

        [DataMember]
        public string Delay { get; set; }

        [DataMember]
        public bool Skipped { get; set; }

        [DataMember]
        public string GuardZoneType { get; set; }

        public string PresentationName
        {
            get { return No + "." + Name; }
        }
    }
}
