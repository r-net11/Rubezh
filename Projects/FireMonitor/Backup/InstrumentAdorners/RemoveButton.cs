using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class RemoveButton : FrameworkElement
	{
		public double ButtonSize { get { return 14; } }
		private static SolidColorBrush _tabItemCloseButtonHoverBackgroundBrush;
		private static Pen _tabItemCloseButtonHoverForegroundPen;
		private static SolidColorBrush _tabItemCloseButtonPressedBackgroundBrush;
		private static Pen _tabItemCloseButtonPressedForegroundPen;
		private static StreamGeometry _geometry;
		static RemoveButton()
		{
			_tabItemCloseButtonHoverBackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFc13535"));
			_tabItemCloseButtonPressedBackgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF431e20"));
			_tabItemCloseButtonPressedForegroundPen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFf9ebeb")), 2);
			_tabItemCloseButtonHoverForegroundPen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFf9ebeb")), 2);
			_geometry = new StreamGeometry();
			using (StreamGeometryContext context = _geometry.Open())
			{
				context.BeginFigure(new Point(-3, -3), true, false);
				context.LineTo(new Point(3, 3), true, false);
				context.BeginFigure(new Point(-3, 3), true, false);
				context.LineTo(new Point(3, -3), true, false);
				context.Close();
			}
			_tabItemCloseButtonHoverBackgroundBrush.Freeze();
			_tabItemCloseButtonPressedBackgroundBrush.Freeze();
			_tabItemCloseButtonPressedForegroundPen.Freeze();
			_tabItemCloseButtonHoverForegroundPen.Freeze();
			_geometry.Freeze();
		}

		public RemoveButton()
		{
			SnapsToDevicePixels = false;
			Cursor = Cursors.Hand;
			Focusable = false;
			Width = ButtonSize;
			Height = ButtonSize;
		}
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			drawingContext.DrawEllipse(IsMouseOver ? _tabItemCloseButtonPressedBackgroundBrush : _tabItemCloseButtonHoverBackgroundBrush, null, new Point(0, 0), 7, 7);
			drawingContext.DrawGeometry(null, IsMouseOver ? _tabItemCloseButtonPressedForegroundPen : _tabItemCloseButtonHoverForegroundPen, _geometry);
		}
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			InvalidateVisual();
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			InvalidateVisual();
		}
	}
}
