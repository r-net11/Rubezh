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
using PlansModule.Models;

namespace PlansModule.ViewModels
{
    public class ElementSubPlanViewModel : BaseViewModel
    {
        public void Initialize(ElementSubPlan elementSubPlan, Canvas canvas)
        {
            Polygon subPlanPolygon = new Polygon();
            subPlanPolygon.Name = elementSubPlan.Name;
            subPlanPolygon.ToolTip = elementSubPlan.Caption;
            subPlanPolygon.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(subPlanPolygon_PreviewMouseLeftButtonDown);
            subPlanPolygon.MouseEnter += new MouseEventHandler(subPlanPolygon_MouseEnter);
            subPlanPolygon.MouseLeave += new MouseEventHandler(subPlanPolygon_MouseLeave);
            foreach (PolygonPoint polygonPoint in elementSubPlan.PolygonPoints)
            {
                subPlanPolygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            if (elementSubPlan.ShowBackgroundImage)
            {
                ImageBrush polygonImageBrush = new ImageBrush();
                polygonImageBrush.ImageSource = new BitmapImage(new Uri(elementSubPlan.BackgroundSource, UriKind.Absolute));
                subPlanPolygon.Fill = polygonImageBrush;
            }
            else
            {
                subPlanPolygon.Fill = Brushes.Transparent;
            }
            subPlanPolygon.Stroke = new SolidColorBrush(Colors.Blue);
            canvas.Children.Add(subPlanPolygon);
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
