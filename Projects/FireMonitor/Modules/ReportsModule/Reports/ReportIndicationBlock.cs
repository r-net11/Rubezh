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
using CodeReason.Reports.Document;

namespace ReportsModule.Reports
{
    public class ReportIndicationBlock
    {
        List<IndicationBlock> IndicationBlockList;

        public XpsDocument CreateReport()
        {
            Initialize();

            var reportDocument = new ReportDocument();
            //string path = @"H:\Rubezh\Projects\FireMonitor\Modules\ReportsModule\ReportTemplates\IndicationBlockFlowDocument.xaml";
            string path = @"ReportTemplates/IndicationBlockFlowDocument.xaml";
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                reportDocument.XamlData = new StreamReader(fileStream).ReadToEnd();
            }
            var dateTime = DateTime.Now;
            List<ReportData> listData = new List<ReportData>();
            int k = 1;
            foreach (var indicationBlock in IndicationBlockList)
            {
                foreach (var page in indicationBlock.Pages)
                {
                    DataTable dataTable = new DataTable("IndicationBlockList");
                    for (int i = 0; i < 10; i++)
                    {
                        dataTable.Columns.Add();
                    }
                    var data = new ReportData();
                    List<ElementPage> elementsPage = new List<ElementPage>();
                    for (int i = 1; i <= 10; i++)
                    {
                        elementsPage.Clear();
                        for (int j = i; j <= 50; j += 10)
                        {
                            elementsPage.Add(page.ElementsPage.FirstOrDefault(x => (x.No == j)));
                        }
                        object[] obj = new object[10];
                        int count = 0;
                        foreach (var elementPage in elementsPage)
                        {
                            obj[count] = elementPage.No.ToString();
                            obj[count + 1] = elementPage.PresentationName;
                            count += 2;
                        }
                        dataTable.Rows.Add(obj);
                    }
                    data.ReportDocumentValues.Add("PrintDate", dateTime);
                    data.ReportDocumentValues.Add("ReportNumber", k); // report number
                    k++;
                    DataTable dataHeaderTable = new DataTable("HeaderTable");
                    dataHeaderTable.Columns.Add();
                    dataHeaderTable.Rows.Add(new object[] { "Блок индикации:" + indicationBlock.IndicationBlockNumber + 
                    ", Страница БИ: " + page.PageNumber});
                    data.DataTables.Add(dataHeaderTable);
                    data.DataTables.Add(dataTable);
                    listData.Add(data);
                }
            }

            return reportDocument.CreateXpsDocument(listData);
        }

        void Initialize()
        {
            IndicationBlockList = new List<IndicationBlock>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                var devices = FiresecManager.DeviceConfiguration.Devices.FindAll(x => (x.Driver.DriverType == DriverType.IndicationBlock));
                foreach (var device in devices)
                {
                    IndicationBlockList.Add(new IndicationBlock(device));
                }
            }
        }
    }

    class IndicationBlock
    {
        private IndicationBlock() { }
        public IndicationBlock(Device device)
        {
            if (device.Driver.DriverType != DriverType.IndicationBlock)
            {
                return;
            }
            Pages = new List<Page>();
            IndicationBlockNumber = device.DottedAddress;
            foreach (var pageDevice in device.Children)
            {
                Pages.Add(new Page(pageDevice));
            }
        }

        public string IndicationBlockNumber { get; set; }
        public List<Page> Pages { get; set; }
    }

    class Page
    {
        private Page() { }
        public Page(Device device)
        {
            PageNumber = device.IntAddress;
            ElementsPage = new List<ElementPage>();
            foreach (var elementPage in device.Children)
            {
                ElementsPage.Add(new ElementPage(
                    elementPage.IntAddress,
                    elementPage.IndicatorLogic.Zones,
                    elementPage.IndicatorLogic.ToString()));
            }
        }
        public int PageNumber { get; set; }
        public List<ElementPage> ElementsPage { get; set; }
    }

    class ElementPage
    {
        private ElementPage() { }
        public ElementPage(int No, List<string> zonesNo, string presentationName)
        {
            this.No = No;
            ZonesNo = zonesNo;
            PresentationName = presentationName;
        }
        public int No { get; set; }
        public List<string> ZonesNo { get; set; }

        string _presentationName;
        public string PresentationName
        {
            get
            {
                if (ZonesNo.Count == 1)
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZonesNo[0]);
                    string presentationName = "";
                    if (zone != null)
                    {
                        presentationName = zone.PresentationName;
                    }
                    return ("Зоны: " + presentationName);
                }
                else
                {
                    return _presentationName;
                }
            }
            set
            {
                _presentationName = value;
            }
        }
    }
}