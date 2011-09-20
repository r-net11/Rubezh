using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class TextBoxAdorner : Adorner
    {
        bool isMove;
        UIElement currentElement;
        Rectangle rectangle;
        double _leftOffset;
        double _topOffset;
        Rect adornedElementRect;

        public TextBoxAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualBrush _brush = new VisualBrush(adornedElement);
            currentElement = adornedElement;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            adornedElementRect.Y = adornedElementRect.Y + PlanCanvasView.dTop;
            drawingContext.DrawRectangle(null, renderPen, adornedElementRect);
        }

        public void SetOperationMove(bool _move)
        {
            isMove = _move;
        }

        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                if (isMove)
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
                if (isMove)
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