using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;

namespace PlansModule.InstrumentAdorners
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
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Points.Count > 0)
			{
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
				Points[Points.Count - 1] = CutPoint(e.GetPosition(this));
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
						AdornerCanvas.ReleaseMouseCapture();
						AdornerCanvas.Children.Remove(rubberband);
						ElementBaseShape element = CreateElement();
						if (element != null)
						{
							if (Points.Count > 1)
								element.Points = new PointCollection(Points);
							else
								element.Position = CutPoint(e.GetPosition(this));
							DesignerCanvas.CreateDesignerItem(element);
						}
						break;
				}
		}

		public override void KeyboardInput(Key key)
		{
			base.KeyboardInput(key);
			if (key == Key.Enter && Points.Count > 2)
			{
				AdornerCanvas.ReleaseMouseCapture();
				AdornerCanvas.Children.Remove(rubberband);
				ElementBaseShape element = CreateElement();
				if (element != null)
				{
					Points.RemoveAt(Points.Count - 1);
					element.Points = Points;
					DesignerCanvas.CreateDesignerItem(element);
				}
			}
		}
	}
}