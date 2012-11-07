using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Infrastructure;
using PlansModule.Events;

namespace PlansModule
{
	public partial class ElementXZoneView : ContentControl
	{
		public ElementXZoneView()
		{
			InitializeComponent();
		}

		void _polygon_MouseEnter(object sender, MouseEventArgs e)
		{
			_polygon.StrokeThickness = 1;
		}

		void _polygon_MouseLeave(object sender, MouseEventArgs e)
		{
			_polygon.StrokeThickness = 0;
		}

		private void _polygon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ServiceFactory.Events.GetEvent<ElementXZoneSelectedEvent>().Publish(null);
			_polygon.StrokeThickness = 1;
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

			_grid.Children.Add(_flushEllipse);
			_flushEllipse.BeginAnimation(Ellipse.MarginProperty, thicknessAnimation);
		}

		void animation_Completed(object sender, EventArgs e)
		{
			_grid.Children.Remove(_flushEllipse);
		}
	}
}