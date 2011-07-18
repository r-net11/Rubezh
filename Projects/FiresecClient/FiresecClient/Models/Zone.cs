using System.Collections.Generic;

namespace FiresecClient.Models
{
    public class Zone
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ZoneType { get; set; }
        public string DetectorCount { get; set; }
        public string EvacuationTime { get; set; }

        public string AutoSet { get; set; }
        public string Delay { get; set; }
        public bool Skipped { get; set; }
        public string GuardZoneType { get; set; }

        public string PresentationName
        {
            get { return No + "." + Name; }
        }
        public List<ValidationError> ValidationErrors { get; set; }
    }
}
