using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Designer;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace PlansModule.Designer
{
	public class GridLinePresenter
	{
		private DesignerCanvas _canvas;
		private StreamGeometry _geometry;
		private bool _isVisible;
		public ObservableCollection<GridLine> GridLines { get; private set; }
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				if (_canvas != null)
					_canvas.Refresh();
			}
		}

		public GridLinePresenter(DesignerCanvas canvas)
		{
			IsVisible = true;
			_canvas = canvas;
			_geometry = new StreamGeometry();
			GridLines = new ObservableCollection<GridLine>();
			GridLines.CollectionChanged += (s, e) => Invalidate();
			Invalidate();
		}
		public void Invalidate()
		{
			_geometry = new StreamGeometry();
			using (StreamGeometryContext ctx = _geometry.Open())
				foreach (var gridLine in GridLines)
				{
					ctx.BeginFigure(gridLine.Orientation == Orientation.Horizontal ? new Point(0, gridLine.Position) : new Point(gridLine.Position, 0), false, false);
					ctx.LineTo(gridLine.Orientation == Orientation.Horizontal ? new Point(_canvas.Width, gridLine.Position) : new Point(gridLine.Position, _canvas.Height), true, false);
				}
			_geometry.Freeze();
			_canvas.Refresh();
		}
		public void Render(DrawingContext drawingContext)
		{
			if (IsVisible)
				drawingContext.DrawGeometry(null, PainterCache.GridLinePen, _geometry);
		}
	}
}
