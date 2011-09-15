using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using Common;
using FiresecClient;

namespace ReportsModule.Reports
{
    public class ReportTypesCount
    {
        List<DriverCounter> DriverCounters;

        public XpsDocument CreateReport()
        {
            Initialize();

            var reportDocument = new ReportDocument();
            string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\TypesCountFlowDocument.xaml";
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                reportDocument.XamlData = new StreamReader(fileStream).ReadToEnd();
            }

            DataTable tableHeader = null;
            DataTable tableData = null;

            // Microsoft.Performance : 'Report1.CreateReport()' объявляет переменную 'obj' типа 'object[]',
            //которая никогда не используется или которой только присваивается значение. Используйте эту переменную, или удалите ее.
            object[] obj = null;

            var data = new ReportData();

            using (tableHeader = new DataTable("Header"))
            using (tableData = new DataTable("Data"))
            {
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
            }

            using (tableData = new DataTable("Total"))
            {
                tableData.Columns.Add();
                tableData.Columns.Add();
                tableData.Rows.Add("Всего", DriverCounters.Count.ToString());

                data.DataTables.Add(tableData);
            }

            return reportDocument.CreateXpsDocument(data);
        }

        void Initialize()
        {
            DriverCounters = new List<DriverCounter>();

            foreach (var driver in FiresecManager.Drivers)
            {
                if (driver.IsPlaceable && driver.ShortName != "Компьютер")
                {
                    var devices = FiresecManager.DeviceConfiguration.Devices.FindAll(x => x.Driver.UID == driver.UID);
                    if (devices.IsNotNullOrEmpty())
                    {
                        DriverCounters.Add(new DriverCounter()
                        {
                            DriverName = driver.ShortName,
                            Count = devices.Count
                        });
                    }
                }
            }
        }
    }

    internal class DriverCounter
    {
        public string DriverName { get; set; }
        public int Count { get; set; }
    }
}