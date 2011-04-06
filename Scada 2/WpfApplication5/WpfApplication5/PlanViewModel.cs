using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace WpfApplication5
{
    public class PlanViewModel : BaseViewModel
    {
        public PlanViewModel()
        {
            Children = new ObservableCollection<PlanViewModel>();
        }

        public PlanViewModel Parent { get; set; }
        public ObservableCollection<PlanViewModel> Children { get; set; }

        public Plan plan { get; set; }

        public string Name { get; set; }

        public void Initialize(Plan plan)
        {
            this.plan = plan;
            Name = plan.Name;
        }

        public void DrawPlan()
        {
            Canvas MainCanvas = MainViewModel.Current.MainCanvas;
            MainCanvas.Children.Clear();

            foreach (Plan childPlan in plan.Children)
            {
                Rectangle rectanglePlan = new Rectangle();
                rectanglePlan.Name = childPlan.Name;
                rectanglePlan.MouseLeftButtonDown += new MouseButtonEventHandler(rectanglePlan_MouseLeftButtonDown);
                rectanglePlan.Width = childPlan.Width;
                rectanglePlan.Height = childPlan.Height;
                rectanglePlan.Fill = childPlan.Brush;
                rectanglePlan.RadiusX = 10;
                rectanglePlan.RadiusY = 10;
                Canvas.SetLeft(rectanglePlan, childPlan.Left);
                Canvas.SetTop(rectanglePlan, childPlan.Top);
                MainCanvas.Children.Add(rectanglePlan);
            }

            //ParentPlans = new ObservableCollection<Plan>();
            //string stringPath = "";
            //List<Plan> parentPlans = plan.AllParents;
            //foreach (Plan _plan in parentPlans)
            //{
            //    ParentPlans.Add(_plan);
            //    stringPath += _plan.Name + "/";
            //}
            //if (stringPath.EndsWith("/"))
            //{
            //}
            //path.Text = stringPath;
        }

        void rectanglePlan_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as Rectangle).Name;
            if (e.ClickCount == 2)
            {
                if (Children.Count > 0)
                {
                    PlanViewModel SelectedPlanViewModel = Children.FirstOrDefault(x => x.Name == name);
                    MainViewModel.Current.SelectedPlanViewModel = SelectedPlanViewModel;
                    SelectedPlanViewModel.DrawPlan();
                }
            }
        }
    }
}
