using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Infrastructure.Common;
using FiresecClient;

namespace ReportsModule.ViewModels
{
    public class FlowDocumentViewModel : BaseViewModel
    {
        public FlowDocumentViewModel()
        {
            Initialize();
        }

        public List<DeviceList> DevicesList { get; set; }

        void Initialize()
        {
            DevicesList = new List<DeviceList>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                string type = "";
                string address = "";
                string zonePresentationName = "";
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    zonePresentationName = "";
                    var a = device.ZoneLogic;
                    var b = device.IndicatorLogic;
                    type = device.Driver.ShortName;
                    var category = device.Driver.Category;
                    address = device.DottedAddress;
                    if (device.Driver.IsZoneDevice)
                    {

                        if (FiresecManager.DeviceConfiguration.Zones.IsNotNullOrEmpty())
                        {
                            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                            if (zone != null)
                            {
                                zonePresentationName = zone.PresentationName;
                            }
                        }
                    }

                    DevicesList.Add(new DeviceList()
                    {
                        Type = type,
                        Address = address,
                        ZoneName = zonePresentationName
                    });
                }
            }
        }
    }

    public class DeviceList
    {
        public string Type { get; set; }
        public string Address { get; set; }
        public string ZoneName { get; set; }
    }
}
