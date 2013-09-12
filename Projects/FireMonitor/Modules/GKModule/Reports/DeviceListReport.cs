using System.Data;
using CodeReason.Reports;
using Common.PDF;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace GKModule.Reports
{
	internal class DeviceListReport : ISingleReportProvider
	{
		private DataTable _table;

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			var data = new ReportData();
			_table = new DataTable("Devices");
			_table.Columns.Add("Type");
			_table.Columns.Add("Address");
			_table.Columns.Add("Zone");

			if (FiresecManager.Devices.IsNotNullOrEmpty())
			{
				foreach (var device in XManager.Devices)
				{
					if (device.Driver.DriverType == XFiresecAPI.XDriverType.System)
						continue;
					if (device.Driver.IsGroupDevice)
						continue;

					var type = device.ShortName;
					var address = device.DottedPresentationAddress;
					var zonePresentationName = XManager.GetPresentationZone(device);
					_table.Rows.Add(type, address, zonePresentationName);
				}
			}
			data.DataTables.Add(_table);
			return data;
		}
		#endregion

		#region IReportProvider Members
		public string Template
		{
			get { return "Reports/DeviceListReport.xaml"; }
		}

		public string Title
		{
			get { return "Список устройств"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public bool CanPdfPrint
		{
			get { return true; }
		}
		public void PdfPrint(iTextSharp.text.Document document)
		{
			var table = PDFHelper.CreateTable(document, _table.Columns.Count);
			table.HeaderRows = 3;
			table.SetWidths(new float[] { 3f, 2f, 5f });
			var cell = PDFHelper.GetCell("Список устройств конфигурации", PDFStyle.HeaderFont, PDFStyle.HeaderBackground);
			cell.Colspan = 3;
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Устройство", PDFStyle.TextFont, PDFStyle.HeaderBackground);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			cell.Colspan = 2;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Зона", PDFStyle.TextFont, PDFStyle.HeaderBackground);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			cell.Rowspan = 2;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Тип", PDFStyle.TextFont, PDFStyle.HeaderBackground);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Адрес", PDFStyle.TextFont, PDFStyle.HeaderBackground);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			PDFHelper.PrintTable(table, _table);
			document.Add(table);
		}

		#endregion
	}
}