using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlansModule.ViewModels
{
    public class RectangleAdornerResize : Adorner
    {
        UIElement _currentElement;
        Rectangle _rectangle;
        Canvas _canvas;
        Rect _adornedElementRect;
        int _number;
        double MinWidth;
        double MinHeight;
        Rectangle _childRect;

        public RectangleAdornerResize(UIElement adornedElement)
            : base(adornedElement)
        {
            _currentElement = adornedElement;
            _childRect = new Rectangle();
            _childRect.Width = adornedElement.RenderSize.Width;
            _childRect.Height = adornedElement.RenderSize.Height;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            _adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            drawingContext.DrawRectangle(null, renderPen, _adornedElementRect);
        }

        public void SetCanvas(Canvas canvas, double minwidth, double minheight)
        {
            _canvas = canvas;
            MinHeight = minheight;
            MinWidth = minwidth;
        }

        public void SetRect(Rect rect, int number)
        {
            _adornedElementRect = rect;
            _number = number;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _childRect.Measure(constraint);
            return _childRect.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _childRect.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _childRect;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        double _heightOffset;
        public double HeightOffset
        {
            get { return _heightOffset; }
            set
            {
                _heightOffset = value;
                UpdatePosition();
            }
        }

        double _widthOffset;
        public double WidthOffset
        {
            get { return _widthOffset; }
            set
            {
                _widthOffset = value;
                UpdatePosition();
            }
        }

        double _leftOffset;
        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value;
                UpdatePosition();
            }
        }

        double _rightOffset;
        public double RightOffset
        {
            get { return _rightOffset; }
            set
            {
                _rightOffset = value;
                UpdatePosition();
            }
        }

        double _topOffset;
        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = value;
                UpdatePosition();
            }
        }

        void UpdatePosition()
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