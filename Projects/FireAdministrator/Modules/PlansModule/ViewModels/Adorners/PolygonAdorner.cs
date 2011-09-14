using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class PolygonAdorner : Adorner
    {
        bool isMove = false;

        private UIElement currentElement = null;
        private Polygon polygon = null;
        private double _leftOffset = 0;
        private double _topOffset = 0;
        

        public PolygonAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualBrush _brush = new VisualBrush(adornedElement);
            currentElement = adornedElement;
            Polygon _polygon = (Polygon)currentElement;
            polygon = new Polygon();
            foreach (var point in _polygon.Points)
            {
                polygon.Points.Add(point);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            Polygon polygon = currentElement as Polygon;
            Point _start = polygon.Points[0];
            Point _prev = _start;
            Point _point = new Point();
            foreach (Point point in polygon.Points)
            {
                drawingContext.DrawLine(renderPen, new Point(_prev.X, _prev.Y + PlanCanvasView.dTop), new Point(point.X, point.Y + PlanCanvasView.dTop));
                _prev = point;
                _point = point;
            }
            drawingContext.DrawLine(renderPen, new Point(_point.X, _point.Y + PlanCanvasView.dTop), new Point(_start.X, _start.Y + PlanCanvasView.dTop));
        }

        public void SetOperationMove(bool _move)
        {
            isMove = _move;
        }

        public double LeftOffset
        {
            get
            {
                return _leftOffset;
            }
            set
            {
                if (isMove)
                {
                    _leftOffset = value;
                }
                else
                {
                    _leftOffset = 0;
                }

                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get
            {
                return _topOffset;
            }
            set
            {
                if (isMove)
                {
                    _topOffset = value;
                }
                else
                {
                    _topOffset = 0;
                }

                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }
    }
}
