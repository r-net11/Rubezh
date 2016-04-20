using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class RubberbandAdorner : InstrumentAdorner
	{
		private Point? endPoint;
		private readonly Rectangle rubberband;

		public RubberbandAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
			rubberband = new Rectangle
			{
				Stroke = Brushes.Navy,
				StrokeThickness = 1 / ZoomFactor,
				StrokeDashArray = new DoubleCollection(new double[] { 2 })
			};
		}

		public override bool AllowBackgroundStart
		{
			get { return true; }
		}
		protected override void Show()
		{
			rubberband.Width = 0;
			rubberband.Height = 0;
			AdornerCanvas.Children.Add(rubberband);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!IsMouseCaptured)
					CaptureMouse();

				endPoint = e.GetPosition(this);

				if (StartPoint != null)
				{
					UpdateRubberband();
					UpdateSelection();
				}
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
		private void UpdateSelection()
		{
			if (!StartPoint.HasValue || !endPoint.HasValue) return;
			var rubberBand = new Rect(StartPoint.Value, endPoint.Value);
			foreach (DesignerItem designerItem in DesignerCanvas.Items)
				if (designerItem.IsEnabled)
				{
					var itemRect = designerItem.ContentBounds;
					designerItem.IsSelected = rubberBand.Contains(itemRect);
					//Rect itemBounds = designerItem.TransformToAncestor(DesignerCanvas).TransformBounds(itemRect);
					//designerItem.IsSelected = rubberBand.Contains(itemBounds);
				}
		}
	}
}