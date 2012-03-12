using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class GCDriver
    {
        public GCDriver()
        {
            Properties = new List<GCProperty>();
            Children = new List<Guid>();
            AutoCreateChildren = new List<Guid>();
        }

        [DataMember]
        public GCDriverType DriverType { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public Guid OldDriverUID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        [DataMember]
        public string ImageSource { get; set; }

        [DataMember]
        public bool HasImage { get; set; }

        [DataMember]
        public bool CanEditAddress { get; set; }

        [DataMember]
        public bool IsChildAddressReservedRange { get; set; }

        [DataMember]
        public List<GCProperty> Properties { get; set; }

        [DataMember]
        public List<Guid> Children { get; set; }

        [DataMember]
        public List<Guid> AutoCreateChildren { get; set; }
    }
}