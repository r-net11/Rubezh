using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;

namespace RepFileManager
{
    public class ImageConverter
    {
        public void SaveWindowSnapshot(Visual targetVisual, string fileName)
        {
            //Matrix m = PresentationSource.FromVisual(source).CompositionTarget.TransformToDevice;
            //double myDeviceDpiX = m.M11 * 96.0;
            //double myDeviceDpiY = m.M22 * 96.0;
            double myDeviceDpiX = 100 * 96.0;
            double myDeviceDpiY = 100 * 96.0;

            var imgStream = GrabSnapshotStream(targetVisual, myDeviceDpiX, myDeviceDpiY, ImageFormats.BMP);
            using (imgStream)
            {
                imgStream.Position = 0;

                var fileStream = new FileStream(@"D:/BMP" + fileName, FileMode.OpenOrCreate);
                using (fileStream)
                {
                    for (int i = 0; i < imgStream.Length; i++)
                    {
                        fileStream.WriteByte((byte)imgStream.ReadByte());
                    }
                }
            }
        }

        public static MemoryStream GrabSnapshotStream(Visual targetVisual, double dpiX, double dpiY, ImageFormats imageFormats)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(targetVisual);

            BitmapSource renderTargetBitmap = captureVisualBitmap(
                targetVisual,
                dpiX,
                dpiY
                );

            BitmapEncoder bitmapEncoder;

            switch (imageFormats)
            {
                case ImageFormats.PNG:
                    {
                        bitmapEncoder = new PngBitmapEncoder();
                        break;
                    }
                case ImageFormats.BMP:
                    {
                        bitmapEncoder = new BmpBitmapEncoder();
                        break;
                    }
                case ImageFormats.JPG:
                    {
                        bitmapEncoder = new JpegBitmapEncoder();
                        break;
                    }
                default:
                    throw new NotSupportedException("The Incorrect Logic");
            }

            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Create a MemoryStream with the image.
            // Returning this as a MemoryStream makes it easier to save the image to a file or simply display it anywhere.
            var memoryStream = new MemoryStream();
            bitmapEncoder.Save(memoryStream);

            return memoryStream;
        }


        private static BitmapSource captureVisualBitmap(Visual targetVisual, double dpiX, double dpiY)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(targetVisual);
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)(bounds.Width * dpiX / 96.0),
                (int)(bounds.Height * dpiY / 96.0),
                dpiX,
                dpiY,

                //PixelFormats.Default
                PixelFormats.Pbgra32
                );

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush visualBrush = new VisualBrush(targetVisual);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }
            renderTargetBitmap.Render(drawingVisual);

            return renderTargetBitmap;
        }

        public enum ImageFormats
        {
            PNG,
            BMP,
            JPG
        }
    }
}
