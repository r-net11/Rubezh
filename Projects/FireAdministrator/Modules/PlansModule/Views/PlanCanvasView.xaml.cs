using System.Windows;
using System.Windows.Controls;
using FiresecClient;
using System.Windows.Media;
using System.Collections.Generic;
using FiresecAPI.Models;
using System.Windows.Shapes;

namespace PlansModule.Views
{
    public partial class PlanCanvasView : UserControl
    {
        public static PlanCanvasView Current { get; set; }
        public PlanCanvasView()
        {
            Plans = FiresecManager.PlansConfiguration.Plans;
            Current = this;
            InitializeComponent();


        }
        List<Plan> Plans;
        public void ChangeSelectedPlan(Plan plan)
        {
            _Canvas.Children.Clear();
            foreach (var zona in plan.ElementZones)
            {
                Polygon myPolygon = new Polygon();
                myPolygon.Stroke = System.Windows.Media.Brushes.Black;
                myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
                myPolygon.StrokeThickness = 2;
                PointCollection myPointCollection = new PointCollection();
                var point = new Point();
                foreach (var _point in zona.PolygonPoints)
                {
                    point.X = _point.X;
                    point.Y = _point.Y;
                    myPointCollection.Add(point);
                }
                myPolygon.Points = myPointCollection;
                _Canvas.Children.Add(myPolygon);
            }
        }


        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /*
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.Red;
            _Canvas.Background = brush;
            */

            foreach (var plan in Plans)
            {
                foreach (var zona in plan.ElementZones)
                {
                    Polygon myPolygon = new Polygon();
                    myPolygon.Stroke = System.Windows.Media.Brushes.Black;
                    myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
                    myPolygon.StrokeThickness = 2;
                    PointCollection myPointCollection = new PointCollection();
                    var point = new Point();
                    foreach (var _point in zona.PolygonPoints)
                    {
                        point.X = _point.X;
                        point.Y = _point.Y;
                        myPointCollection.Add(point);
                    }
                    myPolygon.Points = myPointCollection;
                    _Canvas.Children.Add(myPolygon);
                }
            }
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {


        }
    }
}