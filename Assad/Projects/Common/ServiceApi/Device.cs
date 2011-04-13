using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class Device
    {
        public Device()
        {
            UderlyingZones = new List<string>();
        }

        [IgnoreDataMember]
        public Device Parent { get; set; }

        [DataMember]
        public List<Device> Children { get; set; }

        [DataMember]
        public string DatabaseId { get; set; }

        [DataMember]
        public string DriverId { get; set; }

        [DataMember]
        public string PlaceInTree { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string PresentationAddress { get; set; }

        [DataMember]
        public string ZoneNo { get; set; }

        [DataMember]
        public Firesec.ZoneLogic.expr ZoneLogic { get; set; }

        [DataMember]
        public List<Property> Properties { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<ValidationError> ValidationErrors { get; set; }

        public List<Device> AllParents
        {
            get
            {
                if (Parent == null)
                    return new List<Device>();

                List<Device> allParents = Parent.AllParents;
                allParents.Add(Parent);
                return allParents;
            }
        }

        public List<string> UderlyingZones { get; set; }

        public void AddUnderlyingZone(string zoneNo)
        {
            if (Parent != null)
            {
                if (Parent.UderlyingZones.Contains(zoneNo) == false)
                    Parent.UderlyingZones.Add(zoneNo);
                Parent.AddUnderlyingZone(zoneNo);
            }
        }
    }
}
