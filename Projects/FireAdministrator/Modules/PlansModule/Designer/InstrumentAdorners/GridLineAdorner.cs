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

namespace PlansModule.InstrumentAdorners
{
	public class GridLineAdorner : InstrumentAdorner
	{
		public Orientation Orientation { get; private set; }
		private DesignerCanvas Canvas
		{
			get { return (DesignerCanvas)DesignerCanvas; }
		}

		public GridLineAdorner(DesignerCanvas designerCanvas, Orientation orientation)
			: base(designerCanvas)
		{
			Orientation = orientation;
		}

		protected override void Show()
		{
			Canvas.GridLinePresenter.IsVisible = false;
			AdornerCanvas.Cursor = Cursors.Pen;
			foreach (var gridLine in Canvas.GridLinePresenter.GridLines)
			{
				var line = new Line();
				line.StrokeThickness = PainterCache.GridLinePen.Thickness;
				line.Stroke = PainterCache.GridLinePen.Brush;
				switch (gridLine.Orientation)
				{
					case Orientation.Horizontal:
						var widthBinding = new Binding("ActualWidth");
						widthBinding.Source = AdornerCanvas;
						line.SetBinding(Line.X2Property, widthBinding);
						System.Windows.Controls.Canvas.SetTop(line, gridLine.Position);
						break;
					case Orientation.Vertical:
						var heightBinding = new Binding("ActualHeight");
						heightBinding.Source = AdornerCanvas;
						line.SetBinding(Line.Y2Property, heightBinding);
						System.Windows.Controls.Canvas.SetLeft(line, gridLine.Position);
						break;
				}
				AdornerCanvas.Children.Add(line);
			}
		}
		public override void Hide()
		{
			base.Hide();
			Cleanup();
			Canvas.GridLinePresenter.IsVisible = true;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			DesignerCanvas.DeselectAll();
			//if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			//{
			//    if (!AdornerCanvas.Children.Contains(rubberband))
			//        AdornerCanvas.Children.Add(rubberband);
			//    if (!AdornerCanvas.IsMouseCaptured)
			//        AdornerCanvas.CaptureMouse();
			//    _opened = true;
			//}
			//if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
			//    ClosePolygon();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (StartPoint.HasValue)
			{
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
				var point = e.GetPosition(this);
				//if (Points.Count >= 2 && (Keyboard.Modifiers & ModifierKeys.Shift) > 0)
				//    point = GeometryHelper.TranslatePoint(Points.Count > 2 ? Points[Points.Count - 3] : new Point(Points[Points.Count - 2].X, -100), Points[Points.Count - 2], point);
				//Points[Points.Count - 1] = CutPoint(point);
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			//if (e.ButtonState == MouseButtonState.Released && _opened)
			//    switch (e.ChangedButton)
			//    {
			//        case MouseButton.Left:
			//            Points.Add(CutPoint(e.GetPosition(this)));
			//            if (Points.Count == 1)
			//                Points.Add(CutPoint(e.GetPosition(this)));
			//            break;
			//        case MouseButton.Right:
			//            ElementBaseShape element = CreateElement();
			//            if (element != null)
			//            {
			//                if (Points.Count > 1)
			//                    element.Points = new PointCollection(Points);
			//                else
			//                    element.Position = CutPoint(e.GetPosition(this));
			//                DesignerCanvas.CreateDesignerItem(element);
			//            }
			//            Cleanup();
			//            break;
			//    }
		}

		public override void UpdateZoom()
		{
			//base.UpdateZoom();
			//rubberband.StrokeThickness = 1 / ZoomFactor;
		}
		private void Cleanup()
		{
			AdornerCanvas.ReleaseMouseCapture();
			AdornerCanvas.Children.Clear();
		}
	}
}