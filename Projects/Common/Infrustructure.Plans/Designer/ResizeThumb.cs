using System;
using System.Windows.Controls.Primitives;

namespace Infrustructure.Plans.Designer
{
	public class ResizeThumb : Thumb
	{
		public ResizeDirection Direction { get; set; }
		public ResizeChrome ResizeChrome
		{
			get { return DataContext as ResizeChrome; }
		}
		private CommonDesignerCanvas DesignerCanvas
		{
			get { return ResizeChrome.DesignerCanvas; }
		}

		public ResizeThumb()
		{
			Direction = ResizeDirection.None;
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
			DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
		}

		private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			DesignerCanvas.BeginChange();
		}
		private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			DesignerCanvas.EndChange();
		}

		//private static Brush _brush;
		//static ResizeThumb()
		//{
		//    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeThumb), new FrameworkPropertyMetadata(typeof(ResizeThumb)));
		//    _brush = new RadialGradientBrush()
		//    {
		//        Center = new Point(0.3, 0.3),
		//        GradientOrigin = new Point(0.3, 0.3),
		//        RadiusX = 0.7,
		//        RadiusY = 0.7,
		//        GradientStops = new GradientStopCollection()
		//        {
		//            new GradientStop(Colors.White, 0),
		//            new GradientStop(Colors.DarkSlateGray, 0.9),
		//        }
		//    };
		//    _brush.Freeze();
		//}
		//private Rect _rect;
		//protected override void OnRender(DrawingContext drawingContext)
		//{
		//    drawingContext.DrawGeometry(_brush, null, new EllipseGeometry(_rect));
		//}
		//protected override Size ArrangeOverride(Size arrangeBounds)
		//{
		//    _rect = new Rect(0, 0, arrangeBounds.Width, arrangeBounds.Height);
		//    return base.ArrangeOverride(arrangeBounds);
		//}
	}
}
