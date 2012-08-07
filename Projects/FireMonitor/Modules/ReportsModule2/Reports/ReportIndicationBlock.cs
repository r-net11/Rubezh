using System.Collections.Generic;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule2.Models;

namespace ReportsModule2.Reports
{
	public class ReportIndicationBlock : BaseReportGeneric<ReportIndicationBlockModel>
	{
		public ReportIndicationBlock()
		{
			base.ReportFileName = "IndicationBlockCrystalReport.rpt";
			XpsDocumentName = "IndicationBlockCrystalReport.xps";
		}

		public override void LoadData()
		{
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

		public override void CreateFlowDocumentStringBuilder()
		{
			var flowDocumentSB = new StringBuilder();
			flowDocumentSB.Append(@"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" >");
			flowDocumentSB.Append(@"<Table CellSpacing=""0.1"" BorderThickness=""1,1,1,1"" BorderBrush=""#FFFFFFFF"">");
			flowDocumentSB.Append(@"<Table.Columns><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /></Table.Columns>");
			flowDocumentSB.Append(@"<TableRowGroup Name=""RowVisual2"" FontWeight=""Normal"" FontSize=""12"" Background=""#FFFFFFFF"">");
			string tableCellOpenTag = @"<TableCell BorderThickness=""1,1,1,1"" BorderBrush=""#FF000000""><Paragraph>";
			string tableRowOpenTag = @"<TableRow>";
			string tableRowCloseTag = @"</TableRow>";
			string tableCellCloseTag = @"</Paragraph></TableCell>";
			foreach (var deviceListModel in DataList)
			{
				if ((int.Parse(deviceListModel.Number) - 1) % 10 == 0 || int.Parse(deviceListModel.Number) == 1)
					flowDocumentSB.Append(tableRowOpenTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.Number.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.PresentationName.ToString() + tableCellCloseTag);
				if (int.Parse(deviceListModel.Number) % 10 == 0 || int.Parse(deviceListModel.Number) == 10)
					flowDocumentSB.Append(tableRowCloseTag);
			}
			flowDocumentSB.Append(@"</TableRowGroup></Table></FlowDocument>");
			FlowDocumentStringBuilder = flowDocumentSB;
		}
	}
}