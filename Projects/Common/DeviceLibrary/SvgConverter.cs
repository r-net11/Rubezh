using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Xml.Xsl;
using System.Xml.Serialization;

namespace DeviceLibrary
{
    public static class SvgConverter
    {      
        /// <summary>
        /// Метод преобразующий svg-строку в Canvas c рисунком.
        /// </summary>
        /// <param name="svgString">svg-строка</param>
        /// <returns>Canvas, полученный из svg-строки</returns>
        //public static Canvas Str2Canvas(string svgString, int layer)
        //{
        //    var frameImage = Svg2Xaml(svgString, ResourceHelper.svg2xaml_xsl);
        //    var stringReader = new StringReader(frameImage);
        //    var xmlReader = XmlReader.Create(stringReader);
        //    var canvas = (Canvas)XamlReader.Load(xmlReader);
        //    Panel.SetZIndex(canvas, layer);
        //    return (canvas);
        //}

        /// <summary>
        /// Purpose   Transform an XML document with XSLT.
        /// Entry     sFileName_xml - The XML datafile.
        ///           sFileName_xsl - The XSL style sheet.
        ///           sFileName_out - The result of the transformation.
        /// Return    An empty string if successful, else an error description.
        /// Comments  The output file is directly saved to disk.
        /// </summary>
        public static string Svg2Xaml(string input, string sFileName_xsl)
        {
            try
            {
                var settings = new XsltSettings(true, true);
                var xslt = new XslCompiledTransform();
                xslt.Load(sFileName_xsl, settings, new XmlUrlResolver());
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
