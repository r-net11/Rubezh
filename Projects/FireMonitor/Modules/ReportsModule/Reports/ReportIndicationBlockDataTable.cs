using System.Collections.Generic;
using System.Text;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportIndicationBlockDataTable
    {
        public string RdlcFileName = "ReportIndicationBlockRDLC.rdlc";
        public string DataTableName = "DataSetIndicationBlock";

        public ReportIndicationBlockDataTable()
        {
            _indicationBlockList = new List<ReportIndicationBlockModel>();
        }

        public void Initialize()
        {
            var IndicationBlockList = new List<ReportIndicationBlockModelTemp>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                var devices = FiresecManager.DeviceConfiguration.Devices.FindAll(x => (x.Driver.DriverType == DriverType.IndicationBlock));
                foreach (var device in devices)
                {
                    IndicationBlockList.Add(new ReportIndicationBlockModelTemp(device));
                }
            }
            var listElement = new List<Element>();
            foreach (var block in IndicationBlockList)
            {
                var stringBuilder = new StringBuilder();
                foreach (var page in block.Pages)
                {
                    stringBuilder.Clear();
                    stringBuilder.Append("Блок индикации: ");
                    stringBuilder.Append(block.IndicationBlockNumber);
                    stringBuilder.Append("Страница БИ:");
                    stringBuilder.Append(page.PageNumber);
                    listElement.Clear();
                    foreach (var element in page.ElementsPage)
                    {
                        listElement.Add(new Element() { Number = element.No.ToString(), PresentationName = element.PresentationName });
                        _indicationBlockList.Add(new ReportIndicationBlockModel()
                        {
                            Number = element.No.ToString(),
                            PresentationName = element.PresentationName
                        });
                    }
                    //_indicationBlockList.Add(new ReportIndicationBlockModel()
                    //{
                    //    HeaderTable = stringBuilder.ToString(),
                    //    IndicationBlockTable = listElement
                    //});
                }
            }
        }

        List<ReportIndicationBlockModel> _indicationBlockList;
        public List<ReportIndicationBlockModel> IndicationBlockList
        {
            get { return new List<ReportIndicationBlockModel>(_indicationBlockList); }
        }
    }
}
