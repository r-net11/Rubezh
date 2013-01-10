using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;

namespace PlansModule.InstrumentAdorners
{
	public class RubberbandAdorner : InstrumentAdorner
	{
		private Point? endPoint;
		private Rectangle rubberband;

		public RubberbandAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override void Show()
		{
			rubberband = new Rectangle()
			{
				Stroke = Brushes.Navy,
				StrokeThickness = 1 / ZoomFactor,
				StrokeDashArray = new DoubleCollection(new double[] { 2 })
			};
			AdornerCanvas.Children.Add(rubberband);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!IsMouseCaptured)
					CaptureMouse();

				endPoint = e.GetPosition(this);
				UpdateRubberband();
				UpdateSelection();
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			Hide();
		}

		private void UpdateRubberband()
		{
			rubberband.StrokeThickness = 1 / ZoomFactor;

			double left = Math.Min(StartPoint.Value.X, endPoint.Value.X);
			double top = Math.Min(StartPoint.Value.Y, endPoint.Value.Y);

			double width = Math.Abs(StartPoint.Value.X - endPoint.Value.X);
			double height = Math.Abs(StartPoint.Value.Y - endPoint.Value.Y);

			rubberband.Width = width;
			rubberband.Height = height;
			Canvas.SetLeft(rubberband, left);
			Canvas.SetTop(rubberband, top);
		}
		private void UpdateSelection()
		{
			Rect rubberBand = new Rect(StartPoint.Value, endPoint.Value);
			foreach (DesignerItem designerItem in DesignerCanvas.Items)
				if (designerItem.IsEnabled)
				{
					Rect itemRect = VisualTreeHelper.GetDescendantBounds(designerItem);
					Rect itemBounds = designerItem.TransformToAncestor(DesignerCanvas).TransformBounds(itemRect);
					designerItem.IsSelected = rubberBand.Contains(itemBounds);
				}
		}
	}
}