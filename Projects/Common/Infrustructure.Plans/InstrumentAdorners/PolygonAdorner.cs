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
		private bool _opened;

		public BasePolygonAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
			_opened = false;
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
		public override void Hide()
		{
			base.Hide();
			Cleanup();
		}

		protected abstract Shape CreateRubberband();
		protected abstract PointCollection Points { get; }
		protected abstract ElementBaseShape CreateElement();

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			DesignerCanvas.DeselectAll();
			if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			{
				if (!AdornerCanvas.Children.Contains(rubberband))
					AdornerCanvas.Children.Add(rubberband);
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
				_opened = true;
			}
			if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
				ClosePolygon();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Points.Count > 0 && _opened)
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
			if (e.ButtonState == MouseButtonState.Released && _opened)
				switch (e.ChangedButton)
				{
					case MouseButton.Left:
						Points.Add(CutPoint(e.GetPosition(this)));
						if (Points.Count == 1)
							Points.Add(CutPoint(e.GetPosition(this)));
						break;
					case MouseButton.Right:
						ElementBaseShape element = CreateElement();
						if (element != null)
						{
							if (Points.Count > 1)
								element.Points = new PointCollection(Points);
							else
								element.Position = CutPoint(e.GetPosition(this));
							DesignerCanvas.CreateDesignerItem(element);
						}
						Cleanup();
						break;
				}
		}

		public override void UpdateZoom()
		{
			base.UpdateZoom();
			rubberband.StrokeThickness = 1 / ZoomFactor;
		}
		public override bool KeyboardInput(Key key)
		{
			var handled = base.KeyboardInput(key);
			if (!handled)
			{
				if (key == Key.Enter)
				{
					ClosePolygon();
					handled = true;
				}
				else if (key == Key.Escape && _opened)
				{
					Cleanup();
					handled = true;
				}
				else if (key == Key.RightShift || key == Key.LeftShift)
				{
					var args = new MouseEventArgs(Mouse.PrimaryDevice, 0);
					args.RoutedEvent = UIElement.MouseMoveEvent;
					RaiseEvent(args);
					handled = true;
				}
			}
			return handled;
		}
		private void ClosePolygon()
		{
			if (Points.Count > 2)
			{
				ElementBaseShape element = CreateElement();
				if (element != null)
				{
					Points.RemoveAt(Points.Count - 1);
					element.Points = new PointCollection(Points);
					DesignerCanvas.CreateDesignerItem(element);
				}
				Cleanup();
			}
		}
		private void Cleanup()
		{
			_opened = false;
			Points.Clear();
			AdornerCanvas.ReleaseMouseCapture();
			AdornerCanvas.Children.Remove(rubberband);
		}
	}
}