using CodeReason.Reports;
using Common.PDF;
using Infrastructure.Common.Reports;
using iTextSharp.text;
using Localization.SKD.Common;

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
			var cell = PDFHelper.GetCell(CommonResources.ConfigurationDeviceList, PDFStyle.HeaderFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.Colspan = 3;
			table.AddCell(cell);
			cell = PDFHelper.GetCell(CommonResources.Name, PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			cell = PDFHelper.GetCell(CommonResources.Address, PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			cell = PDFHelper.GetCell(CommonResources.Type, PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
			cell.VerticalAlignment = Element.ALIGN_MIDDLE;
			table.AddCell(cell);
			PDFHelper.PrintTable(table, ReportData.DataTables[0]);
			document.Add(table);
		}

		#endregion
	}
}