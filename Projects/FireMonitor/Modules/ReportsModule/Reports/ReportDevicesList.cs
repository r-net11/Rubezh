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
using ReportsModule.ViewModels;

namespace ReportsModule.Reports
{
    public class ReportDevicesList
    {
        List<DeviceList> DevicesList;

        public XpsDocument CreateReport()
        {
            
            Initialize();
            var reportDocument = new ReportDocument();
            string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\DeviceListFlowDocument.xaml";
            
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                reportDocument.XamlData = new StreamReader(fileStream).ReadToEnd();
            }

            var data = new ReportData();
            var dateTime = DateTime.Now;
            data.ReportDocumentValues.Add("PrintDate", dateTime);

            var dataTable = new DataTable("DevicesList");
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            dataTable.Columns.Add();
            foreach (var device in DevicesList)
            {
                dataTable.Rows.Add(device.Type, device.Address, device.ZoneName);
            }

            data.DataTables.Add(dataTable);
            
            return reportDocument.CreateXpsDocument(data);
        }

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

    internal class DeviceList
    {
        public string Type { get; set; }
        public string Address { get; set; }
        public string ZoneName { get; set; }
    }
}