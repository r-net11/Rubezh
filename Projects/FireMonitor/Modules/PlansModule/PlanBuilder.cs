using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;


namespace PlansModule
{
    public static class PlanLoader
    {
        public static Plan Load()
        {
            try
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                string ыек = PathHelper.Plans;
                FileStream fs = new FileStream(PathHelper.Plans, FileMode.Open);
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, XmlDictionaryReaderQuotas.Max);
                Plan plan = (Plan)dcs.ReadObject(reader);
                reader.Close();
                /*
                Uri uri = new Uri(PathHelper.Data + plan.BackgroundSource);
                byte[] imageInfo = File.ReadAllBytes(uri.AbsolutePath);
                plan.Pixels = imageInfo;

                ConvertPathToArray(plan.Children);

                DataContractSerializer dcs_out = new DataContractSerializer(typeof(Plan));
                FileStream fs_out = new FileStream(@"D:/del/Plans_new240811.xml", FileMode.Create);
                XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs_out);
                dcs.WriteObject(xdw, plan);
                xdw.Close();
                */
                return plan;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        static void ConvertPathToArray(List<Plan> Plans)
        {
            foreach (Plan plan in Plans)
            {
                if (plan.BackgroundSource != null)
                {
                    Uri uri = new Uri(PathHelper.Data + plan.BackgroundSource);
                    byte[] imageInfo = File.ReadAllBytes(uri.AbsolutePath);
                    plan.BackgroundPixels = imageInfo;
                }
                if (plan.Children != null)
                {
                    ConvertPathToArray(plan.Children);
                }
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
            //plan.ElementSubPlans[0].PolygonPoints = new List<PolygonPoint>();
            plan.ElementSubPlans[0].PolygonPoints = new PointCollection();
            plan.ElementSubPlans[0].BackgroundSource = "D:/picture1.jpg";
            plan.ElementSubPlans[0].PolygonPoints.Add(new Point() { X = 100, Y = 100 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new Point() { X = 100, Y = 200 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new Point() { X = 200, Y = 200 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new Point() { X = 200, Y = 100 });
            plan.ElementZones = new List<ElementZone>();
            plan.ElementZones.Add(new ElementZone());
            plan.ElementZones[0].ZoneNo = "0";
            //plan.ElementZones[0].PolygonPoints = new List<PolygonPoint>();
            plan.ElementZones[0].PolygonPoints = new PointCollection();
            plan.ElementZones[0].PolygonPoints.Add(new Point() { X = 300, Y = 300 });
            plan.ElementZones[0].PolygonPoints.Add(new Point() { X = 300, Y = 400 });
            plan.ElementZones[0].PolygonPoints.Add(new Point() { X = 400, Y = 400 });
            plan.ElementZones[0].PolygonPoints.Add(new Point() { X = 400, Y = 300 });
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