using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServiceApi
{
    [DataContract]
    public class ShortDevice
    {
        [IgnoreDataMember]
        public ShortDevice Parent { get; set; }

        [DataMember]
        public List<ShortDevice> Children { get; set; }

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
        public List<DeviceProperty> DeviceProperties { get; set; }

        [DataMember]
        public string Description { get; set; }


        //[DataMember]
        //public List<Parameter> Parameters { get; set; }

        //[DataMember]
        //public List<string> AvailableFunctions { get; set; }

        [DataMember]
        public string ValidationError { get; set; }
    }
}
