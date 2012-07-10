using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Designer;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Infrastructure;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Media;

namespace PlansModule.Designer.Adorners
{
	public class ResizeChromeShape : ResizeChrome
	{
		static ResizeChromeShape()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeChromeShape), new FrameworkPropertyMetadata(typeof(ResizeChromeShape)));
		}

		public ResizeChromeShape(DesignerItem designerItem)
			: base(designerItem)
		{
			Loaded += new RoutedEventHandler(ResizeChromeShape_Loaded);
		}

		private void ResizeChromeShape_Loaded(object sender, RoutedEventArgs e)
		{
			Initialize();
		}

		public override void Initialize()
		{
			//if (IsInitialized)
			//{
			//    Canvas canvas = Template.FindName("canvas", this) as Canvas;
			//    canvas.Children.Clear();
			//    ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			//    Rect rect = DesignerItem.Element.GetRectangle();
			//    if (element != null)
			//        foreach (var point in element.Points)
			//        {
			//            var thumb = new ResizeThumb()
			//            {
			//                Focusable = true,
			//                DataContext = this,
			//                IsHitTestVisible = true
			//            };
			//            Canvas.SetLeft(thumb, point.X - rect.X - 3.5 / DesignerCanvas.Zoom);
			//            Canvas.SetTop(thumb, point.Y - rect.Y - 3.5 / DesignerCanvas.Zoom);
			//            canvas.Children.Add(thumb);
			//        }
			//}
		}

		protected override void Resize(ResizeDirection direction, Vector vector)
		{
			ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
			if (element != null)
			{
				var rect = element.GetRectangle();
				var placeholder = new Rect(rect.TopLeft, rect.Size);
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
				{
					placeholder.Y += vector.Y;
					placeholder.Height -= vector.Y;
				}
				else if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
					placeholder.Height += vector.Y;
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
				{
					placeholder.X += vector.X;
					placeholder.Width -= vector.X;
				}
				else if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
					placeholder.Width += vector.X;
				double kx = placeholder.Width / rect.Width;
				double ky = placeholder.Height / rect.Height;

				PointCollection points = new PointCollection();
				foreach (var point in element.Points)
					points.Add(new Point(placeholder.X + kx * (point.X - rect.X), placeholder.Y + ky * (point.Y - rect.Y)));
				element.Points = points;

				DesignerItem.Redraw();
				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
	}
}
