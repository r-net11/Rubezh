using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Infrastructure;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public partial class ElementDeviceView : UserControl
    {
        public ElementDeviceView()
        {
            InitializeComponent();
        }

        void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 1;
        }

        void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseOverRectangle.StrokeThickness = 0;
        }

        private void _deviceControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ServiceFactory.Events.GetEvent<ElementDeviceSelectedEvent>().Publish(null);
            _selectationRectangle.StrokeThickness = 1;
        }

        public void Flush()
        {
            var thicknessAnimationUsingKeyFrames = new ThicknessAnimationUsingKeyFrames();
            thicknessAnimationUsingKeyFrames.KeyFrames = new ThicknessKeyFrameCollection();

            var thicknessAnimation = new ThicknessAnimation()
            {
                From = new Thickness(0),
                To = new Thickness(-500),
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            thicknessAnimation.Completed += new EventHandler(animation_Completed);

            _flushEllipse.Visibility = Visibility.Visible;
            _flushEllipse.BeginAnimation(Ellipse.MarginProperty, thicknessAnimation);
        }

        void animation_Completed(object sender, EventArgs e)
        {
            var thicknessAnimation = new ThicknessAnimation()
            {
                From = new Thickness(-500),
                To = new Thickness(0),
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            thicknessAnimation.Completed += new EventHandler(animation_Completed2);
            _flushEllipse.BeginAnimation(Ellipse.MarginProperty, thicknessAnimation);
        }

        void animation_Completed2(object sender, EventArgs e)
        {
            _flushEllipse.Visibility = Visibility.Collapsed;
        }
    }
}