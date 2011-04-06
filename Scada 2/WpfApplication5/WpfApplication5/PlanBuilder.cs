using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfApplication5
{
    public static class PlanBuilder
    {
        public static Plan Build()
        {
            Plan rootPlan = new Plan(0, 0, 0, 0, Brushes.Red, "root", "Объект", false);
            rootPlan.Parent = null;

            Plan plan_1 = new Plan(50, 100, 150, 100, Brushes.MediumSlateBlue, "building_1", "Строение 1", false);
            Plan plan_2 = new Plan(300, 100, 100, 150, Brushes.Blue, "building_2", "Строение 1", false);
            Plan plan_3 = new Plan(50, 300, 100, 150, Brushes.Red, "building_3", "Строение 1", false);

            rootPlan.Children.Add(plan_1);
            rootPlan.Children.Add(plan_2);
            rootPlan.Children.Add(plan_3);
            plan_1.Parent = rootPlan;
            plan_2.Parent = rootPlan;
            plan_3.Parent = rootPlan;

            Plan plan_1_1 = new Plan(10, 10, 400, 70, Brushes.SlateGray, "building_1_1", "Этаж 1", true);
            Plan plan_1_2 = new Plan(10, 100, 400, 70, Brushes.LightSeaGreen, "building_1_2", "Этаж 2", true);
            Plan plan_1_3 = new Plan(10, 200, 400, 70, Brushes.Honeydew, "building_1_3", "Этаж 3", true);
            plan_1.Children.Add(plan_1_1);
            plan_1.Children.Add(plan_1_2);
            plan_1.Children.Add(plan_1_3);
            plan_1_1.Parent = plan_1;
            plan_1_2.Parent = plan_1;
            plan_1_3.Parent = plan_1;

            Plan plan_2_1 = new Plan(10, 10, 400, 70, Brushes.LightYellow, "building_2_1", "Этаж 1", true);
            Plan plan_2_2 = new Plan(10, 100, 400, 70, Brushes.Linen, "building_2_2", "Этаж 2", true);
            Plan plan_2_3 = new Plan(10, 200, 400, 70, Brushes.Green, "building_2_3", "Этаж 3", true);
            plan_2.Children.Add(plan_2_1);
            plan_2.Children.Add(plan_2_2);
            plan_2.Children.Add(plan_2_3);
            plan_2_1.Parent = plan_2;
            plan_2_2.Parent = plan_2;
            plan_2_3.Parent = plan_2;

            Plan plan_3_1 = new Plan(10, 10, 400, 70, Brushes.Maroon, "building_3_1", "Этаж 1", true);
            Plan plan_3_2 = new Plan(10, 100, 400, 70, Brushes.MediumOrchid, "building_3_2", "Этаж 2", true);
            Plan plan_3_3 = new Plan(10, 200, 400, 70, Brushes.HotPink, "building_3_3", "Этаж 3", true);
            plan_3.Children.Add(plan_3_1);
            plan_3.Children.Add(plan_3_2);
            plan_3.Children.Add(plan_3_3);
            plan_3_1.Parent = plan_3;
            plan_3_2.Parent = plan_3;
            plan_3_3.Parent = plan_3;

            return rootPlan;
        }
    }
}
