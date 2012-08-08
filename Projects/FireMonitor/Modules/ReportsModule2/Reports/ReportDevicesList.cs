using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule2.Models;
using System.Text;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Data;

namespace ReportsModule2.Reports
{
	public class ReportDevicesList : BaseReportGeneric<ReportDeviceListModel>
	{
		public ReportDevicesList()
			: base()
		{
			ReportFileName = "ReportDeviceList";
			XpsDocumentName = "DeviceListCrystalReport.xps";
		}

		public DataTable DevicdesListDataTable { get; set; }

		public override void LoadData()
		{
			DataList = new List<ReportDeviceListModel>();
			if (FiresecManager.Devices.IsNotNullOrEmpty())
			{
				string type = "";
				string address = "";
				string zonePresentationName = "";
				foreach (var device in FiresecManager.Devices)
				{
					zonePresentationName = "";
					type = device.Driver.ShortName;
					address = device.DottedAddress;
					if (device.Driver.IsZoneDevice)
					{
						if (FiresecManager.Zones.IsNotNullOrEmpty())
						{
							var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
							if (zone != null)
								zonePresentationName = zone.PresentationName;
						}
					}

					if (device.Driver.DriverType == DriverType.Indicator)
					{
						if (device.IndicatorLogic.Zones.Count == 1)
						{
							var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == device.IndicatorLogic.Zones[0]);
							zonePresentationName = "Зоны: " + zone == null ? zone.PresentationName : "";
						}
						else
						{
							zonePresentationName = device.IndicatorLogic.ToString();
						}
					}

					if (device.Driver.DriverType == DriverType.Page)
						address = device.IntAddress.ToString();
					if (device.Driver.DriverType == DriverType.PumpStation)
					{
					}
					for (int i = 0; i < 10; i++)
						DataList.Add(new ReportDeviceListModel()
						{
							Type = type,
							Address = address,
							ZoneName = zonePresentationName
						});
				}
			}
		}

		public override void CreateFlowDocumentStringBuilder()
		{
			var flowDocumentSB = new StringBuilder();
			flowDocumentSB.Append(@"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" >");
			flowDocumentSB.Append(@"<Table CellSpacing=""0.1"" BorderThickness=""1,1,1,1"" BorderBrush=""#FFFFFFFF"">");
			flowDocumentSB.Append(@"<Table.Columns><TableColumn /><TableColumn /><TableColumn /></Table.Columns>");
			flowDocumentSB.Append(@"<TableRowGroup Name=""RowVisual"" ><TableRow FontWeight=""Bold"" FontSize=""14"" Background=""#FFC0C0C0""><TableCell><Paragraph>Тип</Paragraph></TableCell><TableCell><Paragraph>Адрес</Paragraph></TableCell><TableCell><Paragraph>Зона</Paragraph></TableCell></TableRow></TableRowGroup>");
			flowDocumentSB.Append(@"<TableRowGroup Name=""RowVisual2"" FontWeight=""Normal"" FontSize=""12"" Background=""#FFFFFFFF"">");
			string tableCellOpenTag = @"<TableCell BorderThickness=""1,1,1,1"" BorderBrush=""#FF000000""><Paragraph>";
			string tableRowOpenTag = @"<TableRow>";
			string tableRowCloseTag = @"</TableRow>";
			string tableCellCloseTag = @"</Paragraph></TableCell>";
			foreach (var deviceListModel in DataList)
			{
				flowDocumentSB.Append(tableRowOpenTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.Type.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.Address.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.ZoneName.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableRowCloseTag);
			}
			flowDocumentSB.Append(@"</TableRowGroup></Table></FlowDocument>");
			FlowDocumentStringBuilder = flowDocumentSB;
		}
	}
}