﻿using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using FiresecClient;
using ItvIntergation.Ngi;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Controls;

namespace RepFileManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool result = FiresecManager.Connect("adm", "");
            if (result == false)
            {
                MessageBox.Show("Не удается соединиться с сервером");
                return;
            }

            var repositoryModule = new repositoryModule();
            repositoryModule.name = "Устройства Рубеж";
            repositoryModule.version = "1.0.0";
            repositoryModule.port = "1234";
            var repository = new repository();
            repository.module = repositoryModule;

            var devices = new List<repositoryModuleDevice>();
            foreach (var driver in FiresecManager.Drivers)
            {
                var repDevice = new RepDevice();
                repDevice.Initialize(driver);
                devices.Add(repDevice.Device);
            }

            repositoryModule.device = devices.ToArray();

            var serializer = new XmlSerializer(typeof(repositoryModule));
            using (var fileStream = File.CreateText("Rubezh.rep"))
            {
                serializer.Serialize(fileStream, repositoryModule);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = new Button();
            button.Width = 100;
            button.Height = 100;
            button.Content = "Hello";
            button.SnapsToDevicePixels = false;

            //this.AddLogicalChild(button);
            SaveWindowSnapshot(button, "xxx.jpg");

            SaveWindowSnapshot(this, "!!!__!!!__MyWindowSnapshot.jpg");

            // add Button name "button1" before
            SaveWindowSnapshot(button1, "!!!__!!!__MyButtonSnapshot.jpg");
        }

        private void SaveWindowSnapshot(Visual targetVisual, string fileName)
        {
            //Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            //double myDeviceDpiX = m.M11 * 96.0;
            //double myDeviceDpiY = m.M22 * 96.0;

            double myDeviceDpiX = 100 * 96.0;
            double myDeviceDpiY = 100 * 96.0;

            var imgStream = GrabSnapshotStream(targetVisual, myDeviceDpiX, myDeviceDpiY, ImageFormats.JPG);
            using (imgStream)
            {
                imgStream.Position = 0;

                var fileStream = new FileStream(@"c:/" + fileName, FileMode.OpenOrCreate);
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
