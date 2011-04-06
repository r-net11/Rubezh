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
            Plan rootPlan = new Plan(0, 0, 0, 0, Brushes.Red, "root");
            rootPlan.Parent = null;

            Plan plan_1 = new Plan(100, 50, 50, 70, Brushes.Black, "building_1");
            Plan plan_2 = new Plan(240, 80, 30, 80, Brushes.Blue, "building_2");
            Plan plan_3 = new Plan(300, 200, 100, 50, Brushes.Red, "building_3");

            rootPlan.Children.Add(plan_1);
            rootPlan.Children.Add(plan_2);
            rootPlan.Children.Add(plan_3);
            plan_1.Parent = rootPlan;
            plan_2.Parent = rootPlan;
            plan_3.Parent = rootPlan;

            Plan plan_1_1 = new Plan(10, 15, 50, 46, Brushes.SlateGray, "building_1_1");
            Plan plan_1_2 = new Plan(200, 340, 300, 15, Brushes.Tan, "building_1_2");
            Plan plan_1_3 = new Plan(80, 10, 100, 100, Brushes.White, "building_1_3");
            plan_1.Children.Add(plan_1_1);
            plan_1.Children.Add(plan_1_2);
            plan_1.Children.Add(plan_1_3);
            plan_1_1.Parent = plan_1;
            plan_1_2.Parent = plan_1;
            plan_1_3.Parent = plan_1;

            Plan plan_2_1 = new Plan(184, 50, 50, 81, Brushes.SlateGray, "building_2_1");
            Plan plan_2_2 = new Plan(648, 290, 30, 79, Brushes.Tan, "building_2_2");
            Plan plan_2_3 = new Plan(150, 10, 54, 50, Brushes.White, "building_2_3");
            plan_2.Children.Add(plan_2_1);
            plan_2.Children.Add(plan_2_2);
            plan_2.Children.Add(plan_2_3);
            plan_2_1.Parent = plan_2;
            plan_2_2.Parent = plan_2;
            plan_2_3.Parent = plan_2;

            Plan plan_3_1 = new Plan(10, 50, 50, 46, Brushes.SlateGray, "building_3_1");
            Plan plan_3_2 = new Plan(240, 80, 300, 80, Brushes.Tan, "building_3_2");
            Plan plan_3_3 = new Plan(150, 10, 100, 50, Brushes.White, "building_3_3");
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
