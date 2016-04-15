using CodeReason.Reports;
using Controls.PDF;
using Infrastructure.Common.Reports;
using iTextSharp.text;

namespace GKModule.Reports
{
	internal class DeviceListReportPdf : IReportPdfProvider
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
			table.HeaderRows = 3;
			table.SetWidths(new float[] { 3f, 2f, 5f });
			var cell = PDFHelper.GetCell("Список устройств конфигурации", PDFStyle.HeaderFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.Colspan = 3;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Устройство", PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			cell.Colspan = 2;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Зона", PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			cell.Rowspan = 2;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Тип", PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Адрес", PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			PDFHelper.PrintTable(table, ReportData.DataTables[0]);
			document.Add(table);
		}

		#endregion
	}
}