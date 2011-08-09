using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace DeviceControls
{
    public static class Helper
    {
        public static Canvas Str2Canvas(string image, int layer)
        {
            var canvas = new Canvas();
            try
            {
                var stringReader = new StringReader(image);
                var xmlReader = XmlReader.Create(stringReader);
                canvas = (Canvas) XamlReader.Load(xmlReader);
                Panel.SetZIndex(canvas, layer);
            }
            catch { }
            return canvas;
        }
    }
}