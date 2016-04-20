using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Infrastructure.Plans.InstrumentAdorners
{
	public abstract class BaseRectangleAdorner : InstrumentAdorner
	{
		private Point? endPoint;
		private Shape rubberband;

		public BaseRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
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
			endPoint = null;
		}

		protected virtual Shape CreateRubberband()
		{
			return new Rectangle();
		}

		protected abstract ElementBaseRectangle CreateElement();

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !endPoint.HasValue)
			{
				DesignerCanvas.DeselectAll();
				StartPoint = e.GetPosition(this);
				endPoint = null;
				if (!AdornerCanvas.Children.Contains(rubberband))
					AdornerCanvas.Children.Add(rubberband);
				//AdornerCanvas.Cursor = null;
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (StartPoint.HasValue && AdornerCanvas.Children.Contains(rubberband))
			{
				if (!AdornerCanvas.IsMouseCaptured)
					AdornerCanvas.CaptureMouse();
				endPoint = CutPoint(e.GetPosition(this));
				UpdateRubberband();
				e.Handled = true;
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (!AdornerCanvas.IsMouseCaptured || !StartPoint.HasValue || !endPoint.HasValue) return;
			//AdornerCanvas.Cursor = Cursors.Pen;
			ElementBaseRectangle element = CreateElement();
			if (element != null)
			{
				element.Left = Canvas.GetLeft(rubberband);
				element.Top = Canvas.GetTop(rubberband);
				element.Height = rubberband.Height;
				element.Width = rubberband.Width;
				DesignerCanvas.CreateDesignerItem(element);
			}
			Cleanup();
		}

		private void UpdateRubberband()
		{
			if (!StartPoint.HasValue || !endPoint.HasValue) return;

			var left = Math.Min(StartPoint.Value.X, endPoint.Value.X);
			var top = Math.Min(StartPoint.Value.Y, endPoint.Value.Y);

			var width = Math.Abs(StartPoint.Value.X - endPoint.Value.X);
			var height = Math.Abs(StartPoint.Value.Y - endPoint.Value.Y);

			rubberband.Width = width;
			rubberband.Height = height;
			Canvas.SetLeft(rubberband, left);
			Canvas.SetTop(rubberband, top);
		}

		public override void UpdateZoom()
		{
			base.UpdateZoom();
			rubberband.StrokeThickness = 1 / ZoomFactor;
		}

		public override bool KeyboardInput(Key key)
		{
			var handled = base.KeyboardInput(key);
			if (handled || key != Key.Escape || !endPoint.HasValue) return handled;

			Cleanup();

			return true;
		}

		private void Cleanup()
		{
			StartPoint = null;
			endPoint = null;
			rubberband.Width = 0;
			rubberband.Height = 0;
			AdornerCanvas.ReleaseMouseCapture();
			AdornerCanvas.Children.Remove(rubberband);
		}
	}
}