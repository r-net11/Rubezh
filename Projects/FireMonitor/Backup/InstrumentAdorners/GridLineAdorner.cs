using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class GridLineAdorner : InstrumentAdorner
	{
		public Orientation Orientation { get; private set; }
		private GridLineShape _gridLineShape;
		private double _thickness;
		public double CanvasWidth
		{
			get { return DesignerCanvas.CanvasWidth; }
		}
		public double CanvasHeight
		{
			get { return DesignerCanvas.CanvasHeight; }
		}

		public GridLineAdorner(DesignerCanvas designerCanvas, Orientation orientation)
			: base(designerCanvas)
		{
			Orientation = orientation;
		}

		protected override void Show()
		{
			_thickness = 1 / ZoomFactor;
			Cleanup();
			DesignerCanvas.GridLineController.IsVisible = false;
			foreach (var gridLine in DesignerCanvas.GridLineController.GridLines)
				if (gridLine.IsInside(CanvasWidth, CanvasHeight))
					AdornerCanvas.Children.Add(new GridLineShape(gridLine, _thickness) { DataContext = this });
		}
		public override void Hide()
		{
			base.Hide();
			Cleanup();
			DesignerCanvas.GridLineController.Invalidate();
			DesignerCanvas.GridLineController.IsVisible = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			DesignerCanvas.DeselectAll();
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DesignerCanvas.DeselectAll();
				StartPoint = e.GetPosition(this);
				_gridLineShape = new GridLineShape(new GridLine(Orientation, StartPoint.Value), _thickness);
				_gridLineShape.DataContext = this;
				switch (Orientation)
				{
					case Orientation.Horizontal:
						AdornerCanvas.Cursor = Cursors.SizeNS;
						break;
					case Orientation.Vertical:
						AdornerCanvas.Cursor = Cursors.SizeWE;
						break;
				}
				AdornerCanvas.Children.Add(_gridLineShape);
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_gridLineShape != null)
			{
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
				var point = e.GetPosition(this);
				_gridLineShape.UpdatePosition(point);
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (e.ButtonState == MouseButtonState.Released && e.ChangedButton == MouseButton.Left && _gridLineShape != null)
			{
				DesignerCanvas.GridLineController.GridLines.Add(_gridLineShape.GridLine);
				Cleanup();
			}
		}

		public override void UpdateZoom()
		{
			base.UpdateZoom();
			_thickness = 1 / ZoomFactor;
			foreach (var shape in AdornerCanvas.Children.OfType<GridLineShape>())
				shape.UpdateZoom(_thickness);
		}
		private void Cleanup()
		{
			_gridLineShape = null;
			AdornerCanvas.ReleaseMouseCapture();
			AdornerCanvas.Cursor = Cursors.Pen;
		}
		public void Remove(GridLineShape gridLineShape)
		{
			DesignerCanvas.GridLineController.GridLines.Remove(gridLineShape.GridLine);
			AdornerCanvas.Children.Remove(gridLineShape);
		}
	}
}