using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Device
    {
        public Device()
        {
            UID = Guid.NewGuid();
            Children = new List<Device>();
            Properties = new List<Property>();
            IndicatorLogic = new IndicatorLogic();
            PDUGroupLogic = new PDUGroupLogic();
            PlanElementUIDs = new List<Guid>();
        }

        public Driver Driver { get; set; }
        public Device Parent { get; set; }
        public List<string> ShapeIds { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public List<Device> Children { get; set; }

        [DataMember]
        public string DatabaseId { get; set; }

        [DataMember]
        public Guid DriverUID { get; set; }

        [DataMember]
        public int IntAddress { get; set; }

        [DataMember]
        public ulong? ZoneNo { get; set; }

        [DataMember]
        public ZoneLogic ZoneLogic { get; set; }

        [DataMember]
        public IndicatorLogic IndicatorLogic { get; set; }

        [DataMember]
        public PDUGroupLogic PDUGroupLogic { get; set; }

        [DataMember]
        public List<Property> Properties { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsRmAlarmDevice { get; set; }

        [DataMember]
        public List<Guid> PlanElementUIDs { get; set; }

        [DataMember]
        public bool IsMonitoringDisabled { get; set; }

        [DataMember]
        public bool IsAltInterface { get; set; }

        public string EditingPresentationAddress
        {
            get
            {
                if (Driver.HasAddress == false)
                    return "";

                return AddressConverter.IntToStringAddress(Driver, IntAddress);
            }
        }

        public string PresentationAddress
        {
            get
            {
                if (Driver.HasAddress == false)
                    return "";

                string address = AddressConverter.IntToStringAddress(Driver, IntAddress);

                if (Driver.IsChildAddressReservedRange)
                {
                    int reservedCount = GetReservedCount();

                    int endAddress = IntAddress + reservedCount;
                    if (endAddress >> 8 != IntAddress >> 8)
                        endAddress = (IntAddress / 256) * 256 + 255;
                    address += " - " + AddressConverter.IntToStringAddress(Driver, endAddress);
                }

                return address;
            }
        }

        public string PresentationAddressDriver
        {
            get
            {
                if (Driver.HasAddress)
                    return PresentationAddress + " - " + Driver.Name;
                return Driver.Name;
            }
        }

        public int GetReservedCount()
        {
            int reservedCount = Driver.ChildAddressReserveRangeCount;
            if (Driver.DriverType == DriverType.MRK_30)
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

        public void SetAddress(string address)
        {
            IntAddress = AddressConverter.StringToIntAddress(Driver, address);
            if (Driver.IsChildAddressReservedRange)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].IntAddress = IntAddress + i;
                }
            }
        }

        public string AddressFullPath
        {
            get
            {
                string address = IntAddress.ToString();

                if ((Driver.DriverType == DriverType.MS_1) || (Driver.DriverType == DriverType.MS_2))
                {
                    if ((Parent.Children != null) && (Parent.Children.Any(x => ((x.Driver.DriverType == DriverType.MS_1) || (x.Driver.DriverType == DriverType.MS_2)))))
                    {
                        var serialNoProperty = Properties.FirstOrDefault(x => x.Name == "SerialNo");
                        if (serialNoProperty != null)
                            address = serialNoProperty.Value;
                    }
                }

                if (Driver.IsDeviceOnShleif)
                    address = AddressConverter.IntToStringAddress(Driver, IntAddress);
                return address;
            }
        }

        public string Id
        {
            get
            {
                string currentId = Driver.UID.ToString() + ":" + AddressFullPath;
                if (Parent != null)
                    return Parent.Id + @"/" + currentId;
                return currentId;
            }
        }

        public string DottedAddress
        {
            get
            {
                var address = new StringBuilder();
                foreach (var parentDevice in AllParents.Where(x => x.Driver.HasAddress))
                {
                    if (parentDevice.Driver.IsChildAddressReservedRange)
                        continue;

                    address.Append(parentDevice.PresentationAddress);
                    address.Append(".");
                }
                if (Driver.HasAddress)
                {
                    address.Append(PresentationAddress);
                    address.Append(".");
                }

                if (address.Length > 0 && address[address.Length - 1] == '.')
                    address.Remove(address.Length - 1, 1);

                return address.ToString();
            }
        }

        public bool CanEditAddress
        {
            get
            {
                if (Parent != null && Parent.Driver.IsChildAddressReservedRange && Parent.Driver.DriverType != DriverType.MRK_30)
                    return false;
                return (Driver.HasAddress && Driver.CanEditAddress);
            }
        }

        public string PlaceInTree
        {
            get
            {
                if (Parent == null)
                    return "";
                if (Parent.PlaceInTree == "")
                    return Parent.Children.IndexOf(this).ToString();
                return Parent.PlaceInTree + @"\" + Parent.Children.IndexOf(this).ToString();
            }
        }

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

        public Device Channel
        {
            get
            {
                return AllParents.FirstOrDefault(x => (x.Driver.DriverType == DriverType.USB_Channel ||
                    x.Driver.DriverType == DriverType.USB_Channel_1 ||
                    x.Driver.DriverType == DriverType.USB_Channel_2));
            }
        }

        public string ConnectedTo
        {
            get
            {
                if (Parent == null)
                    return null;

                string parentPart = Parent.Driver.ShortName;
                if (Parent.Driver.HasAddress)
                    parentPart += " - " + Parent.PresentationAddress;

                if (Parent.ConnectedTo == null || Parent.Parent.ConnectedTo == null)
                    return parentPart;

                return parentPart + @"\" + Parent.ConnectedTo;
            }
        }

        public Device Copy(bool fullCopy)
        {
            var newDevice = new Device()
            {
                DriverUID = DriverUID,
                Driver = Driver,
                IntAddress = IntAddress,
                Description = Description,
                ZoneNo = ZoneNo
            };

            if (fullCopy)
            {
                newDevice.UID = UID;
                newDevice.DatabaseId = DatabaseId;
            }

            if (ZoneLogic != null)
            {
                newDevice.ZoneLogic = new Models.ZoneLogic();
                newDevice.ZoneLogic.JoinOperator = ZoneLogic.JoinOperator;
                foreach (var clause in ZoneLogic.Clauses)
                {
                    newDevice.ZoneLogic.Clauses.Add(new Clause()
                    {
                        State = clause.State,
                        Operation = clause.Operation,
                        Zones = clause.Zones.ToList()
                    });
                }
            }

            newDevice.Properties = new List<Property>();
            foreach (var property in Properties)
            {
                newDevice.Properties.Add(new Property()
                {
                    Name = property.Name,
                    Value = property.Value
                });
            }

            newDevice.Children = new List<Device>();
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