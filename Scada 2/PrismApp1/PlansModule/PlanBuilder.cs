using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace PlansModule
{
    public static class PlanLoader
    {
        public static Plan Load()
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Plan));
                StreamReader reader = new StreamReader("D:/Plans.xml");
                Plan plan = (Plan)deserializer.Deserialize(reader);
                reader.Close();
                return plan;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static void CreateDemoData()
        {
            Plan plan = new Plan();
            plan.Name = "rootPlan";
            plan.BackgroundSource = "D:/picture6.jpg";
            plan.Caption = "cap";
            plan.Height = 500;
            plan.Width = 500;
            plan.SubPlans = new List<SubPlan>();
            plan.SubPlans.Add(new SubPlan());
            plan.SubPlans[0].Name = "subPlan1";
            plan.SubPlans[0].PolygonPoints = new List<PolygonPoint>();
            plan.SubPlans[0].BackgroundSource = "D:/picture1.jpg";
            plan.SubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 100 });
            plan.SubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 200 });
            plan.SubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 200 });
            plan.SubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 100 });
            plan.PlanZones = new List<PlanZone>();
            plan.PlanZones.Add(new PlanZone());
            plan.PlanZones[0].ZoneNo = "0";
            plan.PlanZones[0].PolygonPoints = new List<PolygonPoint>();
            plan.PlanZones[0].PolygonPoints.Add(new PolygonPoint() { X = 300, Y = 300 });
            plan.PlanZones[0].PolygonPoints.Add(new PolygonPoint() { X = 300, Y = 400 });
            plan.PlanZones[0].PolygonPoints.Add(new PolygonPoint() { X = 400, Y = 400 });
            plan.PlanZones[0].PolygonPoints.Add(new PolygonPoint() { X = 400, Y = 300 });
            plan.PlanDevices = new List<PlanDevice>();
            plan.PlanDevices.Add(new PlanDevice());
            plan.PlanDevices[0].Driver = "driver";
            plan.PlanDevices[0].Left = 100;
            plan.PlanDevices[0].Top = 200;
            plan.Children = new List<Plan>();
            plan.Children.Add(new Plan());
            plan.Children[0].Name = "subPlan1";
            plan.Children[0].Caption = "Plan 2";

            XmlSerializer serializer = new XmlSerializer(typeof(Plan));
            StreamWriter writer = new StreamWriter("D:/Plans.xml");
            serializer.Serialize(writer, plan);
            writer.Close();
        }
    }
}
