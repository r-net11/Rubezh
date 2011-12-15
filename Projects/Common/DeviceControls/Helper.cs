using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace DeviceControls
{
    public static class Helper
    {
        static readonly XamlReader xamlReader = new XamlReader();

        public static Canvas Xml2Canvas(string xmlOfimage, int layer)
        {
            try
            {
                using (var stringReader = new StringReader(xmlOfimage))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    var canvas = (Canvas) xamlReader.LoadAsync(xmlReader);
                    Panel.SetZIndex(canvas, layer);

                    return canvas;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}