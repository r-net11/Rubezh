using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class GCDriversConfiguration
    {
        public GCDriversConfiguration()
        {
            Drivers = new List<GCDriver>();
        }

        [DataMember]
        public List<GCDriver> Drivers { get; set; }
    }
}