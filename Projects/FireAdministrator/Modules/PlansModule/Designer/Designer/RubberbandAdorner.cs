using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlansModule.Designer
{
	public class RubberbandAdorner : Adorner
	{
		private Point? startPoint, endPoint;
		private Rectangle rubberband;
		private DesignerCanvas designerCanvas;
		private VisualCollection visuals;
		private Canvas adornerCanvas;

		protected override int VisualChildrenCount
		{
			get { return this.visuals.Count; }
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			this.adornerCanvas.Arrange(new Rect(arrangeBounds));
			return arrangeBounds;
		}

		protected override Visual GetVisualChild(int index)
		{
			return this.visuals[index];
		}

		double ZoomFactor
		{
			get { return designerCanvas.PlanDesignerViewModel.Zoom; }
		}

		public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
			: base(designerCanvas)
		{
			this.designerCanvas = designerCanvas;
			this.startPoint = dragStartPoint;

			this.adornerCanvas = new Canvas()
			{
				Background = Brushes.Transparent
			};
			this.visuals = new VisualCollection(this);
			this.visuals.Add(this.adornerCanvas);

			this.rubberband = new Rectangle()
			{
				Stroke = Brushes.Navy,
				StrokeThickness = 1 / ZoomFactor,
				StrokeDashArray = new DoubleCollection(new double[] { 2 })
			};

			this.adornerCanvas.Children.Add(this.rubberband);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!this.IsMouseCaptured)
				{
					this.CaptureMouse();
				}

				this.endPoint = e.GetPosition(this);
				this.UpdateRubberband();
				this.UpdateSelection();
				e.Handled = true;
			}
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (this.IsMouseCaptured)
			{
				this.ReleaseMouseCapture();
			}

			AdornerLayer adornerLayer = this.Parent as AdornerLayer;
			if (adornerLayer != null)
			{
				adornerLayer.Remove(this);
			}
		}

		private void UpdateRubberband()
		{
			rubberband.StrokeThickness = 1 / ZoomFactor;

			double left = Math.Min(this.startPoint.Value.X, this.endPoint.Value.X);
			double top = Math.Min(this.startPoint.Value.Y, this.endPoint.Value.Y);

			double width = Math.Abs(this.startPoint.Value.X - this.endPoint.Value.X);
			double height = Math.Abs(this.startPoint.Value.Y - this.endPoint.Value.Y);

			this.rubberband.Width = width;
			this.rubberband.Height = height;
			Canvas.SetLeft(this.rubberband, left);
			Canvas.SetTop(this.rubberband, top);
		}

		private void UpdateSelection()
		{
			Rect rubberBand = new Rect(this.startPoint.Value, this.endPoint.Value);
			foreach (DesignerItem designerItem in this.designerCanvas.Children)
			{
				if (designerItem.IsVisibleLayout && designerItem.IsSelectableLayout)
				{
					Rect itemRect = VisualTreeHelper.GetDescendantBounds(designerItem);
					Rect itemBounds = designerItem.TransformToAncestor(designerCanvas).TransformBounds(itemRect);
					designerItem.IsSelected = rubberBand.Contains(itemBounds);
				}
			}
		}
	}
}