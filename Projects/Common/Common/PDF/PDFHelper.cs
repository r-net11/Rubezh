using iTextSharp.text;
using iTextSharp.text.pdf;
using Localization.Common.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;

namespace Common.PDF
{
	public static class PDFHelper
	{
		private static Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

		public static Image GetImage(string source)
		{
			if (string.IsNullOrEmpty(source))
				return null;
			if (_imageCache.ContainsKey(source))
				return _imageCache[source];

			var uri = new Uri(source, UriKind.RelativeOrAbsolute);
			var resource = System.Windows.Application.GetResourceStream(uri);
			if (resource == null || resource.Stream == null)
				return null;

			var image = Image.GetInstance(resource.Stream);
			if (image == null)
				return null;

			_imageCache.Add(source, image);
			return image;
		}

		public static void ClearCaches()
		{
			_imageCache.Clear();
		}

		public static string ShowSavePdfDialog()
		{
			var saveDialog = new SaveFileDialog()
			{
				Filter = CommonResources.PDFFiles,
				DefaultExt = CommonResources.PDFFiles
			};
			return saveDialog.ShowDialog().Value ? saveDialog.FileName : null;
		}

		public static PdfPCell GetCell(string content, BaseColor background = null)
		{
			return GetCell(content, PDFStyle.NormalFont, background);
		}

		public static PdfPCell GetCell(string content, Font font, BaseColor background = null)
		{
			return GetCell(content, font, Element.ALIGN_LEFT, background);
		}

		public static PdfPCell GetCell(string content, Font font, int horizontalAlignment, BaseColor background = null)
		{
			return new PdfPCell(new Phrase(content, font))
			{
				BackgroundColor = background,
				HorizontalAlignment = horizontalAlignment,
			};
		}

		public static PdfPCell GetImageTextCell(string imageSource, string text, float partial, BaseColor background = null)
		{
			var image = GetImage(imageSource);
			var nestedTable = new PdfPTable(2);
			nestedTable.SetWidths(new float[] { 1f, partial });
			nestedTable.DefaultCell.Border = 0;
			if (image == null)
				nestedTable.AddCell("");
			else
				nestedTable.AddCell(image);
			nestedTable.AddCell(new Phrase(text, PDFStyle.NormalFont));

			return new PdfPCell(nestedTable)
			{
				Padding = 0,
				BackgroundColor = background
			};
		}

		public static PdfPTable CreateTable(Document doc, int columnCount)
		{
			var table = new PdfPTable(columnCount);
			//table.SplitLate = false;
			table.TotalWidth = doc.PageSize.Width - doc.LeftMargin - doc.RightMargin;
			table.LockedWidth = true;
			table.HorizontalAlignment = 1;
			table.SpacingBefore = 20f;
			table.SpacingAfter = 20f;
			return table;
		}

		public static void PrintTable(PdfPTable table, DataTable dataTable)
		{
			foreach (DataRow row in dataTable.Rows)
				for (int i = 0; i < table.NumberOfColumns; i++)
					table.AddCell(GetCell(row[i].ToString(), PDFStyle.TextFont));
		}
	}
}