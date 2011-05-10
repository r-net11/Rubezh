using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace DeviceLibrary
{
    public static class Functions
    {
        public static void Draw(ICollection<Canvas> stateCanvases, ref Canvas canvas, string image, int layer)
        {
            if (stateCanvases == null) throw new ArgumentNullException("stateCanvases");
            stateCanvases.Remove(canvas);
            SvgToCanvas(ref canvas, image);
            Panel.SetZIndex(canvas, layer);
            stateCanvases.Add(canvas);
        }

        public static void SvgToCanvas(ref Canvas canvas, string svg)
        {
            if (canvas == null) throw new ArgumentNullException("canvas");
            var frameImage = svg;
            var stringReader = new StringReader(frameImage);
            var xmlReader = XmlReader.Create(stringReader);
            canvas = (Canvas)XamlReader.Load(xmlReader);
        }
    }
}
