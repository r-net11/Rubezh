using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class ZonePlanViewModel : BaseViewModel
    {
        Polygon zonePolygon;

        public void Initialize(PlanZone planZone, Canvas canvas)
        {
            zonePolygon = new Polygon();
            foreach (PolygonPoint polygonPoint in planZone.PolygonPoints)
            {
                zonePolygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            zonePolygon.Fill = Brushes.Transparent;

            zonePolygon.ToolTip = "Зона " + planZone.ZoneNo;

            zonePolygon.MouseEnter += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseEnter);
            zonePolygon.MouseLeave += new System.Windows.Input.MouseEventHandler(zonePolygon_MouseLeave);
            zonePolygon.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(zonePolygon_PreviewMouseLeftButtonDown);

            canvas.Children.Add(zonePolygon);
        }

        void zonePolygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.Stroke = Brushes.Orange;
            zonePolygon.StrokeThickness = 1;
            zonePolygon.Fill = Brushes.Green;
            zonePolygon.Opacity = 0.5;
        }

        void zonePolygon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            zonePolygon.StrokeThickness = 0;
            zonePolygon.Opacity = 0.0;
        }

        void zonePolygon_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ServiceFactory.Events.GetEvent<PlanZoneSelectedEvent>().Publish("Zone");
        }
    }
}
