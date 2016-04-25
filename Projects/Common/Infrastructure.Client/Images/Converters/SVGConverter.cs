using Common;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Xsl;

namespace Infrastructure.Client.Converters
{
	public static class SVGConverters
	{
		static readonly string XslFilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration\\XslFiles");
		static readonly string Svg2XamlXslFile = Path.Combine(XslFilesDirectory, "svg2xaml.xsl");
		static readonly string Xaml2SvgXslFile = Path.Combine(XslFilesDirectory, "xaml2svg.xsl");

		public static string Svg2Xaml(string svgFileName)
		{
			try
			{
				if (File.Exists(svgFileName) == false || File.Exists(Svg2XamlXslFile) == false)
					return null;

				var xslt = new XslCompiledTransform();
				var settings = new XsltSettings(true, true);
				xslt.Load(Svg2XamlXslFile, settings, new XmlUrlResolver());

				var xamlOfSvg = new StringBuilder();
				var xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
				xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
				using (var xmlReader = XmlReader.Create(svgFileName, xmlReaderSettings))
				using (var xmlWriter = XmlWriter.Create(xamlOfSvg))
				{
					xslt.Transform(xmlReader, xmlWriter);
					return xamlOfSvg.ToString();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ImageConverters.Svg2Xaml");
				return null;
			}
		}
		public static string Svg2Xaml2(string svgFileName)
		{
			string result = null;
			try
			{
				using (var memoryStream = new MemoryStream())
				{
					XamlWriter.Save(XamlTune.ConvertUtility.LoadSvg(svgFileName), memoryStream);
					byte[] buffer = memoryStream.GetBuffer();
					result = Encoding.UTF8.GetString(buffer);
					var lastIndex = result.LastIndexOf("</Canvas>");
					result = result.Substring(0, lastIndex + "</Canvas>".Length);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ImageConverters.Svg2Xaml2");
			}
			return result;
		}
		public static void Xaml2Svg(string xaml, string svgFileName)
		{
			if (CheckXaml2SvgFiles() == false)
				return;

			var xslt = new XslCompiledTransform();
			var settings = new XsltSettings(true, true);
			xslt.Load(Xaml2SvgXslFile, settings, new XmlUrlResolver());

			using (var xmlReader = XmlReader.Create(new StringReader(xaml)))
			using (var xmlWriter = XmlWriter.Create(svgFileName))
			{
				xslt.Transform(xmlReader, xmlWriter);
			}
		}
		public static Canvas Xml2Canvas(string xmlOfimage)
		{
			try
			{
				using (var stringReader = new StringReader(xmlOfimage))
				{
					var xmlReader = XmlReader.Create(stringReader);
					var canvas = (Canvas)XamlReader.Load(xmlReader);
					return canvas;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ImageConverters.Xml2Canvas");
				return null;
			}
		}

		static bool CheckXaml2SvgFiles()
		{
			var xaml2svgDirectory = Path.Combine(XslFilesDirectory, "xaml2svg");

			if (File.Exists(Xaml2SvgXslFile) == false ||
				Directory.Exists(xaml2svgDirectory) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "animation.xsl")) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "brushes.xsl")) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "canvas.xsl")) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "geometry.xsl")) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "properties-MODIF.xsl")) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "properties.xsl")) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "shapes.xsl")) == false ||
				File.Exists(Path.Combine(xaml2svgDirectory, "transform.xsl")) == false)
				return false;
			return true;
		}

		public static DrawingGroup ReadDrawing(string svgFileName)
		{
			var settings = new WpfDrawingSettings()
			{
				IncludeRuntime = false,
				TextAsGeometry = true,
				OptimizePath = true,
			};
			using (FileSvgReader reader = new FileSvgReader(settings))
				return reader.Read(svgFileName);
		}
	}
}
