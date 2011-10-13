using System.Collections.Generic;
using System.Text;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportIndicationBlock : BaseReportGeneric<ReportIndicationBlockModel>
    {
        public ReportIndicationBlock()
        {
            base.RdlcFileName = "ReportIndicationBlockRDLC.rdlc";
            base.DataTableName = "DataSetIndicationBlock";
        }

        public void TestInitialize()
        {
            DataList = new List<ReportIndicationBlockModel>();
            for (int i = 0; i < 91; i++)
            {
                DataList.Add(new ReportIndicationBlockModel()
                {
                    Number = "50",
                    PresentationName = "TestPresentationName"
                });
            }
            for (int i = 91; i < 100; i++)
            {
                DataList.Add(new ReportIndicationBlockModel()
                {
                    Number = "",
                    PresentationName = ""
                });
            }
        }

        public override void LoadData()
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
                        DataList.Add(new ReportIndicationBlockModel()
                        {
                            Number = element.No.ToString(),
                            PresentationName = element.PresentationName
                        });
                    }
                }
            }
        }
    }
}
