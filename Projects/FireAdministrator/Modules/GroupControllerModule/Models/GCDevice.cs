using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class GCDevice
    {
        public GCDevice()
        {
            Children = new List<GCDevice>();
            Properties = new List<GCProperty>();
        }

        public GCDriver Driver { get; set; }
        public GCDevice Parent { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public Guid DriverUID { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<GCDevice> Children { get; set; }

        [DataMember]
        public List<GCProperty> Properties { get; set; }
    }
}