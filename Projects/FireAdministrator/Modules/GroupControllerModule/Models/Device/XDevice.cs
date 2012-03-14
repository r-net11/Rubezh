using System;
using System.Collections.Generic;
using System.Linq;
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
            OutDependenceUIDs = new List<Guid>();
        }

        public XDriver Driver { get; set; }
        public XDevice Parent { get; set; }
        public List<Guid> OutDependenceUIDs { get; set; }
        public short ObjectNo { get; set; }

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

        public XDevice Copy(bool fullCopy)
        {
            var newDevice = new XDevice()
            {
                DriverUID = DriverUID,
                Driver = Driver,
                IntAddress = IntAddress,
                Description = Description
            };

            if (fullCopy)
            {
                newDevice.UID = UID;
            }

            newDevice.Properties = new List<XProperty>();
            foreach (var property in Properties)
            {
                newDevice.Properties.Add(new XProperty()
                {
                    Name = property.Name,
                    Value = property.Value
                });
            }

            newDevice.Children = new List<XDevice>();
            foreach (var childDevice in Children)
            {
                var newChildDevice = childDevice.Copy(fullCopy);
                newChildDevice.Parent = newDevice;
                newDevice.Children.Add(newChildDevice);
            }

            return newDevice;
        }
    }
}