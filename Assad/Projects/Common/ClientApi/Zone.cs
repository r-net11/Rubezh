using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ClientApi
{
    public class Zone
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string DetectorCount { get; set; }
        public string EvacuationTime { get; set; }
        public string Description { get; set; }
        public List<ValidationError> ValidationErrors { get; set; }
    }
}
