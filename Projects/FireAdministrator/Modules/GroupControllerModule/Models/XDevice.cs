using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XDevice
    {
        public XDevice()
        {
            Children = new List<XDevice>();
            Properties = new List<XProperty>();
        }

        public XDriver Driver { get; set; }
        public XDevice Parent { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public Guid DriverUID { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public int IntAddress { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<XDevice> Children { get; set; }

        [DataMember]
        public List<XProperty> Properties { get; set; }

        public int GetReservedCount()
        {
            int reservedCount = Driver.ChildAddressReserveRangeCount;
            if (Driver.DriverType == XDriverType.MRK_30)
            {
                reservedCount = 30;

                var reservedCountProperty = Properties.FirstOrDefault(x => x.Name == "MRK30ChildCount");
                if (reservedCountProperty != null)
                {
                    reservedCount = int.Parse(reservedCountProperty.Value);
                }
            }
            return reservedCount;
        }
    }
}