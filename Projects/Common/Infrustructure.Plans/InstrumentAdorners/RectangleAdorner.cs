using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Infrustructure.Plans.InstrumentAdorners
{
	public abstract class BaseRectangleAdorner : InstrumentAdorner
	{
		private Point? _endPoint;
		private Shape _rubberband;

		public BaseRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override void Show()
		{
			_rubberband = CreateRubberband();
			_rubberband.Stroke = Brushes.Navy;
			_rubberband.StrokeThickness = 1 / ZoomFactor;
			AdornerCanvas.Cursor = Cursors.Pen;
		}

		public override void Hide()
		{
			base.Hide();
			_endPoint = null;
		}

		protected virtual Shape CreateRubberband()
		{
			return new Rectangle();
		}

		protected abstract ElementBaseRectangle CreateElement();

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !_endPoint.HasValue)
			{
				DesignerCanvas.DeselectAll();
				StartPoint = e.GetPosition(this);
				_endPoint = null;
				if (!AdornerCanvas.Children.Contains(_rubberband))
					AdornerCanvas.Children.Add(_rubberband);
				//AdornerCanvas.Cursor = null;
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (StartPoint.HasValue && AdornerCanvas.Children.Contains(_rubberband))
			{
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
				_endPoint = CutPoint(e.GetPosition(this));
				UpdateRubberband();
				e.Handled = true;
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (!AdornerCanvas.IsMouseCaptured || !StartPoint.HasValue || !_endPoint.HasValue) return;
			//AdornerCanvas.Cursor = Cursors.Pen;
			ElementBaseRectangle element = CreateElement();
			if (element != null)
			{
				element.Left = Canvas.GetLeft(_rubberband);
				element.Top = Canvas.GetTop(_rubberband);
				element.Height = _rubberband.Height;
				element.Width = _rubberband.Width;
				DesignerCanvas.CreateDesignerItem(element);
			}
			Cleanup();
		}

		private void UpdateRubberband()
		{
			if (!StartPoint.HasValue || !_endPoint.HasValue) return;

			var left = Math.Min(StartPoint.Value.X, _endPoint.Value.X);
			var top = Math.Min(StartPoint.Value.Y, _endPoint.Value.Y);

			var width = Math.Abs(StartPoint.Value.X - _endPoint.Value.X);
			var height = Math.Abs(StartPoint.Value.Y - _endPoint.Value.Y);

			_rubberband.Width = width;
			_rubberband.Height = height;
			Canvas.SetLeft(_rubberband, left);
			Canvas.SetTop(_rubberband, top);
		}

		public override void UpdateZoom()
		{
			base.UpdateZoom();
			_rubberband.StrokeThickness = 1 / ZoomFactor;
		}

		public override bool KeyboardInput(Key key)
		{
			var handled = base.KeyboardInput(key);
			if (handled || key != Key.Escape || !_endPoint.HasValue) return handled;

			Cleanup();

			return true;
		}

		private void Cleanup()
		{
			StartPoint = null;
			_endPoint = null;
			_rubberband.Width = 0;
			_rubberband.Height = 0;
			AdornerCanvas.ReleaseMouseCapture();
			AdornerCanvas.Children.Remove(_rubberband);
		}
	}
}