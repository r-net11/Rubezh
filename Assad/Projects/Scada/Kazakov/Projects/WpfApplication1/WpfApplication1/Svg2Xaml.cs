using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

namespace WpfApplication1
{
    public static class Svg2Xaml
    {

        // ---------------------------------------------------------------
        // Date      030409
        // Purpose   Transform an XML document with XSLT.
        // Entry     sFileName_xml - The XML datafile.
        //           sFileName_xsl - The XSL style sheet.
        //           sFileName_out - The result of the transformation.
        // Return    An empty string if successful, else an error description.
        // Comments  The output file is directly saved to disk.
        //           Source:
        //           D:\PC3000_Info_Base\My_Website\Website_NET2971_016_2
        //             \Standards\Standard_XSLT.htm. Modified.
        // ---------------------------------------------------------------
        public static string XSLT_Transform(string input,
                                      string sFileName_xsl)
        {
            try
            {
                // Enable support for XSLT 'document()' function.
                XsltSettings settings = new XsltSettings(true, true);
                // Alternative:
                // settings.EnableDocumentFunction = true;     

                // Load the style sheet.
                string output;
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(sFileName_xsl, settings, new XmlUrlResolver());
                // Execute the transform and output the results to a file.

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

    }   // end class
}       // end namespace

