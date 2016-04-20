using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
//using PointCollection = Common.PointCollection;

namespace Infrastructure.Plans.InstrumentAdorners
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
				if (Points.Count >= 2 && (Keyboard.Modifiers & ModifierKeys.Shift) > 0)
					point = GeometryHelper.TranslatePoint(Points.Count > 2 ? Points[Points.Count - 3] : new Point(Points[Points.Count - 2].X, -100), Points[Points.Count - 2], point);
				Points[Points.Count - 1] = CutPoint(point);
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			var point = e.GetPosition(this);
			if (e.ButtonState == MouseButtonState.Released && _opened)
				switch (e.ChangedButton)
				{
					case MouseButton.Left:
						Points.Add(CutPoint(point));
						if (Points.Count == 1)
							Points.Add(CutPoint(point));
						break;
					case MouseButton.Right:
						if (Points.Count > 1)
						{
							ElementBaseShape element = CreateElement();
							if (element != null)
							{
								element.Points = Points.ToRubezhPointCollection();
								DesignerCanvas.CreateDesignerItem(element);
							}
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
					element.Points = Points.ToRubezhPointCollection();
					DesignerCanvas.CreateDesignerItem(element);
				}
				Cleanup();
			}
		}
		private void Cleanup()
		{
			_opened = false;
			if (Rubberband != null)
				Points.Clear();
			AdornerCanvas.ReleaseMouseCapture();
			AdornerCanvas.Children.Remove(rubberband);
		}
	}
}