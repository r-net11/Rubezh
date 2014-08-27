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
using CodeReason.Reports.Providers;

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
			var provider = new TableProvider();
			return provider.Clone(rowGroup, true);
		}
		public static TableRow SimpleClone(TableRow row)
		{
			var provider = new TableProvider();
			return provider.Clone(row, true);
		}

	}
}
