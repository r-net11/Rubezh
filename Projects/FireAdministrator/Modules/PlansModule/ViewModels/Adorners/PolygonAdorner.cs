using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class PolygonAdorner : Adorner
    {
        bool _isMove;
        UIElement _currentElement;
        Polygon _polygon;
        double _leftOffset;
        double _topOffset;

        public PolygonAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            var brush = new VisualBrush(adornedElement);
            _currentElement = adornedElement;
            _polygon = new Polygon();
            foreach (var point in (_currentElement as Polygon).Points)
            {
                _polygon.Points.Add(point);
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Point start = (_currentElement as Polygon).Points[0];
            Point prev = start;
            Point _point = new Point();
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            foreach (Point point in (_currentElement as Polygon).Points)
            {
                drawingContext.DrawLine(renderPen, new Point(prev.X, prev.Y + PlanCanvasView.dTop), new Point(point.X, point.Y + PlanCanvasView.dTop));
                prev = point;
                _point = point;
            }
            drawingContext.DrawLine(renderPen, new Point(_point.X, _point.Y + PlanCanvasView.dTop), new Point(start.X, start.Y + PlanCanvasView.dTop));
        }

        public void SetOperationMove(bool isMove)
        {
            _isMove = isMove;
        }

        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                if (_isMove)
                    _leftOffset = value;
                else
                    _leftOffset = 0;

                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                if (_isMove)
                    _topOffset = value;
                else
                    _topOffset = 0;

                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            if (Parent is AdornerLayer)
            {
                (Parent as AdornerLayer).Update(AdornedElement);
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