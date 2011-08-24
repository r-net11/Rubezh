using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Windows.Media.Imaging;

namespace PlansModule
{
    public static class PlanLoader
    {
        public static Plan Load()
        {
            try
            {
                /*
                var deserializer = new XmlSerializer(typeof(Plan));
                var reader = new StreamReader(PathHelper.Plans);
                Plan plan = (Plan)deserializer.Deserialize(reader);
                reader.Close();



                var xdw = XmlDictionaryWriter.CreateTextWriter(fs);
                FileStream fs = new FileStream(@"D:/del/Plans_new180811.xml", FileMode.Create);
                dcs.WriteObject(xdw, plan);
                xdw.Close();
                */


                DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                FileStream fs = new FileStream(PathHelper.Plans, FileMode.Open);
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, XmlDictionaryReaderQuotas.Max);
                Plan plan = (Plan)dcs.ReadObject(reader);
                reader.Close();
                /*
                
                List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
                colors.Add(System.Windows.Media.Colors.Red);
                colors.Add(System.Windows.Media.Colors.Blue);
                colors.Add(System.Windows.Media.Colors.Green);
                BitmapPalette myPalette = new BitmapPalette(colors);
                BitmapSource bitmap = new BitmapImage();
                
                bitmap = BitmapSource.Create(plan.pixelWidth, plan.pixelHeight, plan.dpiX, plan.dpiY,
                    plan.pixelFormat, plan.palette, plan.pixels, plan.stride);
                
                Uri uri = new Uri(PathHelper.Data+ plan.BackgroundSource);

                BitmapSource bitmap = new BitmapImage(uri);
                int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
                int pixelWidth = bitmap.PixelWidth;
                int pixelHeight = bitmap.PixelHeight;
                int stride = bytesPerPixel * pixelWidth;
                int pixelCount = pixelWidth * pixelHeight;
                var pixelBytes = new byte[pixelCount * bytesPerPixel];
                bitmap.CopyPixels(pixelBytes, stride,0);

                plan.pixelWidth = pixelWidth;
                plan.pixelHeight = pixelHeight;
                plan.dpiX = 96;
                plan.dpiY = 96;
                plan.pixelFormat = bitmap.Format;
                plan.palette = bitmap.Palette;
                plan.pixels = pixelBytes;
                plan.stride = stride;
                */
                DataContractSerializer dcs_out = new DataContractSerializer(typeof(Plan));
                FileStream fs_out= new FileStream(@"D:/del/Plans_new190811.xml", FileMode.Create);
                XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs_out);
                dcs.WriteObject(xdw, plan);
                xdw.Close();
                
                return plan;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        static void CreateDemoData()
        {
            var plan = new Plan();
            plan.Name = "rootPlan";
            plan.BackgroundSource = "D:/picture6.jpg";
            plan.Caption = "cap";
            plan.Height = 500;
            plan.Width = 500;
            plan.ElementSubPlans = new List<ElementSubPlan>();
            plan.ElementSubPlans.Add(new ElementSubPlan());
            plan.ElementSubPlans[0].Name = "subPlan1";
            plan.ElementSubPlans[0].PolygonPoints = new List<PolygonPoint>();
            plan.ElementSubPlans[0].BackgroundSource = "D:/picture1.jpg";
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 100 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 200 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 200 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 100 });
            plan.ElementZones = new List<ElementZone>();
            plan.ElementZones.Add(new ElementZone());
            plan.ElementZones[0].ZoneNo = "0";
            plan.ElementZones[0].PolygonPoints = new List<PolygonPoint>();
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 300, Y = 300 });
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 300, Y = 400 });
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 400, Y = 400 });
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 400, Y = 300 });
            plan.ElementDevices = new List<ElementDevice>();
            plan.ElementDevices.Add(new ElementDevice());
            plan.ElementDevices[0].Left = 100;
            plan.ElementDevices[0].Top = 200;
            plan.Children = new List<Plan>();
            plan.Children.Add(new Plan());
            plan.Children[0].Name = "subPlan1";
            plan.Children[0].Caption = "Plan 2";
            /*
                             XmlSerializer deserializer = new XmlSerializer(typeof(Plan));
                            StreamReader reader = new StreamReader(PathHelper.Plans);
                            Plan plan = (Plan)deserializer.Deserialize(reader);
                            reader.Close();

                            DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                            FileStream fs = new FileStream(@"D:/del/Plans_new.xml", FileMode.Create);
                            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs);
                            dcs.WriteObject(xdw, plan);
                            xdw.Close();
                 
                
                
                            DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                            FileStream fs = new FileStream(@"D:/del/Plans_new.xml", FileMode.Open);
                            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                            Plan plan = (Plan)dcs.ReadObject(reader);
                            reader.Close();

             * */

        }
    }
}