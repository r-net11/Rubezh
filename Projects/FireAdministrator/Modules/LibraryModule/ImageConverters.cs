using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Xsl;

namespace LibraryModule
{
    public static class ImageConverters
    {
        public static string Svg2Xaml(string svgFileName, string xslFileName)
        {
            if (File.Exists(svgFileName) == false ||
                File.Exists(xslFileName) == false) return null;

            var xslt = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings(true, true);
            xslt.Load(xslFileName, settings, new XmlUrlResolver());

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

        public static Canvas Xml2Canvas(string xmlOfimage, int layer)
        {
            var canvas = new Canvas();
            try
            {
                using (var stringReader = new StringReader(xmlOfimage))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    canvas = (Canvas) XamlReader.Load(xmlReader);
                    Panel.SetZIndex(canvas, layer);
                }
            }
            catch { }
            return canvas;
        }
    }
}
