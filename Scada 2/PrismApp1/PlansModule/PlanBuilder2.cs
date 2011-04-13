using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace PlansModule
{
    public static class PlanBuilder2
    {
        public static void Build()
        {
            Plan2 plan2 = new Plan2();
            plan2.BackgroundImageName = "qwe";
            plan2.Caption = "cap";
            plan2.Height = 400;
            plan2.Width = 400;
            plan2.SubPlans = new List<SubPlan>();
            plan2.SubPlans.Add(new SubPlan());
            plan2.SubPlans[0].PolygonPoints = new List<PolygonPoint>();
            plan2.SubPlans[0].BackgroundImageName = "bk";
            plan2.SubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 100 });
            plan2.SubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 200 });
            plan2.PlanZones = new List<PlanZone>();
            plan2.PlanZones.Add(new PlanZone());
            plan2.PlanZones[0].ZoneNo = "0";
            plan2.PlanZones[0].PolygonPoints = new List<PolygonPoint>();
            plan2.PlanZones[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 100 });
            plan2.PlanZones[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 200 });
            plan2.PlanDevices = new List<PlanDevice>();
            plan2.PlanDevices.Add(new PlanDevice());
            plan2.PlanDevices[0].Driver = "driver";
            plan2.PlanDevices[0].Left = 100;
            plan2.PlanDevices[0].Top = 200;
            plan2.Children = new List<Plan2>();
            plan2.Children.Add(new Plan2());
            plan2.Children[0].Caption = "Plan 2";

            XmlSerializer serializer = new XmlSerializer(typeof(Plan2));
            StreamWriter writer = new StreamWriter("D:/xxx.xml");
            serializer.Serialize(writer, plan2);
            writer.Close();
        }
    }
}
