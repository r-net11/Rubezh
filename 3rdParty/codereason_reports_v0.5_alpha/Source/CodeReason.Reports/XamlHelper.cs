/************************************************************************
 * Copyright: Hans Wolff
 *
 * License:  This software abides by the LGPL license terms. For further
 *           licensing information please see the top level LICENSE.txt 
 *           file found in the root directory of CodeReason Reports.
 *
 * Author:   Hans Wolff
 *
 ************************************************************************/

using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Linq;
using CodeReason.Reports.Document;

namespace CodeReason.Reports
{
	/// <summary>
	/// Helper class for XAML
	/// </summary>
	public static class XamlHelper
	{
		/// <summary>
		/// Loads a XAML object from string
		/// </summary>
		/// <param name="s">string containing the XAML object</param>
		/// <returns>XAML object or null, if string was empty</returns>
		public static T LoadXamlFromString<T>(string s)
			where T : class
		{
			if (String.IsNullOrEmpty(s))
				return null;
			using (StringReader stringReader = new StringReader(s))
			using (XmlReader xmlReader = XmlTextReader.Create(stringReader, new XmlReaderSettings()))
				return (T)XamlReader.Load(xmlReader);
		}

		public static T Clone<T>(T orig)
			where T : class
		{
			if (orig == null)
				return null;
			using (var stream = new MemoryStream())
			{
				XamlWriter.Save(orig, stream);
				stream.Seek(0, SeekOrigin.Begin);
				return (T)XamlReader.Load(stream);
			}
		}

		/// <summary>
		/// Saves a visual to bitmap into stream
		/// </summary>
		/// <param name="visual">visual</param>
		/// <param name="stream">stream</param>
		/// <param name="width">width</param>
		/// <param name="height">height</param>
		/// <param name="dpiX">X DPI resolution</param>
		/// <param name="dpiY">Y DPI resolution</param>
		public static void SaveImageBmp(Visual visual, Stream stream, int width, int height, double dpiX, double dpiY)
		{
			RenderTargetBitmap bitmap = new RenderTargetBitmap((int)(width * dpiX / 96d), (int)(height * dpiY / 96d), dpiX, dpiY, PixelFormats.Pbgra32);
			bitmap.Render(visual);

			BmpBitmapEncoder image = new BmpBitmapEncoder();
			image.Frames.Add(BitmapFrame.Create(bitmap));
			image.Save(stream);
		}

		/// <summary>
		/// Saves a visual to PNG into stream
		/// </summary>
		/// <param name="visual">visual</param>
		/// <param name="stream">stream</param>
		/// <param name="width">width</param>
		/// <param name="height">height</param>
		/// <param name="dpiX">X DPI resolution</param>
		/// <param name="dpiY">Y DPI resolution</param>
		public static void SaveImagePng(Visual visual, Stream stream, int width, int height, double dpiX, double dpiY)
		{
			RenderTargetBitmap bitmap = new RenderTargetBitmap((int)(width * dpiX / 96d), (int)(height * dpiY / 96d), dpiX, dpiY, PixelFormats.Pbgra32);
			bitmap.Render(visual);

			PngBitmapEncoder image = new PngBitmapEncoder();
			image.Frames.Add(BitmapFrame.Create(bitmap));
			image.Save(stream);
		}

		public static TableRowGroup SimpleClone(TableRowGroup rowGroup)
		{
			var group = rowGroup is TableRowGroupForDataTable ? new TableRowGroupForDataTable() { TableName = ((TableRowGroupForDataTable)rowGroup).TableName } : new TableRowGroup();
			CopyProperties(rowGroup, group);
			foreach (var row in rowGroup.Rows)
				group.Rows.Add(SimpleClone(row));
			return group;
		}
		public static TableRow SimpleClone(TableRow row)
		{
			var newRow = row is TableRowForDataTable ? new TableRowForDataTable() { TableName = ((TableRowForDataTable)row).TableName } : new TableRow();
			CopyProperties(row, newRow);
			foreach (var cell in row.Cells)
				newRow.Cells.Add(SimpleClone(cell));
			return newRow;
		}
		private static TableCell SimpleClone(TableCell cell)
		{
			var newCell = new TableCell()
			{
				ColumnSpan = cell.ColumnSpan,
				RowSpan = cell.RowSpan,
				Padding = cell.Padding,
				BorderBrush = cell.BorderBrush,
				BorderThickness = cell.BorderThickness,
				FontSize = cell.FontSize,
				FontWeight = cell.FontWeight,
				TextAlignment = cell.TextAlignment,
			};
			foreach (var block in cell.Blocks)
				newCell.Blocks.Add(SimpleClone(block));
			CopyProperties(cell, newCell);
			return newCell;
		}
		private static Block SimpleClone(Block block)
		{
			var paragraph = block as Paragraph;
			if (paragraph != null)
			{
				var newParagraph = new Paragraph()
				{
					FontSize = paragraph.FontSize,
					FontWeight = paragraph.FontWeight,
					Padding = paragraph.Padding,
					Margin = paragraph.Margin,
					TextAlignment = paragraph.TextAlignment,
				};
				foreach (var inline in paragraph.Inlines)
					newParagraph.Inlines.Add(SimpleClone(inline));
				return newParagraph;
			}
			throw new NotSupportedException();
		}
		private static Inline SimpleClone(Inline inline)
		{
			Inline newInline = null;
			if (inline is InlineTableCellIndexValue)
			{
				var inlineValue = (InlineTableCellIndexValue)inline;
				newInline = new InlineTableCellIndexValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					Index = inlineValue.Index,
					Format = inlineValue.Format,
				};
			}
			else if (inline is InlineTableCellValue)
			{
				var inlineValue = (InlineTableCellValue)inline;
				newInline = new InlineTableCellValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					PropertyName = inlineValue.PropertyName,
					Format = inlineValue.Format,
				};
			}
			else if (inline is InlineAggregateValue)
			{
				var inlineValue = (InlineAggregateValue)inline;
				newInline = new InlineAggregateValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					EmptyValue = inlineValue.EmptyValue,
					ErrorValue = inlineValue.ErrorValue,
					Format = inlineValue.Format,
				};
			}
			else if (inline is InlineContextValue)
			{
				var inlineValue = (InlineContextValue)inline;
				newInline = new InlineContextValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					Type = inlineValue.Type,
					Format = inlineValue.Format,
				};
			}
			else if (inline is InlineDocumentValue)
			{
				var inlineValue = (InlineDocumentValue)inline;
				newInline = new InlineDocumentValue()
				{
					AggregateGroup = inlineValue.AggregateGroup,
					PropertyName = inlineValue.PropertyName,
					Format = inlineValue.Format,
				};
			}
			else if (inline is Run)
			{
				var run = (Run)inline;
				newInline = new Run()
				{
					Text = run.Text,
					FontSize = run.FontSize,
					FontWeight = run.FontWeight,
				};
			}
			else if (inline is LineBreak)
				newInline = new LineBreak();
			if (newInline != null)
			{
				newInline.FontSize = inline.FontSize;
				newInline.FontWeight = inline.FontWeight;
				return newInline;
			}
			throw new NotSupportedException();
		}
		private static void CopyProperties(TextElement source, TextElement target)
		{
		}
	}
}
