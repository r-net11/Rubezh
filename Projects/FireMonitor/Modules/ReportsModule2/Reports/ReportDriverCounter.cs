using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule2.Models;
using System.Text;

namespace ReportsModule2.Reports
{
    public class ReportDriverCounter : BaseReportGeneric<ReportDriverCounterModel>
    {
        public ReportDriverCounter()
        {
            ReportFileName = "DriverCounterCrystalReport.rpt";
			XpsDocumentName = "DriverCounterCrystalReport.xps";
        }

        public override void LoadData()
        {
            DataList = new List<ReportDriverCounterModel>();

            foreach (var driver in FiresecManager.Drivers)
            {
                if (driver.IsPlaceable && driver.DriverType != DriverType.Computer)
                {
                    AddDrivers(driver);
                }
                else
                {
                    switch (driver.DriverType)
                    {
                        case DriverType.IndicationBlock:
                            AddDrivers(driver);
                            break;
                        case DriverType.MS_1:
                            AddDrivers(driver);
                            break;
                        case DriverType.MS_2:
                            AddDrivers(driver);
                            break;
                        case DriverType.MS_3:
                            AddDrivers(driver);
                            break;
                        case DriverType.MS_4:
                            AddDrivers(driver);
                            break;
                        default:
                            break;
                    }
                }
            }

            DataList.Add(new ReportDriverCounterModel()
            {
                DriverName = "Всего устройств",
                Count = CountDrivers()
            });
        }

		public override void CreateFlowDocumentStringBuilder()
		{
			var flowDocumentSB = new StringBuilder();
			flowDocumentSB.Append(@"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" >");
			flowDocumentSB.Append(@"<Table CellSpacing=""0.1"" BorderThickness=""1,1,1,1"" BorderBrush=""#FFFFFFFF"">");
			flowDocumentSB.Append(@"<Table.Columns><TableColumn /><TableColumn /></Table.Columns>");
			flowDocumentSB.Append(@"<TableRowGroup Name=""RowVisual""><TableRow FontWeight=""Bold"" FontSize=""14"" Background=""#FFC0C0C0""><TableCell><Paragraph>Устройство</Paragraph></TableCell><TableCell><Paragraph>Используется в конфигурации</Paragraph></TableCell></TableRow></TableRowGroup>");
			flowDocumentSB.Append(@"<TableRowGroup Name=""RowVisual2"" FontWeight=""Normal"" FontSize=""12"" Background=""#FFFFFFFF"">");
			string tableCellOpenTag = @"<TableCell BorderThickness=""1,1,1,1"" BorderBrush=""#FF000000""><Paragraph>";
			string tableRowOpenTag = @"<TableRow>";
			string tableRowCloseTag = @"</TableRow>";
			string tableCellCloseTag = @"</Paragraph></TableCell>";
			foreach (var deviceListModel in DataList)
			{
				flowDocumentSB.Append(tableRowOpenTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.DriverName.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.Count.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableRowCloseTag);
			}
			flowDocumentSB.Append(@"</TableRowGroup></Table></FlowDocument>");
			FlowDocumentStringBuilder = flowDocumentSB;
		}

        void AddDrivers(Driver driver)
        {
            var devices = FiresecManager.Devices.FindAll(x => x.Driver.UID == driver.UID);
            if (devices.IsNotNullOrEmpty())
            {
                DataList.Add(new ReportDriverCounterModel()
                {
                    DriverName = driver.ShortName,
                    Count = devices.Count
                });
            }
        }

        int CountDrivers()
        {
            int count = 0;
            DataList.ForEach(x => count += x.Count);
            return count;
        }
    }
}