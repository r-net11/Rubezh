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

namespace Resources
{
    public static class Functions
    {      

        /// <summary>
        /// Метод преобразующий svg-строку в Canvas c рисунком.
        /// </summary>
        /// <param name="svgString">svg-строка</param>
        /// <returns>Canvas, полученный из svg-строки</returns>
        public static Canvas Str2Canvas(string svgString, int layer)
        {
            string frameImage = Svg2Xaml(svgString, References.svg2xaml_xsl);
            StringReader stringReader = new StringReader(frameImage);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Canvas Picture = (Canvas)XamlReader.Load(xmlReader);
            Canvas.SetZIndex(Picture, layer);
            return (Picture);
        }

        /// <summary>
        /// Purpose   Transform an XML document with XSLT.
        /// Entry     sFileName_xml - The XML datafile.
        ///           sFileName_xsl - The XSL style sheet.
        ///           sFileName_out - The result of the transformation.
        /// Return    An empty string if successful, else an error description.
        /// Comments  The output file is directly saved to disk.
        /// </summary>
        public static string Svg2Xaml(string input,
                                      string sFileName_xsl)
        {
            try
            {

                XsltSettings settings = new XsltSettings(true, true);
                string output;
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(sFileName_xsl, settings, new XmlUrlResolver());
                StringReader stringReader = new StringReader(input);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriter xmlWriter = XmlWriter.Create(stringBuilder);
                xslt.Transform(xmlReader, xmlWriter);
                output = stringBuilder.ToString();
                return output;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }    
    }
}
