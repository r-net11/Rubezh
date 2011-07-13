using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace LibraryModule
{
    public static class SvgConverter
    {      
        public static string Svg2Xaml(string input, string sFileNameXsl)
        {
            try
            {
                var settings = new XsltSettings(true, true);
                var xslt = new XslCompiledTransform();
                xslt.Load(sFileNameXsl, settings, new XmlUrlResolver());
                var stringReader = new StringReader(input);
                var xmlReader = XmlReader.Create(stringReader);
                var stringBuilder = new StringBuilder();
                var xmlWriter = XmlWriter.Create(stringBuilder);
                xslt.Transform(xmlReader, xmlWriter);
                var output = stringBuilder.ToString();
                return output;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }    
    }
}
