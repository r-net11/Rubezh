using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using System.Diagnostics;
using System.Windows;

namespace Infrustructure.Plans.InstrumentAdorners
{
	public abstract class BasePolygonAdorner : InstrumentAdorner
	{
		private Shape rubberband;

		public BasePolygonAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected Shape Rubberband
		{
			get { return rubberband; }
		}

		protected override void Show()
		{
			rubberband = CreateRubberband();
			rubberband.Stroke = Brushes.Navy;
			rubberband.StrokeThickness = 1 / ZoomFactor;
			AdornerCanvas.Cursor = Cursors.Pen;
		}

		protected abstract Shape CreateRubberband();
		protected abstract PointCollection Points { get; }
		protected abstract ElementBaseShape CreateElement();

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			{
				if (!AdornerCanvas.Children.Contains(rubberband))
					AdornerCanvas.Children.Add(rubberband);
			}
			if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
				ClosePolygon();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Points.Count > 0)
			{
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
				var point = e.GetPosition(this);
				if (Points.Count > 2 && (Keyboard.Modifiers & ModifierKeys.Shift) > 0)
					point = GeometryHelper.TranslatePoint(Points[Points.Count - 3], Points[Points.Count - 2], point);
				Points[Points.Count - 1] = CutPoint(point);
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (e.ButtonState == MouseButtonState.Released)
				switch (e.ChangedButton)
				{
					case MouseButton.Left:
						Points.Add(CutPoint(e.GetPosition(this)));
						if (Points.Count == 1)
							Points.Add(CutPoint(e.GetPosition(this)));
						break;
					case MouseButton.Right:
						AdornerCanvas.Children.Remove(rubberband);
						ElementBaseShape element = CreateElement();
						if (element != null)
						{
							if (Points.Count > 1)
								element.Points = new PointCollection(Points);
							else
								element.Position = CutPoint(e.GetPosition(this));
							DesignerCanvas.CreateDesignerItem(element);
							Points.Clear();
						}
						AdornerCanvas.ReleaseMouseCapture();
						break;
				}
		}

		public override void KeyboardInput(Key key)
		{
			base.KeyboardInput(key);
			if (key == Key.Enter)
				ClosePolygon();
			else if (key == Key.RightShift || key == Key.LeftShift)
			{
				var args = new MouseEventArgs(Mouse.PrimaryDevice, 0);
				args.RoutedEvent = UIElement.MouseMoveEvent;
				RaiseEvent(args);
			}
		}
		private void ClosePolygon()
		{
			if (Points.Count > 2)
			{
				AdornerCanvas.ReleaseMouseCapture();
				AdornerCanvas.Children.Remove(rubberband);
				ElementBaseShape element = CreateElement();
				if (element != null)
				{
					Points.RemoveAt(Points.Count - 1);
					element.Points = new PointCollection(Points);
					DesignerCanvas.CreateDesignerItem(element);
				}
			}
		}
		public override void UpdateZoom()
		{
			base.UpdateZoom();
			rubberband.StrokeThickness = 1 / ZoomFactor;
		}
	}
}