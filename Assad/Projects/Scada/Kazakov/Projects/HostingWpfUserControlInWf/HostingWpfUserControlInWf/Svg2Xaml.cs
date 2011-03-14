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
    public partial class Svg2Xaml
    {

        public string sPath;
        public string sFile_xml;       // SVG file.
        public string sFile_xsl;       // svg2xaml.xsl
        public string sFile_out;       // XAML file.

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
        public string XSLT_Transform(string sFileName_xml,
                                      string sFileName_xsl,
                                      string sFileName_out)
        {
            try
            {
                // Enable support for XSLT 'document()' function.
                XsltSettings settings = new XsltSettings(true, true);
                // Alternative:
                // settings.EnableDocumentFunction = true;     

                // Load the style sheet.
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(sFileName_xsl, settings, new XmlUrlResolver());
                // Execute the transform and output the results to a file.
                xslt.Transform(sFileName_xml, sFileName_out);

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        // Not tested, not used.
        // Allow DTD in source file.
        public string XSLT_Transform_2(string sFileName_xml,
                                        string sFileName_xsl,
                                        string sFileName_out)
        {
            try
            {
                // Enable support for XSLT 'document()' function.
                XsltSettings set1 = new XsltSettings(true, true);

                // Allow DTD.
                XmlReaderSettings set2 = new XmlReaderSettings();
                set2.ProhibitDtd = false;

                // Load the style sheet.
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(XmlReader.Create(sFileName_xml, set2), set1, new XmlUrlResolver());

                // Execute the transform and output the results to a file.
                xslt.Transform(sFileName_xml, sFileName_out);

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        // SVG to XAML. Uses svg2xaml.xsl + colors.xml.
        // Test XSLT_Transform().
        public void btnTransform_Click(object sender, EventArgs e)
        {

            sFile_xsl = sPath + @"\svg2xaml.xsl";

            // Test 2. OK 030409
            sFile_xml = sPath + @"\Image_01.svg";     // Or *.xml
            sFile_out = sPath + @"\Image_01.xaml";

            /*
            // Test 2. OK 030409
            sFile_xml = sPath + @"\Image_02.svg";
            sFile_out = sPath + @"\Image_02.xaml";
            */
            /*            
            // Test 3. OK 030409
            sFile_xml = sPath + @"\Butterfly.svg";
            sFile_out = sPath + @"\Butterfly.xaml";
            */
            /*   
            // Test 4. OK 030409
            sFile_xml = sPath + @"\Lion.svg";
            sFile_out = sPath + @"\Lion.xaml";
            */
            /* 
            // Test 5. OK 030409
            sFile_xml = sPath + @"\Tiger.svg";
            sFile_out = sPath + @"\Tiger.xaml";
            */
            /* 
            // Test 6. OK 030409
            sFile_xml = sPath + @"\Coloredtoucan2.svg";
            sFile_out = sPath + @"\Coloredtoucan2.xaml";
            */
            /* 
            // Test 7. OK 030409
            sFile_xml = sPath + @"\GreenFrond.svg";
            sFile_out = sPath + @"\GreenFrond.xaml";
            */
            /* 
            // Test 8. OK 030409
            sFile_xml = sPath + @"\Drawing3D_02.svg";
            sFile_out = sPath + @"\Drawing3D_02.xaml";
            */
            /* 
            // Test 8. OK 030409
            sFile_xml = sPath + @"\TestText_02.svg";
            sFile_out = sPath + @"\TestText_02.xaml";
            */

            string sRet = XSLT_Transform(sFile_xml, sFile_xsl, sFile_out);
            // string sRet = XSLT_Transform_2(sFile_xml, sFile_xsl, sFile_out);

            /*
            if (sRet.Length == 0)
            {
                if (File.Exists(sFile_out) == true)
                {
                    txtBox.Text = File.ReadAllText(sFile_out);
                }
                else
                {
                    txtBox.Text = "Output file not found.";
                }
            }
            else
            {
                // Show error description.
                txtBox.Text = sRet;
            }
            */
        }


        // XAML to SVG.
        // Uses xaml2svg.xsl + \xaml2svg files.
        // |-> Add xmlns="http://www.w3.org/2000/svg" declaration in root !
        public void button1_Click(object sender, EventArgs e)
        {
            sFile_xsl = sPath + @"\xaml2svg.xsl";


            // Test 1. 060409. Too big. Transform?
            sFile_xml = sPath + @"\Page_06.xaml";
            sFile_out = sPath + @"\Page_06.svg";


            /*
            // Test 2. 060409. 
            // Problem with <GeometryGroup FillRule="EvenOdd">.
            sFile_xml = sPath + @"\Page_10.xaml";
            sFile_out = sPath + @"\Page_10.svg";
            */
            /*
            // Test 3. OK 060409
            sFile_xml = sPath + @"\Page_11.xaml";
            sFile_out = sPath + @"\Page_11.svg";
            */

            /*
            // Test 4. 060409. Too big. Transform?
            sFile_xml = sPath + @"\Page_19.xaml";
            sFile_out = sPath + @"\Page_19.svg";
            */

            /*
            // Test 5. 060409. No support for <Canvas.Resources>?
            sFile_xml = sPath + @"\Page_20.xaml";
            sFile_out = sPath + @"\Page_20.svg";
            */


            string sRet = XSLT_Transform(sFile_xml, sFile_xsl, sFile_out);
            // string sRet = XSLT_Transform_2(sFile_xml, sFile_xsl, sFile_out);

            /*
            if (sRet.Length == 0)
            {
                if (File.Exists(sFile_out) == true)
                {
                    txtBox.Text = File.ReadAllText(sFile_out);
                }
                else
                {
                    txtBox.Text = "Output file not found.";
                }
            }
            else
            {
                // Show error description.
                txtBox.Text = sRet;
            }
            */
        }



    }   // end class
}       // end namespace

