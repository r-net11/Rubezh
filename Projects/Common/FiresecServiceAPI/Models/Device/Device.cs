using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Device
    {
        public Device()
        {
            Children = new List<Device>();
            Properties = new List<Property>();
            Properties = new List<Property>();
            ZoneLogic = new ZoneLogic();
            IndicatorLogic = new IndicatorLogic();
            PDUGroupLogic = new PDUGroupLogic();
            ShapeIds = new List<string>();
            UID = Guid.NewGuid();
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
        public string ZoneNo { get; set; }

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

        public string PresentationAddress
        {
            get
            {
                if (Driver.HasAddress == false)
                {
                    return "";
                }

                string address = AddressConverter.IntToStringAddress(Driver, IntAddress);

                if (Driver.IsChildAddressReservedRange)
                {
                    int endAddress = IntAddress + Driver.ChildAddressReserveRangeCount;
                    if (endAddress / 256 != IntAddress / 256)
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
                string result = "";
                if (Driver.HasAddress)
                    result = PresentationAddress + " - ";
                return result + Driver.Name;
            }
        }

        public void SetAddress(string address)
        {
            IntAddress = AddressConverter.StringToIntAddress(Driver, address);
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
                {
                    address = AddressConverter.IntToStringAddress(Driver, IntAddress);
                }

                return address;
            }
        }

        public string Id
        {
            get
            {
                string currentId = Driver.UID.ToString() + ":" + AddressFullPath;
                if (Parent != null)
                {
                    return Parent.Id + @"/" + currentId;
                }
                return currentId;
            }
        }

        public string DottedAddress
        {
            get
            {
                string address = "";
                foreach (var parentDevice in AllParents)
                {
                    if (parentDevice.Driver.HasAddress)
                    {
                        address += parentDevice.PresentationAddress + ".";
                    }
                }
                if (Driver.HasAddress)
                {
                    address += PresentationAddress + ".";
                }
                if (address.EndsWith("."))
                    address = address.Remove(address.Length - 1);

                return address;
            }
        }

        public string PlaceInTree
        {
            get
            {
                if (Parent == null)
                    return "";

                string localPlaceInTree = Parent.Children.IndexOf(this).ToString();
                if (Parent.PlaceInTree == "")
                {
                    return localPlaceInTree;
                }
                else
                {
                    return Parent.PlaceInTree + @"\" + localPlaceInTree;
                }
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

        public string ConnectedTo
        {
            get
            {
                if (Parent == null)
                    return null;
                else
                {
                    string parentPart = Parent.Driver.ShortName;
                    if (Parent.Driver.HasAddress)
                        parentPart += " - " + Parent.PresentationAddress;

                    if (Parent.ConnectedTo == null)
                        return parentPart;

                    if (Parent.Parent.ConnectedTo == null)
                        return parentPart;

                    return parentPart + @"\" + Parent.ConnectedTo;
                }
            }
        }

        public Device Copy(bool fullCopy)
        {
            var newDevice = new Device();
            newDevice.Driver = Driver;
            newDevice.IntAddress = IntAddress;
            newDevice.Description = Description;
            newDevice.ZoneNo = ZoneNo;

            if (fullCopy)
            {
                newDevice.UID = UID;
                //newDevice.DatabaseId = DatabaseId;
            }

            if (ZoneLogic != null)
            {
                newDevice.ZoneLogic = new Models.ZoneLogic();
                newDevice.ZoneLogic.JoinOperator = ZoneLogic.JoinOperator;
                foreach (var clause in ZoneLogic.Clauses)
                {
                    var newClause = new Clause();
                    newClause.State = clause.State;
                    newClause.Operation = clause.Operation;
                    newClause.Zones = clause.Zones.ToList();
                    newDevice.ZoneLogic.Clauses.Add(newClause);
                }
            }

            var copyProperties = new List<Property>();
            foreach (var property in Properties)
            {
                var copyProperty = new Property();
                copyProperty.Name = property.Name;
                copyProperty.Value = property.Value;
                copyProperties.Add(copyProperty);
            }
            newDevice.Properties = copyProperties;

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