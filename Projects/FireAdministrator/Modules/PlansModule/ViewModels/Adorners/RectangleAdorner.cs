using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class RectangleAdorner : Adorner
    {
        bool isMove = false;

        private UIElement currentElement = null;
        private Rectangle rectangle = null;
        
        private double _leftOffset = 0;
        private double _topOffset = 0;
        Rect adornedElementRect;

        public RectangleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualBrush _brush = new VisualBrush(adornedElement);
            currentElement = adornedElement;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            adornedElementRect.Y = adornedElementRect.Y + PlanCanvasView.dTop;
            drawingContext.DrawRectangle(null, renderPen, adornedElementRect);
        }

        /*
        public void SetOperationMove(bool _move)
        {
            isMove = _move;
        }
        */
        public double LeftOffset
        {
            get
            {
                return _leftOffset;
            }
            set
            {
                //if (isMove)
                {
                    _leftOffset = value;
                }/*
                else
                {
                    _leftOffset = 0;
                }*/

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
               //if (isMove)
               // {
                    _topOffset = value;
               // }
                /*else
                {
                    _topOffset = 0;
                }
                */
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
