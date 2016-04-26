using iTextSharp.text;
using iTextSharp.text.pdf;
using System;

namespace Common.PDF
{
	public class PageEventHelper : PdfPageEventHelper
	{
		private PdfContentByte cb;
		private PdfTemplate template;

		public int FontSize { get; set; }

		public int FotterShift { get; set; }

		public bool PrintPages { get; set; }

		public bool PrintDate { get; set; }

		public bool PrintFooterLine { get; set; }

		public DateTime PrintDateTime { get; private set; }

		public PageEventHelper()
		{
			PrintFooterLine = true;
			PrintPages = true;
			PrintDate = true;
			FontSize = 8;
			FotterShift = 10;
		}

		public override void OnOpenDocument(PdfWriter writer, Document document)
		{
			PrintDateTime = DateTime.Now;
			cb = writer.DirectContent;
			template = cb.CreateTemplate(50, 50);
		}

		public override void OnCloseDocument(PdfWriter writer, Document document)
		{
			base.OnCloseDocument(writer, document);
			if (PrintPages)
			{
				template.BeginText();
				template.SetFontAndSize(PDFStyle.BaseFont, FontSize);
				template.SetTextMatrix(0, 0);
				template.ShowText((writer.PageNumber - 1).ToString());
				template.EndText();
			}
		}

		public override void OnStartPage(PdfWriter writer, Document document)
		{
			base.OnStartPage(writer, document);
		}

		public override void OnEndPage(PdfWriter writer, Document document)
		{
			base.OnEndPage(writer, document);

			Rectangle pageSize = document.PageSize;
			cb.SetRGBColorFill(100, 100, 100);

			if (PrintFooterLine)
			{
				cb.SetRGBColorStroke(100, 100, 100);
				cb.SetLineWidth(1f);
				cb.MoveTo(pageSize.GetLeft(document.LeftMargin), pageSize.GetBottom(document.BottomMargin - 2f));
				cb.LineTo(pageSize.GetRight(document.RightMargin), pageSize.GetBottom(document.BottomMargin - 2f));
				cb.Stroke();
			}
			if (PrintPages)
			{
				int pageN = writer.PageNumber;
				string text = string.Format(Resources.Language.PageEventHelper.text, pageN);

				float len = PDFStyle.BaseFont.GetWidthPoint(text, FontSize);

				cb.BeginText();
				cb.SetFontAndSize(PDFStyle.BaseFont, FontSize);
				cb.SetTextMatrix(pageSize.GetLeft(document.LeftMargin), pageSize.GetBottom(document.BottomMargin - FotterShift));
				cb.ShowText(text);
				cb.EndText();

				cb.AddTemplate(template, pageSize.GetLeft(document.LeftMargin) + len, pageSize.GetBottom(document.BottomMargin - FotterShift));
			}
			if (PrintDate)
			{
				cb.BeginText();
				cb.SetFontAndSize(PDFStyle.BaseFont, FontSize);
				cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, PrintDateTime.ToString("dd.MM.yyyy HH:mm:ss"), pageSize.GetRight(document.RightMargin), pageSize.GetBottom(document.BottomMargin - FotterShift), 0);
				cb.EndText();
			}
		}
	}
}