using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class RubberbandAdorner : InstrumentAdorner
	{
		private Point? _endPoint;
		private readonly Rectangle _rubberband;

		public RubberbandAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
			_rubberband = new Rectangle
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
			_rubberband.Width = 0;
			_rubberband.Height = 0;
			AdornerCanvas.Children.Add(_rubberband);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!IsMouseCaptured)
					CaptureMouse();

				_endPoint = e.GetPosition(this);

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
			_rubberband.StrokeThickness = 1 / ZoomFactor;

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
		private void UpdateSelection()
		{
			if (!StartPoint.HasValue || !_endPoint.HasValue) return;
			var rubberBand = new Rect(StartPoint.Value, _endPoint.Value);
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