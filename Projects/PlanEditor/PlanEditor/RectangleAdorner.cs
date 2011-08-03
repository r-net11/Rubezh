using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace PlanEditor
{
    
    class RectangleAdorner : Adorner
    {
        Canvas canvas;
        Rect adornedElementRect;
        private double MinWidth;
        private double MinHeight;
        bool isMove = false;
        private Rectangle _childRect = null;
        private double _leftOffset = 0;
        private double _topOffset = 0;
        private UIElement currentElement = null;
        int number;
        public RectangleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualBrush _brush = new VisualBrush(adornedElement);
            currentElement = adornedElement;
            Rectangle rect = (Rectangle)currentElement;
            _childRect = new Rectangle();
            _childRect.Width = adornedElement.RenderSize.Width;
            _childRect.Height = adornedElement.RenderSize.Height;
        }

        public void SetOperationMove(bool _move)
        {
            isMove = _move;
        }

        public void SetCanvas(Canvas canvas, double minwidth, double minheight)
        {
            this.canvas = canvas;
            this.MinHeight = minheight;
            this.MinWidth = minwidth;
        }
        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void SetRect(Rect rect, int number)
        {
            this.adornedElementRect = rect;
            this.number = number;
        }

        double tmpX = 0;
        double tmpY = 0;

        public void ChangeText(string s)
        {
            foreach (UIElement element in canvas.Children)
            {
                if (element is System.Windows.Controls.TextBox)
                {
                    TextBox txt = (TextBox)element;
                    txt.Text = s;
                }
            }
        }
        public Rect GetResultRect()
        {
            return adornedElementRect;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);

            if (currentElement is System.Windows.Shapes.Rectangle)
            {
                if (isMove)
                {
                    adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
                    drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
                }
                else
                {
                    tmpX = Mouse.GetPosition(canvas).X;
                    tmpY = Mouse.GetPosition(canvas).Y;
                    double x = adornedElementRect.X;
                    double y = adornedElementRect.Y;
                    if (number == 1 && Math.Abs(adornedElementRect.Width + (x - tmpX)) >= MinWidth && Math.Abs(adornedElementRect.Height + (y - tmpY)) >= MinHeight)
                    {
                        adornedElementRect = new Rect(tmpX, tmpY, Math.Abs(adornedElementRect.Width + (x - tmpX)), Math.Abs(adornedElementRect.Height + (y - tmpY)));
                        
                    }
                    if (number == 2 && Math.Abs(x - tmpX) >= MinWidth && Math.Abs(adornedElementRect.Height + (y - tmpY))>=MinHeight)
                    {
                        adornedElementRect = new Rect(x, tmpY, Math.Abs(x-tmpX), Math.Abs(adornedElementRect.Height + (y- tmpY)));
                    }
                    if (number == 3 && Math.Abs(tmpX - x) >= MinWidth && Math.Abs(tmpY - y)>=MinHeight)
                    {
                        adornedElementRect = new Rect(x, y, Math.Abs(tmpX-x), Math.Abs(tmpY-y));
                    }
                    if (number == 4 && Math.Abs(adornedElementRect.Width + (x - tmpX)) >= MinWidth && Math.Abs(y - tmpY)>=MinHeight)
                    {
                        adornedElementRect = new Rect(tmpX, y, Math.Abs(adornedElementRect.Width + (x - tmpX)), Math.Abs(y - tmpY));
                    }
                    drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
                }
            }
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
            get
            {
                return 1;
            }
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
