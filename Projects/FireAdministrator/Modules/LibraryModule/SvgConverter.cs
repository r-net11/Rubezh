using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace LibraryModule
{
    public static class SvgConverter
    {
        public static string Svg2Xaml(string svgFileName, string xslFileName)
        {
            if (File.Exists(svgFileName) == false ||
                File.Exists(xslFileName) == false) return null;

            var xslt = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings(true, true);
            xslt.Load(xslFileName, settings, new XmlUrlResolver());

            var xamlFromSvgString = new StringBuilder();
            var xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
            using (var xmlReader = XmlReader.Create(svgFileName, xmlReaderSettings))
            using (var xmlWriter = XmlWriter.Create(xamlFromSvgString))
            {
                xslt.Transform(xmlReader, xmlWriter);
                return xamlFromSvgString.ToString();
            }
        }    
    }
}
