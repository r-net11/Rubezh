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

namespace GKModule.Journal.ViewModels
{
	internal class ArchivePdfCreater
	{
		private BaseFont BaseFont;
		private Font NormalFont;
		private Font BoldFont;
		private IEnumerable<JournalItemViewModel> _source;
		private Dictionary<string, Image> _cache;

		private ArchivePdfCreater()
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Tahoma.TTF");
			BaseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
			NormalFont = new Font(BaseFont, Font.DEFAULTSIZE, Font.NORMAL);
			BoldFont = new Font(BaseFont, Font.DEFAULTSIZE, Font.BOLD);
		}

		public static void Create(IEnumerable<JournalItemViewModel> source)
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = "Файлы PDF|*.pdf",
				DefaultExt = "Файлы PDF|*.pdf"
			};
			if (saveDialog.ShowDialog().Value)
				WaitHelper.Execute(() =>
				{
					var creater = new ArchivePdfCreater();
					creater._source = source;
					creater.CreatePdf(saveDialog.FileName);
				});
		}
		private void CreatePdf(string fileName)
		{
			using (var fs = new FileStream(fileName, FileMode.Create))
			{
				_cache = new Dictionary<string, Image>();
				var doc = new Document(PageSize.A4.Rotate());
				var pdfWriter = PdfWriter.GetInstance(doc, fs);
				PageEventHelper pageEventHelper = new PageEventHelper(BaseFont);
				pdfWriter.PageEvent = pageEventHelper;
				doc.Open();
				AddDocumentHeader(doc);
				var table = CreateTable(doc);
				CreateHeader(table);
				FillContent(table);
				doc.Add(table);
				doc.Close();
				_cache.Clear();
			}
#if DEBUG
			Process.Start(fileName);
#endif
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
			float[] widths = new float[] { 25f, 25f, 45f, 10f, 60f, 35f, 30f, 25f };
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
				var cell = new PdfPCell(new Phrase(header, BoldFont));
				cell.BackgroundColor = background;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);
			}
		}
		private void FillContent(PdfPTable table)
		{
			foreach (var item in _source)
			{
				var background = GetStateColor(item.JournalItem.StateClass);
				table.AddCell(GetCell(item.JournalItem.DeviceDateTime.ToString(), background));
				table.AddCell(GetCell(item.JournalItem.SystemDateTime.ToString(), background));
				table.AddCell(GetCell(item.JournalItem.Name ?? string.Empty, background));
				table.AddCell(GetCell(item.JournalItem.YesNo.ToDescription(), background));
				table.AddCell(GetCell(item.JournalItem.Description ?? string.Empty, background));//4
				table.AddCell(GetImageTextCell(item.ImageSource, item.PresentationName ?? string.Empty, background, 6));
				table.AddCell(GetImageTextCell(item.JournalItem.StateClass != XStateClass.Norm && item.JournalItem.StateClass != XStateClass.Off ? "/Controls;component/StateClassIcons/" + item.JournalItem.StateClass.ToString() + ".png" : null, item.JournalItem.StateClass.ToDescription(), background, 5));
				table.AddCell(GetCell(item.JournalItem.GKIpAddress ?? string.Empty, background));//7
			}
		}
		private PdfPCell GetCell(string content, BaseColor background)
		{
			return new PdfPCell(new Phrase(content, NormalFont))
			{
				BackgroundColor = background
			};
		}
		private PdfPCell GetImageTextCell(string imageSource, string text, BaseColor background, float partial)
		{
			var image = GetImage(imageSource);
			var nestedTable = new PdfPTable(2);
			nestedTable.SetWidths(new float[] { 1f, partial });
			nestedTable.DefaultCell.Border = 0;
			if (image == null)
				nestedTable.AddCell("");
			else
				nestedTable.AddCell(image);
			nestedTable.AddCell(new Phrase(text, NormalFont));

			return new PdfPCell(nestedTable)
			{
				Padding = 0,
				BackgroundColor = background
			};
		}
		private Image GetImage(string source)
		{
			if (string.IsNullOrEmpty(source))
				return null;
			if (_cache.ContainsKey(source))
				return _cache[source];

			var uri = new Uri(source, UriKind.RelativeOrAbsolute);
			var resource = System.Windows.Application.GetResourceStream(uri);
			if (resource == null || resource.Stream == null)
				return null;

			var image = Image.GetInstance(resource.Stream);
			if (image == null)
				return null;

			_cache.Add(source, image);
			return image;
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

	public class PageEventHelper : PdfPageEventHelper
	{
		private PdfContentByte cb;
		private PdfTemplate template;
		private BaseFont bf;
		private DateTime printTime;

		public int FontSize { get; set; }
		public int FotterShift { get; set; }

		public PageEventHelper(BaseFont baseFont)
		{
			bf = baseFont;
			FontSize = 9;
			FotterShift = 10;
		}

		public override void OnOpenDocument(PdfWriter writer, Document document)
		{
			printTime = DateTime.Now;
			cb = writer.DirectContent;
			template = cb.CreateTemplate(50, 50);
		}

		public override void OnEndPage(PdfWriter writer, Document document)
		{
			base.OnEndPage(writer, document);

			int pageN = writer.PageNumber;
			String text = "Страница " + pageN + " из ";

			float len = bf.GetWidthPoint(text, 8);
			Rectangle pageSize = document.PageSize;

			cb.SetRGBColorFill(100, 100, 100);

			cb.BeginText();
			cb.SetFontAndSize(bf, 8);
			cb.SetTextMatrix(pageSize.GetLeft(document.LeftMargin), pageSize.GetBottom(document.BottomMargin - FotterShift));
			cb.ShowText(text);
			cb.EndText();

			cb.AddTemplate(template, pageSize.GetLeft(document.LeftMargin) + len, pageSize.GetBottom(document.BottomMargin - FotterShift));

			cb.BeginText();
			cb.SetFontAndSize(bf, 8);
			cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Сгенерировано " + printTime.ToString(), pageSize.GetRight(document.RightMargin), pageSize.GetBottom(document.BottomMargin - FotterShift), 0);
			cb.EndText();
		}

		public override void OnCloseDocument(PdfWriter writer, Document document)
		{
			base.OnCloseDocument(writer, document);

			template.BeginText();
			template.SetFontAndSize(bf, FontSize);
			template.SetTextMatrix(0, 0);
			template.ShowText((writer.PageNumber - 1).ToString());
			template.EndText();
		}
	}
}
