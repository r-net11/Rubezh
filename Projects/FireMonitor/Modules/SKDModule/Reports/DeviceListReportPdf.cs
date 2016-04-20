using CodeReason.Reports;
using Controls.PDF;
using Infrastructure.Common.Windows.Reports;
using iTextSharp.text;

namespace SKDModule.Reports
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
			table.HeaderRows = 2;
			table.SetWidths(new float[] { 2f, 1f, 2f });
			var cell = PDFHelper.GetCell("Список устройств конфигурации", PDFStyle.HeaderFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.Colspan = 3;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Название", PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Адрес", PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			cell = PDFHelper.GetCell("Тип", PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			PDFHelper.PrintTable(table, ReportData.DataTables[0]);
			document.Add(table);
		}

		#endregion
	}
}