using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace DeviceControls
{
    public static class Helper
    {
        public static Canvas Xml2Canvas(string xmlOfimage, int layer)
        {
            var canvas = new Canvas();
            try
            {
                using (var stringReader = new StringReader(xmlOfimage))
                {
                    var xmlReader = XmlReader.Create(stringReader);
                    canvas = (Canvas) XamlReader.Load(xmlReader);
                    Panel.SetZIndex(canvas, layer);
                }
            }
            catch { }
            return canvas;
        }
    }
}