using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;
using FiresecAPI.Models;

namespace PlansModule.InstrumentAdorners
{
	public class RectangleAdorner : InstrumentAdorner
	{
		private Point? endPoint;
		private Rectangle rubberband;

		public RectangleAdorner(DesignerCanvas designerCanvas)
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
			AdornerCanvas.Cursor = Cursors.Pen;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				StartPoint = e.GetPosition(this);
				AdornerCanvas.Children.Add(rubberband);
				AdornerCanvas.Cursor = null;
				if (!IsMouseCaptured)
					CaptureMouse();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				endPoint = e.GetPosition(this);
				UpdateRubberband();
				e.Handled = true;
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (IsMouseCaptured)
				ReleaseMouseCapture();
			AdornerCanvas.Children.Remove(rubberband);
			AdornerCanvas.Cursor = Cursors.Pen;
			ElementRectangle element = new ElementRectangle();
			element.SetDefault();
			element.Left = Canvas.GetLeft(rubberband);
			element.Top = Canvas.GetTop(rubberband);
			element.Height = rubberband.Height;
			element.Width = rubberband.Width;
			((DesignerCanvas)DesignerCanvas).AddElement(element);

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

		//private void UpdateSelection()
		//{
		//    Rect rubberBand = new Rect(StartPoint.Value, endPoint.Value);
		//    foreach (DesignerItem designerItem in DesignerCanvas.Children)
		//    {
		//        if (designerItem.IsVisibleLayout && designerItem.IsSelectableLayout)
		//        {
		//            Rect itemRect = VisualTreeHelper.GetDescendantBounds(designerItem);
		//            Rect itemBounds = designerItem.TransformToAncestor(DesignerCanvas).TransformBounds(itemRect);
		//            designerItem.IsSelected = rubberBand.Contains(itemBounds);
		//        }
		//    }
		//}
	}
}