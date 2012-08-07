using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ReportsModule2.Models;
using System.Text;

namespace ReportsModule2.Reports
{
    public class ReportDeviceParams : BaseReportGeneric<ReportDeviceParamsModel>
    {
        public ReportDeviceParams() : base()
        {
            ReportFileName = "DeviceParamsCrystalReport.rpt";
			XpsDocumentName = "DeviceParamsCrystalReport.xps";
        }

        public override void LoadData()
        {
            DataList = new List<ReportDeviceParamsModel>();
            if (FiresecManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
            {
                string type = "";
                string address = "";
                string zonePresentationName = "";
                string dustiness = "";
                string failureType = "";
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    if (device.Driver.Category == DeviceCategoryType.Other || device.Driver.Category == DeviceCategoryType.Communication)
                        continue;

                    type = device.Driver.ShortName;
                    address = device.DottedAddress;
                    zonePresentationName = "";
                    dustiness = "";
                    failureType = "";

                    if (device.Driver.IsZoneDevice)
                    {
                        if (FiresecManager.DeviceConfiguration.Zones.IsNotNullOrEmpty())
                        {
                            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                            if (zone != null)
                                zonePresentationName = zone.PresentationName;
                        }
                    }

                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
                    if (deviceState.Parameters != null)
                    {
                        var parameter = deviceState.Parameters.FirstOrDefault(x => (x.Name == "Dustiness" && x.Visible));
                        if (parameter != null)
                        {
                            if (string.IsNullOrEmpty(parameter.Value) == false && parameter.Value != "<NULL>")
                                dustiness = parameter.Value;
                        }
                        parameter = deviceState.Parameters.FirstOrDefault(x => (x.Name == "FailureType" && x.Visible));
                        if (parameter != null)
                        {
                            if (string.IsNullOrEmpty(parameter.Value) == false && parameter.Value != "<NULL>")
                                failureType = parameter.Value;
                        }
                    }
                    DataList.Add(new ReportDeviceParamsModel()
                    {
                        Type = type,
                        Address = address,
                        Zone = zonePresentationName,
                        Dustiness = dustiness,
                        FailureType = failureType
                    });
                }
            }
        }

		public override void CreateFlowDocumentStringBuilder()
		{
			var flowDocumentSB = new StringBuilder();
			flowDocumentSB.Append(@"<FlowDocument xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" >");
			flowDocumentSB.Append(@"<Table CellSpacing=""0.1"" BorderThickness=""1,1,1,1"" BorderBrush=""#FFFFFFFF"">");
			flowDocumentSB.Append(@"<Table.Columns><TableColumn /><TableColumn /><TableColumn /><TableColumn /><TableColumn /></Table.Columns>");
			flowDocumentSB.Append(@"<TableRowGroup Name=""RowVisual"" ><TableRow FontWeight=""Bold"" FontSize=""14"" Background=""#FFC0C0C0""><TableCell><Paragraph>Тип</Paragraph></TableCell><TableCell><Paragraph>Адрес</Paragraph></TableCell><TableCell><Paragraph>Зона</Paragraph></TableCell><TableCell><Paragraph>Запыленность</Paragraph></TableCell><TableCell><Paragraph>Неисправность</Paragraph></TableCell></TableRow></TableRowGroup>");
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
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.Zone.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.Dustiness.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableCellOpenTag + deviceListModel.Address.ToString() + tableCellCloseTag);
				flowDocumentSB.Append(tableRowCloseTag);
			}
			flowDocumentSB.Append(@"</TableRowGroup></Table></FlowDocument>");
			FlowDocumentStringBuilder = flowDocumentSB;
		}
    }
}