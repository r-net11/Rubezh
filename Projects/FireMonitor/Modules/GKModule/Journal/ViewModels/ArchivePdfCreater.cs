using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.Diagnostics;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Pdf;
using GKModule.ViewModels;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common;
using System.IO;
using Microsoft.Win32;

namespace GKModule.Journal.ViewModels
{
	internal class ArchivePdfCreater
	{
		private IEnumerable<JournalItemViewModel> _source;
		private string _imagePath;

		private ArchivePdfCreater()
		{
			_imagePath = AppDataFolderHelper.GetTempFolder();
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
			if (!Directory.Exists(_imagePath))
				Directory.CreateDirectory(_imagePath);
			PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always)
			{
				Document = CreateDocument()
			};
			pdfRenderer.RenderDocument();
			pdfRenderer.Save(fileName);
			//if (Directory.Exists(_imagePath))
			//    Directory.Delete(_imagePath, true);
#if DEBUG
			Process.Start(fileName);
#endif
		}

		private Document CreateDocument()
		{
			var document = new Document()
			{
				Info = new DocumentInfo()
				{
					Title = "Архив",
					Subject = "",
					Author = "Рубеж / Оперативные задачи",
				},
				UseCmykColor = true,
			};
			DefineStyles(document);
			var section = CreatePage(document);
			var table = CreateTable(section);
			FillContent(table);
			return document;
		}
		private void DefineStyles(Document document)
		{
			Style style = document.Styles["Normal"];
			style.Font.Name = "Verdana";

			style = document.Styles[StyleNames.Header];
			style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

			style = document.Styles[StyleNames.Footer];
			style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

			style = document.Styles.AddStyle("Table", "Normal");
			style.Font.Name = "Verdana";
			style.Font.Name = "Times New Roman";
			style.Font.Size = 9;

			style = document.Styles.AddStyle("Reference", "Normal");
			style.ParagraphFormat.SpaceBefore = "5mm";
			style.ParagraphFormat.SpaceAfter = "5mm";
			style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
		}
		private Section CreatePage(Document document)
		{
			Section section = document.AddSection();
			section.PageSetup.Orientation = Orientation.Landscape;
			section.PageSetup.StartingNumber = 1;

			//var image = section.Headers.Primary.AddImage("../../PowerBooks.png");
			//image.Height = "2.5cm";
			//image.LockAspectRatio = true;
			//image.RelativeVertical = RelativeVertical.Line;
			//image.RelativeHorizontal = RelativeHorizontal.Margin;
			//image.Top = ShapePosition.Top;
			//image.Left = ShapePosition.Right;
			//image.WrapFormat.Style = WrapStyle.Through;

			var paragraph = section.Footers.Primary.AddParagraph();
			paragraph.AddText("Сгенерировано ");
			paragraph.AddDateField("dd.MM.yyyy");
			paragraph.Format.Font.Size = 9;
			paragraph.Format.Alignment = ParagraphAlignment.Left;

			paragraph = section.Footers.Primary.AddParagraph();
			paragraph.AddText("Страница ");
			paragraph.AddPageField();
			paragraph.Format.Font.Size = 9;
			paragraph.Format.Alignment = ParagraphAlignment.Right;

			//paragraph = section.Footers.Primary.AddParagraph();
			//paragraph.AddText(string.Format("Всего записей {0}", _source.Count()));
			//paragraph.Format.Font.Size = 9;
			//paragraph.Format.Alignment = ParagraphAlignment.Center;
			
			//paragraph = section.AddParagraph();
			//paragraph.Format.Alignment = ParagraphAlignment.Center;
			//paragraph.Style = "Reference";
			//paragraph.AddFormattedText("Архив", TextFormat.Bold);

			return section;
		}
		private Table CreateTable(Section section)
		{
			var table = section.AddTable();
			table.Style = "Table";
			table.Borders.Color = Colors.Black;
			table.Borders.Width = 0.25;
			table.Borders.Left.Width = 0.5;
			table.Borders.Right.Width = 0.5;
			table.Rows.LeftIndent = 0;
			CreateColumns(table);
			CreateHeader(table);
			return table;
		}
		private void CreateColumns(Table table)
		{
			var column = table.AddColumn(Unit.FromMillimeter(30));
			column.Format.Alignment = ParagraphAlignment.Center;
			column = table.AddColumn(Unit.FromMillimeter(30));
			column.Format.Alignment = ParagraphAlignment.Center;
			column = table.AddColumn(Unit.FromMillimeter(45));
			column = table.AddColumn(Unit.FromMillimeter(10));
			column = table.AddColumn(Unit.FromMillimeter(60));
			column = table.AddColumn(Unit.FromMillimeter(12));
			column = table.AddColumn(Unit.FromMillimeter(30));
			column = table.AddColumn(Unit.FromMillimeter(30));
		}
		private void CreateHeader(Table table)
		{
			var row = table.AddRow();
			row.HeadingFormat = true;
			row.Format.Alignment = ParagraphAlignment.Center;
			row.Format.Font.Bold = true;
			row.Shading.Color = Colors.LightBlue;
			row.Cells[0].AddParagraph("Дата в приборе");
			row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			row.Cells[1].AddParagraph("Системная дата");
			row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
			row.Cells[2].AddParagraph("Событие");
			row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			row.Cells[3].AddParagraph("");
			row.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			row.Cells[4].AddParagraph("Уточнение");
			row.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			row.Cells[5].AddParagraph("Объект");
			row.Cells[5].VerticalAlignment = VerticalAlignment.Center;
			row.Cells[6].AddParagraph("Класс состояния");
			row.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			row.Cells[7].AddParagraph("Пользователь");
			row.Cells[7].VerticalAlignment = VerticalAlignment.Center;
		}
		private void FillContent(Table table)
		{
			var imageSize = Unit.FromMillimeter(4);
			Image image;
			string source;
			Paragraph paragraph;
			foreach (var item in _source)
			{
				Row row = table.AddRow();
				row.Cells[0].AddParagraph(item.JournalItem.DeviceDateTime.ToString());
				row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
				row.Cells[1].AddParagraph(item.JournalItem.SystemDateTime.ToString());
				row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
				row.Cells[2].AddParagraph(item.JournalItem.Name ?? string.Empty);
				row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
				row.Cells[3].AddParagraph(item.JournalItem.YesNo.ToDescription());
				row.Cells[3].VerticalAlignment = VerticalAlignment.Center;
				row.Cells[4].AddParagraph(item.JournalItem.Description ?? string.Empty);
				row.Cells[4].VerticalAlignment = VerticalAlignment.Center;
				paragraph = row.Cells[5].AddParagraph();
				source = GetImage(item.ImageSource);
				if (source != null)
				{
					image = paragraph.AddImage(source);
					image.Width = imageSize;
					image.Height = imageSize;
				}
				paragraph.AddText(item.PresentationName ?? string.Empty);
				row.Cells[5].VerticalAlignment = VerticalAlignment.Center;
				paragraph = row.Cells[6].AddParagraph();
				if (item.JournalItem.StateClass != XStateClass.Norm && item.JournalItem.StateClass != XStateClass.Off)
				{
					source = GetImage("/Controls;component/StateClassIcons/" + item.JournalItem.StateClass.ToString() + ".png");
					if (source != null)
					{
						image = paragraph.AddImage(source);
						image.Width = imageSize;
						image.Height = imageSize;
					}
				}
				paragraph.AddText(item.JournalItem.StateClass.ToDescription());
				row.Cells[6].VerticalAlignment = VerticalAlignment.Center;
				row.Cells[7].AddParagraph(item.JournalItem.UserName ?? string.Empty);
				row.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			}
		}
		private string GetImage(string source)
		{
			var resource = System.Windows.Application.GetResourceStream(new Uri(source, UriKind.RelativeOrAbsolute));
			if (resource == null || resource.Stream == null)
				return null;

			var location = Path.GetInvalidFileNameChars().Aggregate(source, (current, c) => current.Replace(c.ToString(), string.Empty));
			location = Path.Combine(_imagePath, location);

			if (!File.Exists(location))
				using (var br = new BinaryReader(resource.Stream))
				using (FileStream fs = new FileStream(location, FileMode.Create))
				using (BinaryWriter bw = new BinaryWriter(fs))
				{
					byte[] ba = new byte[resource.Stream.Length];
					resource.Stream.Read(ba, 0, ba.Length);
					bw.Write(ba);
					br.Close();
					bw.Close();
					resource.Stream.Close();
				}
			return location;
		}
	}
}
