using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Reports;
using iTextSharp.text;
using CodeReason.Reports;
using Common.PDF;
using System.Data;

namespace GKModule.Reports
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
			var table = PDFHelper.CreateTable(document, 6);
			table.HeaderRows = 2;
			table.SetWidths(new float[] { 1f, 1f, 0.5f, 2f, 2f, 1f });
			var cell = PDFHelper.GetCell(string.Format("Журнал событий с {0:dd.MM.yyyy HH:mm:ss} по {1:dd.MM.yyyy HH:mm:ss}", ReportData.ReportDocumentValues["StartDate"], ReportData.ReportDocumentValues["EndDate"]), PDFStyle.HeaderFont, PDFStyle.HeaderBackground);
			cell.Colspan = 8;
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			var headers = new string[] 
			{
				"Дата",
				"Название",
				"Д/Н",
				"Описание",
				"Объект",
				"Состояние",
			};
			foreach (var heder in headers)
			{
				cell = PDFHelper.GetCell(heder, PDFStyle.TextFont, Element.ALIGN_CENTER, PDFStyle.HeaderBackground);
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);
			};
			foreach (DataRow row in ReportData.DataTables[0].Rows)
			{
				table.AddCell(PDFHelper.GetCell(row["DateTime"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["Name"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["YesNo"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["Description"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["ObjectName"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
				table.AddCell(PDFHelper.GetCell(row["StateClass"].ToString(), PDFStyle.TextFont, Element.ALIGN_CENTER));
			}
			document.Add(table);
		}

		#endregion
	}
}
