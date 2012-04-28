using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportDevicesList : BaseReportGeneric<ReportDeviceListModel>
    {
        public ReportDevicesList()
			: base()
        {
            ReportFileName = "DeviceListCrystalReport.rpt";
        }

        public override void LoadData()
        {
            DataList = new List<ReportDeviceListModel>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                string type = "";
                string address = "";
                string zonePresentationName = "";
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    zonePresentationName = "";
                    type = device.Driver.ShortName;
                    address = device.DottedAddress;
                    if (device.Driver.IsZoneDevice)
                    {
                        if (FiresecManager.DeviceConfiguration.Zones.IsNotNullOrEmpty())
                        {
                            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                            if (zone != null)
                                zonePresentationName = zone.PresentationName;
                        }
                    }

                    if (device.Driver.DriverType == DriverType.Indicator)
                    {
                        if (device.IndicatorLogic.Zones.Count == 1)
                        {
                            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.IndicatorLogic.Zones[0]);
                            zonePresentationName = "Зоны: " + zone == null ? zone.PresentationName : "";
                        }
                        else
                        {
                            zonePresentationName = device.IndicatorLogic.ToString();
                        }
                    }

                    if (device.Driver.DriverType == DriverType.Page)
                        address = device.IntAddress.ToString();
                    if (device.Driver.DriverType == DriverType.PumpStation)
                    {
                    }
                    DataList.Add(new ReportDeviceListModel()
                    {
                        Type = type,
                        Address = address,
                        ZoneName = zonePresentationName
                    });
                }
            }
        }
    }
}