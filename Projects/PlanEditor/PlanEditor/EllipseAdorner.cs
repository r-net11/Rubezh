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
    class EllipseAdorner : Adorner
    {
        Canvas canvas;
        Rect adornedElementRect;
        private double MinWidth;
        private double MinHeight;
        bool isMove = false;
        //private Rectangle _childEllipse = null;
        private Ellipse _childEllipse = null;
        private double _leftOffset = 0;
        private double _topOffset = 0;
        private UIElement currentElement = null;
        private Thumb thumb;
        
        public EllipseAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualBrush _brush = new VisualBrush(adornedElement);
            currentElement = adornedElement;
            Ellipse rect = (Ellipse)currentElement;
            _childEllipse = new Ellipse();
            _childEllipse.Width = adornedElement.RenderSize.Width;
            _childEllipse.Height = adornedElement.RenderSize.Height;
        }

        public void SetOperationMove(bool _move)
        {
            isMove = _move;
        }

        public void SetCanvas(Canvas canvas, double minwidth, double minheight, Thumb thumb)
        {
            this.canvas = canvas;
            this.MinHeight = minheight;
            this.MinWidth = minwidth;
            this.thumb = thumb;
        }
        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void SetRect(Rect rect)
        {
            this.adornedElementRect = rect;
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

            if (currentElement is System.Windows.Shapes.Ellipse)
            {
                if (isMove)
                {
                    adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
                    adornedElementRect.X = adornedElementRect.X + _childEllipse.Width / 2;
                    adornedElementRect.Y = adornedElementRect.Y + _childEllipse.Height / 2;
                    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, _childEllipse.Width / 2, _childEllipse.Height / 2);
                }
                else
                {
                    tmpX = Mouse.GetPosition(canvas).X;
                    tmpY = Mouse.GetPosition(canvas).Y;
                    double radiusX = 0;
                    double radiusY = 0;
                    double x = adornedElementRect.X;
                    double y = adornedElementRect.Y;
                    double w = adornedElementRect.Width;
                    double h = adornedElementRect.Height;

                    if (thumb.Name.Equals("thumb1"))
                    {
                        adornedElementRect = new Rect(tmpX+w, tmpY+h, w + Math.Abs(tmpX - x),h + Math.Abs(tmpY - y));
                        radiusX = w/2 + Math.Abs((tmpX - x));
                        radiusY = h/2 + Math.Abs((tmpY - y));
                    }

                    if (thumb.Name.Equals("thumb2"))
                    {
                        //MessageBox.Show("2");
                        //_overlayElementRect.SetRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height), 2);
                    }
                    if (thumb.Name.Equals("thumb3"))
                    {
                        //_overlayElementRect.SetRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height), 3);
                    }
                    if (thumb.Name.Equals("thumb4"))
                    {
                        //_overlayElementRect.SetRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height), 4);
                    }
                    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, radiusX, radiusY);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _childEllipse.Measure(constraint);
            return _childEllipse.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _childEllipse.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _childEllipse;
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
