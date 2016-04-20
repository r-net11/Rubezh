using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Infrastructure.Common.Services.DragDrop
{
	public class DragAdorner : Adorner
	{
		private UIElement _child;
		private Point _adornerOffset;
		private Vector _vector;

		public DragAdorner(UIElement adornedElement, UIElement adornerElement, bool useVisualBrush, double opacity = 0.7)
			: base(adornedElement)
		{
			if (useVisualBrush)
			{
				Rectangle rect = new Rectangle();
				rect.Width = adornerElement.RenderSize.Width;
				rect.Height = adornerElement.RenderSize.Height;

				VisualBrush visualBrush = new VisualBrush(adornerElement);
				visualBrush.Opacity = opacity;
				visualBrush.Stretch = Stretch.None;
				rect.Fill = visualBrush;

				_child = rect;
			}
			else
				_child = adornerElement;
		}

		public void UpdatePosition(Point position, bool recalculateShift = false)
		{
			if (recalculateShift)
			{
				Measure(AdornedElement.DesiredSize);
				_vector = new Vector(_child.DesiredSize.Width / 2, _child.DesiredSize.Height / 2);
			}
			if (_adornerOffset != position)
			{
				_adornerOffset = position - _vector;
				AdornerLayer adornerLayer = (AdornerLayer)Parent;
				if (adornerLayer != null)
					adornerLayer.Update(AdornedElement);
			}
		}

		protected override int VisualChildrenCount { get { return 1; } }
		protected override Visual GetVisualChild(int index)
		{
			return _child;
		}
		protected override Size MeasureOverride(Size finalSize)
		{
			_child.Measure(finalSize);
			_vector = new Vector(_child.DesiredSize.Width / 2, _child.DesiredSize.Height / 2);
			return _child.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize)
		{
			_child.Arrange(new Rect(finalSize));
			return finalSize;
		}
		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			GeneralTransformGroup newTransform = new GeneralTransformGroup();
			newTransform.Children.Add(base.GetDesiredTransform(transform));
			newTransform.Children.Add(new TranslateTransform(_adornerOffset.X, _adornerOffset.Y));
			return newTransform;
		}
	}
}
