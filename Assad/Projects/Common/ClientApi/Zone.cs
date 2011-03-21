using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;

namespace ClientApi
{
    public class Zone
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DetectorCount { get; set; }
        public string EvacuationTime { get; set; }
        public string Description { get; set; }
        public List<Device> Devices { get; set; }
        public string ValidationError { get; set; }
        public string State { get; set; }

        public void SetConfig(ShortZone shortZone)
        {
            this.Name = shortZone.Name;
            this.Id = shortZone.Id;
            this.DetectorCount = shortZone.DetectorCount;
            this.EvacuationTime = shortZone.EvacuationTime;
            this.Description = shortZone.Description;
        }

        public void SetState(ShortZoneState shortZoneState)
        {
            this.State = shortZoneState.State;
        }
    }
}
