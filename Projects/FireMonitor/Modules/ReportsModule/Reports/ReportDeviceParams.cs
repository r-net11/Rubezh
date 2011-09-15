using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using Common;
using FiresecClient;
using Infrastructure.Common;
using System.Windows.Documents;
using System;
using System.Text;
using FiresecAPI.Models;
using Microsoft.Practices.Unity.Utility;
using ReportsModule.ViewModels;

namespace ReportsModule.Reports
{
    public class ReportDeviceParams
    {
        List<DeviceParam> DeviceParams;

        public XpsDocument CreateReport()
        {
            Initialize();

            var reportDocument = new ReportDocument();
            string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\DeviceParamsFlowDocument.xaml";

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                reportDocument.XamlData = new StreamReader(fileStream).ReadToEnd();
            }

            var data = new ReportData();
            var dateTime = DateTime.Now;
            data.ReportDocumentValues.Add("PrintDate", dateTime);

            var dataTable = new DataTable("DeviceParams");
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();

            foreach (var deviceParam in DeviceParams)
            {
                dataTable.Rows.Add(deviceParam.Type, deviceParam.Address, deviceParam.Zone, deviceParam.Dustiness,
                    deviceParam.FailureType);
            }
            data.DataTables.Add(dataTable);
            return reportDocument.CreateXpsDocument(data);
        }

        void Initialize()
        {
            DeviceParams = new List<DeviceParam>();
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
                    DeviceParams.Add(new DeviceParam()
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

    internal class DeviceParam
    {
        public string Type { get; set; }
        public string Address { get; set; }
        public string Zone { get; set; }
        public string Dustiness { get; set; }
        public string FailureType { get; set; }
    }
}