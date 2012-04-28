using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Infrastructure;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
	public partial class ElementDeviceView : Grid
	{
		public ElementDeviceView()
		{
			InitializeComponent();
			MouseEnter += new MouseEventHandler(Grid_MouseEnter);
			MouseLeave += new MouseEventHandler(Grid_MouseLeave);
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

		Ellipse _flushEllipse;

		public void Flush()
		{
			var thicknessAnimationUsingKeyFrames = new ThicknessAnimationUsingKeyFrames();
			thicknessAnimationUsingKeyFrames.KeyFrames = new ThicknessKeyFrameCollection();

			double delta = (Width - Height) / 2;

			var thicknessAnimation = new ThicknessAnimation()
			{
				From = new Thickness(delta, 0, delta, 0),
				To = new Thickness(delta - 500, -500, delta - 500, -500),
				Duration = new Duration(TimeSpan.FromSeconds(1)),
				AutoReverse = true
			};
			thicknessAnimation.Completed += new EventHandler(animation_Completed);

			_flushEllipse = new Ellipse()
			{
				Fill = new SolidColorBrush(Colors.LightBlue),
				Stroke = new SolidColorBrush(Colors.Orange),
				StrokeThickness = 5,
				Opacity = 0.5
			};

			Children.Add(_flushEllipse);
			_flushEllipse.BeginAnimation(Ellipse.MarginProperty, thicknessAnimation);
		}

		void animation_Completed(object sender, EventArgs e)
		{
			Children.Remove(_flushEllipse);
		}
	}
}