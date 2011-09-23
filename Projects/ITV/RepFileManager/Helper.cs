using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace RepFileManager
{
    public static class ImageHelper
    {
        public static Canvas XmlToCanvas(string xmlOfimage)
        {
            var canvas = new Canvas();
            try
            {
                using (var stringReader = new StringReader(xmlOfimage))
                {
                    var xmlReader = XmlReader.Create(stringReader);
                    canvas = (Canvas)XamlReader.Load(xmlReader);
                }
            }
            catch { }
            return canvas;
        }

        public static void XAMLToBitmap(FrameworkElement element, string sFileName_png)
        {
            try
            {
                element.Measure(new Size((int)element.Width, (int)element.Height));
                element.Arrange(new Rect(new Size((int)element.Width,
                                                   (int)element.Height)));

                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)element.ActualWidth,
                                         (int)element.ActualHeight, 96d, 96d,
                                         PixelFormats.Pbgra32);
                renderTargetBitmap.Render(element);

                using (FileStream fileStream = new FileStream(sFileName_png, FileMode.Create))
                {
                    var bmpBitmapEncoder = new BmpBitmapEncoder();
                    bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                    bmpBitmapEncoder.Save(fileStream);
                }
            }
            catch
            {
            }
        }
    }
}
