using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Zone
    {
        public Zone()
        {
            ZoneType = ZoneType.Fire;
            GuardZoneType = GuardZoneType.Ordinary;
            DetectorCount = "2";
            EvacuationTime = "0";
            AutoSet = "0";
            Delay = "0";
        }

        [DataMember]
        public string No { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public ZoneType ZoneType { get; set; }

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
        public GuardZoneType GuardZoneType { get; set; }

        public string PresentationName
        {
            get { return No + "." + Name; }
        }
    }
}