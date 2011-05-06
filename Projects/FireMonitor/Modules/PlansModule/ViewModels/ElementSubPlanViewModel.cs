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
using Firesec;

namespace PlansModule.ViewModels
{
    public class ElementSubPlanViewModel : BaseViewModel
    {
        Polygon _subPlanPolygon;
        public string Name { get; private set; }

        public void Initialize(ElementSubPlan elementSubPlan, Canvas canvas)
        {
            Name = elementSubPlan.Name;
            _subPlanPolygon = new Polygon();
            _subPlanPolygon.Opacity = 0.6;
            _subPlanPolygon.Stroke = Brushes.Blue;
            _subPlanPolygon.StrokeThickness = 1;
            _subPlanPolygon.ToolTip = elementSubPlan.Caption;
            _subPlanPolygon.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(subPlanPolygon_PreviewMouseLeftButtonDown);
            _subPlanPolygon.MouseEnter += new MouseEventHandler(subPlanPolygon_MouseEnter);
            _subPlanPolygon.MouseLeave += new MouseEventHandler(subPlanPolygon_MouseLeave);
            foreach (PolygonPoint polygonPoint in elementSubPlan.PolygonPoints)
            {
                _subPlanPolygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }
            if (elementSubPlan.ShowBackgroundImage)
            {
                ImageBrush polygonImageBrush = new ImageBrush();
                polygonImageBrush.ImageSource = new BitmapImage(new Uri(elementSubPlan.BackgroundSource, UriKind.Absolute));
                _subPlanPolygon.Fill = polygonImageBrush;
            }
            else
            {
                _subPlanPolygon.Fill = Brushes.Transparent;
            }
            _subPlanPolygon.Stroke = new SolidColorBrush(Colors.Blue);
            canvas.Children.Add(_subPlanPolygon);
        }

        void subPlanPolygon_MouseEnter(object sender, MouseEventArgs e)
        {
            _subPlanPolygon.Stroke = Brushes.Orange;
        }

        void subPlanPolygon_MouseLeave(object sender, MouseEventArgs e)
        {
            _subPlanPolygon.Stroke = Brushes.Blue;
        }

        void subPlanPolygon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(Name);
            }
        }

        public void Update(string state)
        {
            StateType stateType = StateHelper.NameToType(state);
            switch (stateType)
            {
                case StateType.Alarm:
                    _subPlanPolygon.Fill = Brushes.Red;
                    break;

                case StateType.Failure:
                    _subPlanPolygon.Fill = Brushes.Red;
                    break;

                case StateType.Info:
                    _subPlanPolygon.Fill = Brushes.YellowGreen;
                    break;

                case StateType.No:
                    _subPlanPolygon.Fill = Brushes.Transparent;
                    break;

                case StateType.Norm:
                    _subPlanPolygon.Fill = Brushes.Green;
                    break;

                case StateType.Off:
                    _subPlanPolygon.Fill = Brushes.Red;
                    break;

                case StateType.Service:
                    _subPlanPolygon.Fill = Brushes.Yellow;
                    break;

                case StateType.Unknown:
                    _subPlanPolygon.Fill = Brushes.Gray;
                    break;

                case StateType.Warning:
                    _subPlanPolygon.Fill = Brushes.Yellow;
                    break;
            }
        }
    }
}
