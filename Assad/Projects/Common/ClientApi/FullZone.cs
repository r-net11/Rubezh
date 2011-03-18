using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;

namespace ClientApi
{
    public class FullZone
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string DetectorCount { get; set; }
        public string EvacuationTime { get; set; }
        public string Description { get; set; }
        public string ValidationError { get; set; }
        public List<FullDevice> Devices { get; set; }
        public string State { get; set; }

        public FullZone(ShortZone shortZone)
        {
            this.Name = shortZone.Name;
            this.Id = shortZone.Id;
            this.DetectorCount = shortZone.DetectorCount;
            this.EvacuationTime = shortZone.EvacuationTime;
            this.Description = shortZone.Description;
        }

        public FullZone()
        {
        }
    }
}
