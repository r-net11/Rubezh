using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule2.Models;
using System.Text;
using System.Windows.Xps.Packaging;
using System.IO;

namespace ReportsModule2.Reports
{
	public class ReportDevicesList : BaseReportGeneric<ReportDeviceListModel>
	{
		public ReportDevicesList()
			: base()
		{
			ReportFileName = "DeviceListCrystalReport.rpt";
			XpsDocumentName = "DeviceListCrystalReport.xps";
		}

		public StringBuilder FlowDocumentStringBuilder { get; set; }
		public string XpsDocumentName { get; set; }
		public XpsDocument XpsDocument
		{
			get
			{
				var xpsDocument = new XpsDocument(XpsDocumentName, FileAccess.Read);
				return xpsDocument;
			}
		}

		public override void LoadData()
		{
			DataList = new List<ReportDeviceListModel>();
			if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
			{
				string type = "";
				string address = "";
				string zonePresentationName = "";
				foreach (var device in FiresecManager.DeviceConfiguration.Devices)
				{
					zonePresentationName = "";
					type = device.Driver.ShortName;
					address = device.DottedAddress;
					if (device.Driver.IsZoneDevice)
					{
						if (FiresecManager.DeviceConfiguration.Zones.IsNotNullOrEmpty())
						{
							var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
							if (zone != null)
								zonePresentationName = zone.PresentationName;
						}
					}

					if (device.Driver.DriverType == DriverType.Indicator)
					{
						if (device.IndicatorLogic.Zones.Count == 1)
						{
							var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.IndicatorLogic.Zones[0]);
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
					DataList.Add(new ReportDeviceListModel()
					{
						Type = type,
						Address = address,
						ZoneName = zonePresentationName
					});
				}
			}
		}

		public void CreateFlowDocumentStringBuilder()
		{
			var flowDocumentSB = new StringBuilder();
			flowDocumentSB.Append(@"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" >");
			flowDocumentSB.Append(@"<Table CellSpacing=""0.1"" BorderThickness=""1,1,1,1"" BorderBrush=""#FFFFFFFF"">");
			flowDocumentSB.Append(@"<Table.Columns><TableColumn /><TableColumn /><TableColumn /></Table.Columns>");
			flowDocumentSB.Append(@"<TableRowGroup><TableRow FontWeight=""Bold"" FontSize=""14"" Background=""#FFC0C0C0""><TableCell><Paragraph>Тип</Paragraph></TableCell><TableCell><Paragraph>Адрес</Paragraph></TableCell><TableCell><Paragraph>Зона</Paragraph></TableCell></TableRow></TableRowGroup>");
			flowDocumentSB.Append(@"<TableRowGroup FontWeight=""Normal"" FontSize=""12"" Background=""#FFFFFFFF"">");
			foreach (var deviceListModel in DataList)
			{
				string tableCellHeader = @"<TableCell BorderThickness=""1,1,1,1"" BorderBrush=""#FF000000"">";
				flowDocumentSB.Append(@"<TableRow>");
				flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + deviceListModel.Type.ToString() + "</Paragraph></TableCell>");
				flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + deviceListModel.Address.ToString() + "</Paragraph></TableCell>");
				flowDocumentSB.Append(tableCellHeader + "<Paragraph>" + deviceListModel.ZoneName.ToString() + "</Paragraph></TableCell>");
				flowDocumentSB.Append(@"</TableRow>");
			}
			flowDocumentSB.Append(@"</TableRowGroup></Table></FlowDocument>");
			FlowDocumentStringBuilder = flowDocumentSB;
		}
	}
}