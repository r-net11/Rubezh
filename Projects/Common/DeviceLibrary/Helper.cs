using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace DeviceLibrary
{
    public static class Helper
    {
        public static void Draw(ICollection<Canvas> stateCanvases, ref Canvas canvas, string image, int layer)
        {
            if (stateCanvases == null) throw new ArgumentNullException("stateCanvases");
            stateCanvases.Remove(canvas);
            var stringReader = new StringReader(image);
            var xmlReader = XmlReader.Create(stringReader);
            canvas = (Canvas)XamlReader.Load(xmlReader);
            Panel.SetZIndex(canvas, layer);
            stateCanvases.Add(canvas);
        }

        public static Canvas Str2Canvas(string image, int layer)
        {
            var canvas = new Canvas();
            try
            {
                var stringReader = new StringReader(image);
                var xmlReader = XmlReader.Create(stringReader);
                canvas = (Canvas)XamlReader.Load(xmlReader);
                Panel.SetZIndex(canvas, layer);
            }catch{}

            return canvas;
        }
    }
}
