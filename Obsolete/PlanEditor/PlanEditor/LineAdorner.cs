using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PlanEditor
{
    class LineAdorner : Adorner
    {
        Canvas canvas;
        Point from;
        bool isMove = false;
        //private Rectangle _child = null;
        private Line _childLine = null;
        private Thumb _childthumb = null;
        
        private double _leftOffset = 0;
        private double _topOffset = 0;
        private UIElement currentElement = null;

        public LineAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            VisualBrush _brush = new VisualBrush(adornedElement);

            currentElement = adornedElement;
            Line line = (Line)currentElement;
            _childLine = new Line();
            _childLine.X1 = line.X1;
            _childLine.Y1 = line.Y1;
            _childLine.X2 = line.X2;
            _childLine.Y2 = line.Y2;
        }

        public void SetOperationMove(bool _move)
        {
            isMove = _move;
        }

        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void SetPointFrom(Point point)
        {
            this.from = point;
        }

        double tmpX = 0;
        double tmpY = 0;

        protected override void OnRender(DrawingContext drawingContext)
        {
            //Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);

            if (currentElement is System.Windows.Shapes.Line)
            {
                if (isMove)
                {
                    drawingContext.DrawLine(renderPen, new Point(_childLine.X1, _childLine.Y1), new Point(_childLine.X2, _childLine.Y2));
                }
                else
                {
                    /*
                    tmpX = Mouse.GetPosition(canvas).X;
                    tmpY = Mouse.GetPosition(canvas).Y;
                    Canvas.SetTop(_childLine, 0);
                    Canvas.SetLeft(_childLine, 0);
                    drawingContext.DrawLine(renderPen, new Point(from.X, from.Y), new Point(tmpX, tmpY));
                     */
                    tmpX = Mouse.GetPosition(canvas).X;
                    tmpY = Mouse.GetPosition(canvas).Y;
                    drawingContext.DrawLine(renderPen, new Point(from.X, from.Y), new Point(tmpX, tmpY));
                    
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (currentElement is System.Windows.Controls.Primitives.Thumb)
            {
                _childLine.Measure(constraint);
                return _childLine.DesiredSize;
            }
            else
            {
                _childLine.Measure(constraint);
                return _childLine.DesiredSize;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (currentElement is System.Windows.Controls.Primitives.Thumb)
            {
                _childLine.Arrange(new Rect(finalSize));
                return finalSize;
            }
            else
            {
                _childLine.Arrange(new Rect(finalSize));
                return finalSize;
            }


        }

        protected override Visual GetVisualChild(int index)
        {
            return _childLine;
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
