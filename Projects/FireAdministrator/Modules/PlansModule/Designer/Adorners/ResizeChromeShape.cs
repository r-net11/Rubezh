using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;

namespace PlansModule.Designer.Adorners
{
	public class ResizeChromeShape : ResizeChrome
	{
		private bool _isDragging = false;
		private List<ResizeThumb> _thumbs;
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
			if (IsInitialized && !_isDragging)
			{
				Canvas canvas = Template.FindName("canvas", this) as Canvas;
				if (canvas != null)
				{
					canvas.Children.Clear();
					_thumbs = new List<ResizeThumb>();
					ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
					Rect rect = DesignerItem.Element.GetRectangle();
					if (element != null)
						foreach (var point in element.Points)
						{
							var thumb = new ResizeThumb()
							{
								Direction = ResizeDirection.None,
								DataContext = this,
								IsHitTestVisible = true,
								Cursor = Cursors.Pen,
							};
							thumb.SetBinding(ResizeThumb.MarginProperty, new Binding("PointMargin"));
							thumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
							thumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
							thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
							Canvas.SetLeft(thumb, point.X - rect.X + element.BorderThickness / 2);
							Canvas.SetTop(thumb, point.Y - rect.Y + element.BorderThickness / 2);
							canvas.Children.Add(thumb);
							_thumbs.Add(thumb);
						}
				}
			}
		}
		private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			_isDragging = true;
		}
		private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			_isDragging = false;
			Initialize();
		}
		private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (DesignerItem.IsSelected)
			{
				int index = _thumbs.IndexOf((ResizeThumb)sender);
				if (index > -1)
				{
					ElementBaseShape element = DesignerItem.Element as ElementBaseShape;
					double x = element.Points[index].X + e.HorizontalChange;
					if (x < 0)
						x = 0;
					else if (x > DesignerCanvas.Width)
						x = DesignerCanvas.Width;
					double y = element.Points[index].Y + e.VerticalChange;
					if (y < 0)
						y = 0;
					else if (y > DesignerCanvas.Height)
						y = DesignerCanvas.Height;
					element.Points[index] = new Point(x, y);
					DesignerItem.Redraw();
					Rect rect = element.GetRectangle();
					for (int i = 0; i < _thumbs.Count; i++)
					{
						Canvas.SetLeft(_thumbs[i], element.Points[i].X - rect.X + element.BorderThickness / 2);
						Canvas.SetTop(_thumbs[i], element.Points[i].Y - rect.Y + element.BorderThickness / 2);
					}
					ServiceFactory.SaveService.PlansChanged = true;
					e.Handled = true;
				}
			}
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
				double kx = rect.Width == 0 ? 0 : placeholder.Width / rect.Width;
				double ky = rect.Height == 0 ? 0 : placeholder.Height / rect.Height;

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
