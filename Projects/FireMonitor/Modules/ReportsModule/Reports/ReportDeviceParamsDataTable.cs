using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportDeviceParamsDataTable
    {
        public string RdlcFileName = "ReportDeviceParamsRDLC.rdlc";
        public string DataTableName = "DataSetDeviceParams";

        public ReportDeviceParamsDataTable()
        {
            _deviceParamsList = new List<ReportDeviceParamsModel>();
        }

        public void Initialize()
        {
            _deviceParamsList = new List<ReportDeviceParamsModel>();
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
                    _deviceParamsList.Add(new ReportDeviceParamsModel()
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

        List<ReportDeviceParamsModel> _deviceParamsList;
        public List<ReportDeviceParamsModel> DeviceParamsList
        {
            get { return new List<ReportDeviceParamsModel>(_deviceParamsList); }
        }
    }
}
