using System.Windows.Shapes;
using FiresecAPI.Models;
using PlansModule.Designer;
using System.Windows.Controls;
using Infrustructure.Plans.InstrumentAdorners;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using Infrustructure.Plans.Painters;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Linq;
using Infrustructure.Plans.Designer;
using System;

namespace PlansModule.InstrumentAdorners
{
	public class GridLineAdorner : InstrumentAdorner
	{
		public Orientation Orientation { get; private set; }
		private DesignerCanvas Canvas
		{
			get { return (DesignerCanvas)DesignerCanvas; }
		}
		private GridLineShape _gridLineShape;

		public GridLineAdorner(DesignerCanvas designerCanvas, Orientation orientation)
			: base(designerCanvas)
		{
			Orientation = orientation;
		}

		protected override void Show()
		{
			Cleanup();
			Canvas.GridLinePresenter.IsVisible = false;
			foreach (var gridLine in Canvas.GridLinePresenter.GridLines)
				AdornerCanvas.Children.Add(new GridLineShape(gridLine, ZoomFactor) { DataContext = this });
		}
		public override void Hide()
		{
			base.Hide();
			Cleanup();
			Canvas.GridLinePresenter.Invalidate();
			Canvas.GridLinePresenter.IsVisible = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			DesignerCanvas.DeselectAll();
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DesignerCanvas.DeselectAll();
				StartPoint = e.GetPosition(this);
				_gridLineShape = new GridLineShape(new GridLine(Orientation, StartPoint.Value), ZoomFactor);
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
				Canvas.GridLinePresenter.GridLines.Add(_gridLineShape.GridLine);
				Cleanup();
			}
		}

		public override void UpdateZoom()
		{
			base.UpdateZoom();
			foreach (var shape in AdornerCanvas.Children.OfType<GridLineShape>())
				shape.UpdateZoom(ZoomFactor);
		}
		private void Cleanup()
		{
			_gridLineShape = null;
			AdornerCanvas.ReleaseMouseCapture();
			AdornerCanvas.Cursor = Cursors.Pen;
		}
		public void Remove(GridLineShape gridLineShape)
		{
			Canvas.GridLinePresenter.GridLines.Remove(gridLineShape.GridLine);
			AdornerCanvas.Children.Remove(gridLineShape);
		}
	}
}