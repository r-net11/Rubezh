using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GKModule.ViewModels;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common;
using System.IO;
using Microsoft.Win32;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Common.PDF;

namespace GKModule.Journal.ViewModels
{
	internal class ArchivePdfCreater
	{
		private IEnumerable<JournalItemViewModel> _source;

		private ArchivePdfCreater()
		{
		}

		public static void Create(IEnumerable<JournalItemViewModel> source)
		{
			var fileName = PDFHelper.ShowSavePdfDialog();
			if (!string.IsNullOrEmpty(fileName))
				WaitHelper.Execute(() =>
				{
					var creater = new ArchivePdfCreater();
					creater._source = source;
					creater.CreatePdf(fileName);
				});
		}
		private void CreatePdf(string fileName)
		{
			using (var fs = new FileStream(fileName, FileMode.Create))
			using (var pdf = new PDFDocument(fs, PageSize.A4.Rotate()))
			{
				pdf.PageEventHelper.PrintFooterLine = false;
				AddDocumentHeader(pdf.Document);
				var table = CreateTable(pdf.Document);
				CreateHeader(table);
				FillContent(table);
				pdf.Document.Add(table);
#if DEBUG
				Process.Start(fileName);
#endif
			}
		}

		private void AddDocumentHeader(Document doc)
		{
			doc.AddAuthor("Рубеж / Оперативные задачи");
			doc.AddTitle("Архив");
			doc.AddCreator("ОЗ");
			//doc.AddKeywords("AddKeywords");
			//doc.AddSubject("AddSubject");
			doc.AddCreationDate();
		}
		private PdfPTable CreateTable(Document doc)
		{
			var table = new PdfPTable(8);
			//table.SplitLate = false;
			table.HeaderRows = 1;
			table.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
			table.LockedWidth = true;
			float[] widths = new float[] { 25f, 25f, 45f, 10f, 60f, 40f, 35f, 30f };
			table.SetWidths(widths);
			table.HorizontalAlignment = 1;
			table.SpacingBefore = 20f;
			table.SpacingAfter = 30f;
			return table;
		}
		private void CreateHeader(PdfPTable table)
		{
			var background = new BaseColor(System.Windows.Media.Colors.LightBlue.R, System.Windows.Media.Colors.LightBlue.G, System.Windows.Media.Colors.LightBlue.B, System.Windows.Media.Colors.LightBlue.A);
			var headers = new string[]
			{
				"Дата в приборе",
				"Системная дата",
				"Событие",
				"",
				"Уточнение",
				"Объект",
				"Класс состояния",
				"Ip-адрес ГК"
			};
			foreach (var header in headers)
			{
				var cell = PDFHelper.GetCell(header, PDFStyle.BoldFont, Element.ALIGN_CENTER, background);
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);
			}
		}
		private void FillContent(PdfPTable table)
		{
			foreach (var item in _source)
			{
				var background = GetStateColor(item.JournalItem.StateClass);
				table.AddCell(PDFHelper.GetCell(item.JournalItem.DeviceDateTime.ToString(), PDFStyle.NormalFont, Element.ALIGN_CENTER, background));
				table.AddCell(PDFHelper.GetCell(item.JournalItem.SystemDateTime.ToString(), PDFStyle.NormalFont, Element.ALIGN_CENTER, background));
				table.AddCell(PDFHelper.GetCell(item.JournalItem.Name ?? string.Empty, background));
				table.AddCell(PDFHelper.GetCell(item.JournalItem.YesNo.ToDescription(), background));
				table.AddCell(PDFHelper.GetCell(item.JournalItem.Description ?? string.Empty, background));//4
				table.AddCell(PDFHelper.GetImageTextCell(item.ImageSource, item.PresentationName ?? string.Empty, 6, background));
				table.AddCell(PDFHelper.GetImageTextCell(item.JournalItem.StateClass != XStateClass.Norm && item.JournalItem.StateClass != XStateClass.Off ? "/Controls;component/StateClassIcons/" + item.JournalItem.StateClass.ToString() + ".png" : null, item.JournalItem.StateClass.ToDescription(), 5, background));
				table.AddCell(PDFHelper.GetCell(item.JournalItem.GKIpAddress ?? string.Empty, background));//7
			}
		}
		private BaseColor GetStateColor(XStateClass state)
		{
			System.Drawing.Color color;
			switch (state)
			{
				case XStateClass.Fire2:
					color = System.Drawing.Color.Red;
					break;
				case XStateClass.Fire1:
					color = System.Drawing.Color.Red;
					break;
				case XStateClass.Attention:
					color = System.Drawing.Color.Yellow;
					break;
				case XStateClass.Failure:
					color = System.Drawing.Color.Pink;
					break;
				case XStateClass.Service:
					color = System.Drawing.Color.Yellow;
					break;
				case XStateClass.Ignore:
					color = System.Drawing.Color.Yellow;
					break;
				case XStateClass.Unknown:
					color = System.Drawing.Color.Gray;
					break;
				case XStateClass.On:
					color = System.Drawing.Color.LightBlue;
					break;
				case XStateClass.AutoOff:
					color = System.Drawing.Color.Yellow;
					break;
				case XStateClass.Info:
					color = System.Drawing.Color.Transparent;
					break;
				case XStateClass.Norm:
					color = System.Drawing.Color.Transparent;
					break;
				default:
					color = System.Drawing.Color.Transparent;
					break;
			}
			return new BaseColor(color);
		}
	}
}
