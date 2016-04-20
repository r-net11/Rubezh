using System.Data;
using CodeReason.Reports;
using Controls.PDF;
using Infrastructure.Common.Windows.Reports;
using iTextSharp.text;

namespace GKModule.Reports
{
	internal class DriverCounterReportPdf : IReportPdfProvider
	{
		#region IReportPdfProvider Members

		public Rectangle PageFormat
		{
			get { return PageSize.A4; }
		}
		public ReportData ReportData { get; set; }

		public bool CanPrint
		{
			get { return ReportData != null; }
		}

		public void Print(Document document)
		{
			var table = PDFHelper.CreateTable(document, ReportData.DataTables[0].Columns.Count);
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
			foreach (DataRow row in ReportData.DataTables[0].Rows)
			{
				table.AddCell(PDFHelper.GetCell(row[0].ToString(), PDFStyle.TextFont));
				cell = PDFHelper.GetCell(row[1].ToString(), PDFStyle.TextFont);
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				table.AddCell(cell);
			}
			cell = PDFHelper.GetCell("Всего устройств", PDFStyle.BoldTextFont);
			table.AddCell(cell);
			cell = PDFHelper.GetCell(ReportData.DataTables[0].Compute("Sum(Count)", string.Empty).ToString(), PDFStyle.BoldTextFont);
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			document.Add(table);
		}

		#endregion
	}
}