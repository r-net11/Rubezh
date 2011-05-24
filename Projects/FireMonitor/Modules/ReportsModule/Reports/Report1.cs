using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeReason.Reports;
using System.IO;
using System.Data;
using FiresecClient;
using System.Windows.Xps.Packaging;
using Infrastructure.Common;

namespace ReportsModule.Reports
{
    public class Report1
    {
        List<DriverCounter> DriverCounters;

        public XpsDocument CreateReport()
        {
            Initialize();

            ReportDocument reportDocument = new ReportDocument();
            StreamReader streamReader = new StreamReader(new FileStream(PathHelper.Report, FileMode.Open, FileAccess.Read));
            reportDocument.XamlData = streamReader.ReadToEnd();
            streamReader.Close();

            DataTable tableHeader = null;
            DataTable tableData = null;
            object[] obj = null;
            ReportData data = new ReportData();

            tableHeader = new DataTable("Header");
            tableData = new DataTable("Data");

            tableHeader.Columns.Add();
            tableHeader.Rows.Add(new object[] { "Устройство" });
            tableHeader.Rows.Add(new object[] { "Количество" });
            tableData.Columns.Add();
            tableData.Columns.Add();
            obj = new object[2];
            foreach (var driverCounter in DriverCounters)
            {
                tableData.Rows.Add(driverCounter.DriverName, driverCounter.Count);
            }

            data.DataTables.Add(tableHeader);
            data.DataTables.Add(tableData);

            tableData = new DataTable("Total");
            tableData.Columns.Add();
            tableData.Columns.Add();
            tableData.Rows.Add("Всего", DriverCounters.Count.ToString());
            data.DataTables.Add(tableData);

            return reportDocument.CreateXpsDocument(data);
        }

        void Initialize()
        {
            DriverCounters = new List<DriverCounter>();

            foreach (var driver in FiresecManager.CurrentConfiguration.Metadata.drv)
            {
                if ((driver.options != null) && (driver.options.Contains("Placeable")) && (driver.shortName != "Компьютер"))
                {
                    var devices = FiresecManager.CurrentConfiguration.AllDevices.FindAll(x => x.DriverId == driver.id);
                    if (devices.Count > 0)
                    {
                        DriverCounter driverCounter = new DriverCounter();
                        driverCounter.DriverName = driver.shortName;
                        driverCounter.Count = devices.Count;
                        DriverCounters.Add(driverCounter);
                    }
                }
            }
        }
    }

    class DriverCounter
    {
        public string DriverName { get; set; }
        public int Count { get; set; }
    }
}
