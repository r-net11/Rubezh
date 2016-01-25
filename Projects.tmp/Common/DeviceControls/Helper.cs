﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Common;

namespace DeviceControls
{
	internal static class Helper
	{
		static readonly XamlReader xamlReader = new XamlReader();

		public static Canvas Xml2Canvas(string xmlOfimage)
		{
			try
			{
				using (var stringReader = new StringReader(xmlOfimage))
				using (var xmlReader = XmlReader.Create(stringReader))
				{
					var canvas = (Canvas)xamlReader.LoadAsync(xmlReader);
					return canvas;
				}
			}
			catch
			{
				Logger.Error("Ошибка при вызове метода Xml2Canvas. xmlOfimage= " + xmlOfimage);
				return new Canvas();
			}
		}
		public static FrameworkElement GetVisual(string xaml)
		{
			Canvas canvas = Xml2Canvas(xaml);
			canvas.SnapsToDevicePixels = false;
			canvas.Background = Brushes.Transparent;
			return canvas;
		}
	}
}