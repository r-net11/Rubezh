using System.Data;
using CodeReason.Reports;
using FiresecClient;
using Infrastructure.Common.Reports;
using XFiresecAPI;
using iTextSharp.text.pdf;
using Common.PDF;
using iTextSharp.text;

namespace GKModule.Reports
{
	internal class DriverCounterReport : ISingleReportProvider
	{
		private DataTable _table;

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			var data = new ReportData();

			_table = new DataTable("Devices");
			_table.Columns.Add("Driver", typeof(string));
			_table.Columns.Add("Count", typeof(int));
			foreach (var driver in XManager.DriversConfiguration.XDrivers)
			{
				if (driver.IsAutoCreate || driver.DriverType == XDriverType.System)
					continue;
				AddDrivers(driver, _table);
			}
			data.DataTables.Add(_table);
			return data;
		}
		#endregion

		#region IReportProvider Members
		public string Template
		{
			get { return "Reports/DriverCounterReport.xaml"; }
		}

		public string Title
		{
			get { return "Количество устройств по типам"; }
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
			table.HeaderRows = 2;
			table.SetWidths(new float[] { 5f, 1f });
			var cell = PDFHelper.GetCell("Количество устройств в конфигурации", PDFStyle.HeaderFont, PDFStyle.HeaderBackground);
			cell.Colspan = 2;
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Устройство", PDFStyle.TextFont, PDFStyle.HeaderBackground);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Используется в конфигурации", PDFStyle.TextFont, PDFStyle.HeaderBackground);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			foreach (DataRow row in _table.Rows)
			{
				table.AddCell(PDFHelper.GetCell(row[0].ToString(), PDFStyle.TextFont));
				cell = PDFHelper.GetCell(row[1].ToString(), PDFStyle.TextFont);
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				table.AddCell(cell);
			}
			cell = PDFHelper.GetCell("Всего устройств", PDFStyle.BoldTextFont);
			table.AddCell(cell);
			cell = PDFHelper.GetCell(_table.Compute("Sum(Count)", string.Empty).ToString(), PDFStyle.BoldTextFont);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			document.Add(table);
		}
		#endregion

		private void AddDrivers(XDriver driver, DataTable table)
		{
			var count = 0;
			foreach (var device in XManager.Devices)
			{
				if (device.Driver.DriverType == driver.DriverType)
				{
					if (device.Parent.Driver.IsGroupDevice)
						continue;
					count++;
				}
			}
			if (count > 0)
				table.Rows.Add(driver.ShortName, count);
		}
	}
}