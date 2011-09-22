using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Xml;
using System.Windows.Markup;

namespace RepFileManager
{
    public static class Helper
    {
        public static Canvas Xml2Canvas(string xmlOfimage)
        {
            var canvas = new Canvas();
            try
            {
                using (var stringReader = new StringReader(xmlOfimage))
                {
                    var xmlReader = XmlReader.Create(stringReader);
                    canvas = (Canvas)XamlReader.Load(xmlReader);
                    //Panel.SetZIndex(canvas, layer);
                }
            }
            catch { }
            return canvas;
        }
    }
}
