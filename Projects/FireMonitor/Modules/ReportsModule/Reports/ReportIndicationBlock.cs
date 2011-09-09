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
    public class ReportIndicationBlock
    {
        List<IndicationBlock> IndicationBlockList;

        public XpsDocument CreateReport()
        {
            Initialize();

            var reportDocument = new ReportDocument();
            string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\IndicationBlockFlowDocument.xaml";
            
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                reportDocument.XamlData = new StreamReader(fileStream).ReadToEnd();
            }

            var data = new ReportData();
            var dateTime = DateTime.Now;
            data.ReportDocumentValues.Add("PrintDate", dateTime);

            var dataTable = new DataTable("IndicationBlockList");

            Helper.AddDataColumns<IndicationBlock>(dataTable);
            data.DataTables.Add(dataTable);
            return reportDocument.CreateXpsDocument(data);
        }

        void Initialize()
        {
            IndicationBlockList = new List<IndicationBlock>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
               
            }
        }
    }

    internal class IndicationBlock
    {
        public int No { get; set; }
        public string PresentationZone { get; set; }
    }
}