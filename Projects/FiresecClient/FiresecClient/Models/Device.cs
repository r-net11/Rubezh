using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecClient
{
    public class Device
    {
        public Device()
        {
            UderlyingZones = new List<string>();
        }

        public Device Parent { get; set; }
        public List<Device> Children { get; set; }
        public string DatabaseId { get; set; }
        public string DriverId { get; set; }
        public Firesec.Metadata.configDrv Driver { get; set; }
        public string PlaceInTree { get; set; }
        public string Id { get; set; }
        public string Address { get; set; }
        public string ZoneNo { get; set; }
        public Firesec.ZoneLogic.expr ZoneLogic { get; set; }
        public List<Property> Properties { get; set; }
        public string Description { get; set; }
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

        public bool IsZoneDevice
        {
            get
            {
                if ((Driver.minZoneCardinality == "0") && (Driver.maxZoneCardinality == "0"))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsZoneLogicDevice
        {
            get
            {
                if ((Driver.options != null) && (Driver.options.Contains("ExtendedZoneLogic")))
                {
                    return true;
                }
                return false;
            }
        }

        public string PresentationZone
        {
            get
            {
                if (IsZoneDevice)
                {
                    if (string.IsNullOrEmpty(ZoneNo))
                        return "";

                    Zone zone = FiresecManager.Configuration.Zones.FirstOrDefault(x => x.No == ZoneNo);
                    return ZoneNo + "." + zone.Name;
                }
                if (IsZoneLogicDevice)
                {
                    return ZoneLogicToText.Convert(ZoneLogic);
                }
                return "";
            }
        }
    }
}
