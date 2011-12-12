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
            base.ReportFileName = "IndicationBlockCrystalReport.rpt";
        }

        void TestInitialize()
        {
            DataList = new List<ReportIndicationBlockModel>();
            for (int i = 0; i < 91; ++i)
            {
                DataList.Add(new ReportIndicationBlockModel()
                {
                    Number = i.ToString(),
                    PresentationName = "TestPresentationName",
                    BlockIndicationNumber = i.ToString(),
                    PageNumber = i.ToString()
                });
            }
            for (int i = 91; i < 100; ++i)
            {
                DataList.Add(new ReportIndicationBlockModel()
                {
                    Number = "",
                    PresentationName = "",
                    BlockIndicationNumber = i.ToString(),
                    PageNumber = i.ToString()
                });
            }
        }

        public override void LoadData()
        {
            //TestInitialize();
            var IndicationBlockList = new List<IndicationBlocksList>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                var devices = FiresecManager.DeviceConfiguration.Devices.FindAll(x => (x.Driver.DriverType == DriverType.IndicationBlock));
                foreach (var device in devices)
                {
                    IndicationBlockList.Add(new IndicationBlocksList(device));
                }
            }
            foreach (var block in IndicationBlockList)
            {
                var stringBuilder = new StringBuilder();
                foreach (var page in block.Pages)
                {
                    foreach (var element in page.ElementsPage)
                    {
                        DataList.Add(new ReportIndicationBlockModel()
                        {
                            Number = element.No.ToString(),
                            PresentationName = element.PresentationName,
                            BlockIndicationNumber = block.IndicationBlockNumber,
                            PageNumber = page.PageNumber.ToString()
                        });
                    }
                }
            }
        }
    }
}
