using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class SubPlanViewModel : BaseViewModel
    {
        public void Initialize(SubPlan subPlan, Canvas MainCanvas)
        {
            Polygon subPlanPolygon = new Polygon();
            subPlanPolygon.Name = subPlan.Name;
            subPlanPolygon.ToolTip = subPlan.Caption;
            subPlanPolygon.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(subPlanPolygon_PreviewMouseLeftButtonDown);
            subPlanPolygon.MouseEnter += new MouseEventHandler(subPlanPolygon_MouseEnter);
            subPlanPolygon.MouseLeave += new MouseEventHandler(subPlanPolygon_MouseLeave);
            foreach (PolygonPoint polygonPoint in subPlan.PolygonPoints)
            {
                subPlanPolygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            if (subPlan.ShowBackgroundImage)
            {
                ImageBrush polygonImageBrush = new ImageBrush();
                polygonImageBrush.ImageSource = new BitmapImage(new Uri(subPlan.BackgroundSource, UriKind.Absolute));
                subPlanPolygon.Fill = polygonImageBrush;
            }
            else
            {
                subPlanPolygon.Fill = Brushes.Transparent;
            }
            subPlanPolygon.Stroke = new SolidColorBrush(Colors.Blue);
            MainCanvas.Children.Add(subPlanPolygon);
        }

        void subPlanPolygon_MouseEnter(object sender, MouseEventArgs e)
        {
            Polygon subPlanPolygon = sender as Polygon;
            subPlanPolygon.Stroke = Brushes.Orange;
            subPlanPolygon.Fill = Brushes.Red;
            subPlanPolygon.Opacity = 0.5;
        }

        void subPlanPolygon_MouseLeave(object sender, MouseEventArgs e)
        {
            Polygon subPlanPolygon = sender as Polygon;
            subPlanPolygon.Stroke = Brushes.Blue;
            subPlanPolygon.Fill = Brushes.Transparent;
        }

        void subPlanPolygon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as Polygon).Name;
            if (e.ClickCount == 2)
            {
                ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(name);
            }
        }
    }
}
