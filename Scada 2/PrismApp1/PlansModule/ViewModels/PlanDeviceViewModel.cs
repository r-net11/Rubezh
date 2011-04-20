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
    public class PlanDeviceViewModel : BaseViewModel
    {
        Rectangle borderRectangle;
        PlanDevice planDevice;

        public void Initialize(PlanDevice planDevice, Canvas canvas)
        {
            this.planDevice = planDevice;

            Canvas innerCanvas = new Canvas();
            Canvas.SetLeft(innerCanvas, planDevice.Left);
            Canvas.SetTop(innerCanvas, planDevice.Top);

            innerCanvas.ToolTip = "Устройство";

            canvas.Children.Add(innerCanvas);

            Rectangle deviceRectangle = new Rectangle();
            deviceRectangle.Width = 20;
            deviceRectangle.Height = 20;
            deviceRectangle.Fill = Brushes.Blue;
            innerCanvas.Children.Add(deviceRectangle);

            Polyline polyline = new Polyline();
            polyline.Points.Add(new System.Windows.Point(11, 2));
            polyline.Points.Add(new System.Windows.Point(7, 11));
            polyline.Points.Add(new System.Windows.Point(13, 8));
            polyline.Points.Add(new System.Windows.Point(8, 18));
            polyline.Stroke = Brushes.Red;
            polyline.StrokeThickness = 1;
            polyline.StrokeLineJoin = PenLineJoin.Round;
            innerCanvas.Children.Add(polyline);

            borderRectangle = new Rectangle();
            borderRectangle.Width = 20;
            borderRectangle.Height = 20;
            innerCanvas.Children.Add(borderRectangle);

            innerCanvas.MouseEnter += new System.Windows.Input.MouseEventHandler(innerCanvas_MouseEnter);
            innerCanvas.MouseLeave += new System.Windows.Input.MouseEventHandler(innerCanvas_MouseLeave);
            innerCanvas.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(innerCanvas_PreviewMouseLeftButtonDown);
        }

        void innerCanvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            borderRectangle.Stroke = Brushes.Orange;
            borderRectangle.StrokeThickness = 1;
        }

        void innerCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            borderRectangle.StrokeThickness = 0;
        }

        void innerCanvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ServiceFactory.Events.GetEvent<PlanDeviceSelectedEvent>().Publish(planDevice.Path);
        }
    }
}
