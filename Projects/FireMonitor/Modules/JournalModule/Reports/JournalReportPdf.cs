using System.Data;
using CodeReason.Reports;
using Controls.PDF;
using Infrastructure.Common.Windows.Reports;
using iTextSharp.text;

namespace JournalModule.Reports
{
	internal class JournalReportPdf : IReportPdfProvider
	{
		#region IReportPdfProvider Members

		public Rectangle PageFormat
		{
			get { return PageSize.A4.Rotate(); }
		}
		public ReportData ReportData { get; set; }

		public bool CanPrint
		{
			get { return ReportData != null; }
		}

		public void Print(Document document)
		{
			var table = PDFHelper.CreateTable(document, 5);
			table.HeaderRows = 2;
			table.SetWidths(new float[] { 1f, 1f, 1f, 2f, 2f });
			var cell = PDFHelper.GetCell(string.Format("Журнал событий с {0:dd.MM.yyyy HH:mm:ss} по {1:dd.MM.yyyy HH:mm:ss}", ReportData.ReportDocumentValues["StartDate"], ReportData.ReportDocumentValues["EndDate"]), PDFStyle.HeaderFont, PDFStyle.HeaderBackground);
			cell.Colspan = 8;
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			var headers = new string[] 
			{
				"Дата в приборе",
				"Дата в системе",
				"Название",
				"Описание",
				"Объект"
			};
			foreach (var heder in headers)
			{
				cell = PDFHelper.GetCell(heder, PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);
			};
			foreach (DataRow row in ReportData.DataTables[0].Rows)
			{
				table.AddCell(PDFHelper.GetCell(row["DeviceDateTime"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["SystemDateTime"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["Name"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["Description"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["ObjectName"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
			}
			document.Add(table);
		}

		#endregion
	}
}