using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportDeviceParams : BaseReportGeneric<ReportDeviceParamsModel>
    {
        public ReportDeviceParams()
            : base()
        {
            base.RdlcFileName = "ReportDeviceParamsRDLC.rdlc";
            base.DataTableName = "DataSetDeviceParams";
        }

        public override void LoadData()
        {
            DataList = new List<ReportDeviceParamsModel>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                string type = "";
                string address = "";
                string zonePresentationName = "";
                string dustiness = "";
                string failureType = "";
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    if ((device.Driver.Category == DeviceCategoryType.Other) ||
                        (device.Driver.Category == DeviceCategoryType.Communication))
                    {
                        continue;
                    }
                    zonePresentationName = "";
                    dustiness = "";
                    address = "";
                    type = "";
                    failureType = "";

                    type = device.Driver.ShortName;
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

                    dustiness = "";
                    failureType = "";
                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
                    if (deviceState.Parameters != null)
                    {
                        var parameter = deviceState.Parameters.FirstOrDefault(x => (x.Name == "Dustiness" && x.Visible));
                        if (parameter != null)
                        {
                            if ((string.IsNullOrEmpty(parameter.Value) == false) && (parameter.Value != "<NULL>"))
                            {
                                dustiness = parameter.Value;
                            }
                        }
                        parameter = deviceState.Parameters.FirstOrDefault(x => (x.Name == "FailureType" && x.Visible));
                        if (parameter != null)
                        {
                            if ((string.IsNullOrEmpty(parameter.Value) == false) && (parameter.Value != "<NULL>"))
                            {
                                failureType = parameter.Value;
                            }
                        }
                    }
                    DataList.Add(new ReportDeviceParamsModel()
                    {
                        Type = type,
                        Address = address,
                        Zone = zonePresentationName,
                        Dustiness = dustiness,
                        FailureType = failureType
                    });
                }
            }
        }
    }
}
