using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Designer;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infrustructure.Plans.Presenter
{
	public class BorderPainter
	{
		//protected double Thickness { get { return 3 / PresenterItem.DesignerCanvas.Zoom; } }
		//protected double ResizeMargin { get { return 3 / PresenterItem.DesignerCanvas.Zoom; } }

		private ScaleTransform _transform;
		private RectangleGeometry _geometry;
		private Pen _pen;
		private PresenterItem _presenterItem;
		private double _margin;

		public BorderPainter()
		{
			var brush = new SolidColorBrush(Colors.Orange);
			brush.Freeze();
			_margin = 3;
			_pen = new Pen(brush, 3);
			_geometry = new RectangleGeometry();
			_transform = new ScaleTransform(0, 0);
		}
		public void Render(DrawingContext drawingContext)
		{
			drawingContext.PushTransform(_transform);
			drawingContext.DrawGeometry(null, _pen, _geometry);
			drawingContext.Pop();
		}
		public void UpdateZoom(double zoom)
		{
			_pen.Thickness = 3 / zoom;
			_margin = 3 / zoom;
			UpdateBounds();
		}
		public void Hide()
		{
			_presenterItem = null;
			_transform.ScaleX = 0;
			_transform.ScaleY = 0;
		}
		public void Show(PresenterItem presenterItem)
		{
			_presenterItem = presenterItem;
			UpdateBounds();
			_transform.ScaleX = 1;
			_transform.ScaleY = 1;
		}

		private void UpdateBounds()
		{
			if (_presenterItem != null)
			{
				var rect = _presenterItem.Painter.Bounds;
				_geometry.Rect = new Rect(rect.Left - _margin, rect.Top - _margin, rect.Width + 2 * _margin, rect.Height + 2 * _margin);
			}
		}
	}
}
