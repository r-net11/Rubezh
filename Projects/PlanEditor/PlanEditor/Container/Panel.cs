using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Markup;



namespace PlanEditor
{

    public partial class CustomPanel : Canvas
    {
        #region Declaration

        public event EventHandler CompleteDraw;

        #endregion

        #region Properties
        #region Operation

        private bool _isInsert = false;
        private bool _isMove = false;
        private bool _isResize;

        #endregion
        private bool _isDown;
        private bool _isDragging;

        private Point _startPoint;
        private UIElement _originalElement;
        private LineAdorner _overlayElementLine;
        private RectangleAdorner _overlayElementRect;
        private EllipseAdorner _overlayElementEllipse;
        private double _originalTop;
        private double _originalLeft;

        public static ListObjects listShapes = new ListObjects();
        Thumb thumb = null;

        private UnDoRedo _UnDoObject;
        public UnDoRedo UnDoObject
        {
            get { return _UnDoObject; }
            set
            {
                _UnDoObject = value;
                //  UnDoObject.adornerevent += new EventHandler(UnDoObject_adornerevent);
            }
        }
        #endregion

        #region Constructors

        public CustomPanel()
        {
            this.Background = Brushes.White;
            this.Height = 450;
            this.Width = 450;
            ListObjects listShapes = new ListObjects();
        }

        void UnDoObject_adornerevent(object sender, EventArgs e)
        {
            SetEleAfterResize((Shape)selectdeobject);
        }

        #endregion

        public void onDragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = (Thumb)e.Source;
            thumb.Background = Brushes.Green;

        }

        public void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb thumb = (Thumb)e.Source;
            thumb.Background = Brushes.Blue;
        }

        #region Мои операции с PECanvas

        private void DragStarted()
        {
            if (_originalElement != null && (_originalElement is System.Windows.Shapes.Line))
            {
                _isDragging = true;

                _originalLeft = Canvas.GetLeft(_originalElement);
                _originalTop = Canvas.GetTop(_originalElement);
                _overlayElementLine = new LineAdorner(_originalElement);
                if (_isResize)
                {
                    _overlayElementLine.SetCanvas(this);
                    _overlayElementLine.SetOperationMove(false);
                    foreach (Object obj in listShapes)
                    {
                        if (obj is PlanEditor.PELine)
                        {
                            PELine line = (PELine)obj;
                            if (line.active)
                            {
                                Line l = (Line)line.GetShape();
                                double x = 0;
                                double y = 0;
                                if (thumb.Name.Equals("thumb2"))
                                {
                                    x = l.X1 + Canvas.GetLeft(l);
                                    y = l.Y1 + Canvas.GetTop(l);
                                }
                                if (thumb.Name.Equals("thumb1"))
                                {
                                    x = l.X2 + Canvas.GetLeft(l);
                                    y = l.Y2 + Canvas.GetTop(l);
                                }
                                _overlayElementLine.SetPointFrom(new Point(x, y));
                            }
                        }
                    }
                }
                else
                {
                    _overlayElementLine.SetOperationMove(true);
                    Line line = (Line)_originalElement;
                    double x1 = line.X1;
                    double y1 = line.Y1;
                    double top = Canvas.GetTop(line);
                    double left = Canvas.GetLeft(line);
                    double x2 = _overlayElementLine.LeftOffset-x1;
                    double y2 = _overlayElementLine.TopOffset-y1;

                    previousMarginOfSelectedObject = new Point(x1,y1);
                }

                AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                layer.Add(_overlayElementLine);
            };

            if (_originalElement != null && (_originalElement is System.Windows.Shapes.Rectangle))
            {
                _isDragging = true;
                _originalLeft = Canvas.GetLeft(_originalElement);
                _originalTop = Canvas.GetTop(_originalElement);
                _overlayElementRect = new RectangleAdorner(_originalElement);
                if (_isResize)
                {
                    _overlayElementRect.SetOperationMove(false);
                    foreach (Object obj in listShapes)
                    {
                        if (obj is PlanEditor.PERectangle)
                        {
                            PERectangle rect = (PERectangle)obj;
                            _overlayElementRect.SetCanvas(this, rect.MinWidth, rect.MinHeight);
                            if (rect.active)
                            {
                                Rectangle r = (Rectangle)rect.GetShape();
                                if (thumb.Name.Equals("thumb1"))
                                {
                                    _overlayElementRect.SetRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height), 1);
                                }
                                if (thumb.Name.Equals("thumb2"))
                                {
                                    _overlayElementRect.SetRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height), 2);
                                }
                                if (thumb.Name.Equals("thumb3"))
                                {
                                    _overlayElementRect.SetRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height), 3);
                                }
                                if (thumb.Name.Equals("thumb4"))
                                {
                                    _overlayElementRect.SetRect(new Rect(rect.X, rect.Y, rect.Width, rect.Height), 4);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _overlayElementRect.SetOperationMove(true);

                }
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                layer.Add(_overlayElementRect);
            };

            if (_originalElement != null && (_originalElement is System.Windows.Shapes.Ellipse))
            {
                _isDragging = true;
                _originalLeft = Canvas.GetLeft(_originalElement);
                _originalTop = Canvas.GetTop(_originalElement);
                _overlayElementEllipse = new EllipseAdorner(_originalElement);
                if (_isResize)
                {
                    _overlayElementEllipse.SetOperationMove(false);

                    foreach (Object obj in listShapes)
                    {
                        if (obj is PlanEditor.PEEllipse)
                        {
                            PEEllipse ellipse = (PEEllipse)obj;
                            Rectangle r = new Rectangle();
                            _overlayElementEllipse.SetCanvas(this, ellipse.MinWidth, ellipse.MinHeight, thumb);
                            _overlayElementEllipse.SetRect(new Rect(ellipse.X, ellipse.Y, ellipse.Width, ellipse.Height));
                        }
                    }
                }
                else
                {
                    _overlayElementEllipse.SetOperationMove(true);
                }
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                layer.Add(_overlayElementEllipse);
            };
        }
        //123
        private void DragMoved(int _code)
        {
            if (_overlayElementLine != null)
            {
                Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(this);
                _overlayElementLine.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementLine.TopOffset = CurrentPosition.Y - _startPoint.Y;
                listShapes.DragMove(_code, this);
            }
            if (_overlayElementRect != null)
            {
                Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(this);
                _overlayElementRect.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementRect.TopOffset = CurrentPosition.Y - _startPoint.Y;
                listShapes.DragMove(_code, this);
            }
            if (_overlayElementEllipse != null)
            {
                Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(this);
                _overlayElementEllipse.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementEllipse.TopOffset = CurrentPosition.Y - _startPoint.Y;
                listShapes.DragMove(_code, this);
            }
        }
        public void DragFinished(bool cancelled, int _code)
        {
            System.Windows.Input.Mouse.Capture(null);
            if (_isDragging && _overlayElementLine != null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElementLine.AdornedElement).Remove(_overlayElementLine);
                double x2 = _overlayElementLine.LeftOffset;
                double y2 = _overlayElementLine.TopOffset;
                double x1 = _originalLeft;
                double y1 = _originalTop;
                previousMarginOfSelectedObject = new Point(-1*x2, -1*y2);
                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _overlayElementLine.TopOffset);
                    Canvas.SetLeft(_originalElement, _overlayElementLine.LeftOffset);
                    listShapes.DragFinished(_code, this, _overlayElementLine.TopOffset, _overlayElementLine.LeftOffset);
                }
                //_overlayElementLine = null;
            }
            if (_isDragging && _overlayElementRect != null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElementRect.AdornedElement).Remove(_overlayElementRect);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElementRect.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementRect.LeftOffset);
                    listShapes.DragFinished(_code, this, _originalTop + _overlayElementRect.TopOffset, _originalLeft + _overlayElementRect.LeftOffset);
                }
                //_overlayElementRect = null;
            }
            if (_isDragging && _overlayElementEllipse != null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElementEllipse.AdornedElement).Remove(_overlayElementEllipse);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElementEllipse.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementEllipse.LeftOffset);
                    listShapes.DragFinished(_code, this, _originalTop + _overlayElementEllipse.TopOffset, _originalLeft + _overlayElementEllipse.LeftOffset);
                }
                //_overlayElementRect = null;
            }

            _isDragging = false;
            _isDown = false;
            //_isResize = false;
            //listShapes.SetActiveLineToResize(false);
        }

        #endregion


        #region line
        Point StartPointline;
        Line line;
        bool Isline = false;
        bool ContiniousDrawingLine = false;
        public void DrawLineApbMode()
        {
            if (Isline)
            {
            }
            else
            {
                if (Isellipse || Isrectangle || Isline) return;
                Isline = true;
                _isInsert = true;
            }
            this.Cursor = Cursors.Cross;
        }

        #endregion

        #region rectangle
        Point StartPointRectangle;
        Rectangle rect;
        bool Isrectangle = false;
        bool ContiniousDrawing = false;
        public void DrawRectangleApbMode()
        {
            if (Isrectangle)
            {
            }
            else
            {
                if (Isellipse || Isrectangle) return;
                Isrectangle = true;
            }
            this.Cursor = Cursors.Cross;
        }
        #endregion

        #region ellipse
        Ellipse ellipse;
        bool Isellipse = false;
        Point StartPointellipse;
        bool ContiniousDrawingellipse = false;
        public void DrawEllipseApbMode()
        {
            if (Isellipse)
            {
            }
            else
            {
                if (Isellipse || Isrectangle) return;
                Isellipse = true;
            }
            this.Cursor = Cursors.Cross;
        }
        #endregion

        #region delete

        object selectdeobject;
        Point previousMarginOfSelectedObject;
        Double previousheightOfSelectedObject, previouswidthOfSelectedObject;
        public void delete()
        {
            if (selectdeobject == null) return;
            if (selectdeobject is Image)
            {
                if (this.Children.Contains((UIElement)(((Image)selectdeobject).Parent)))
                {
                    UndoColorChangeForApbAndDevice();
                    ChangeRepresentationObject ChangeRepresentationObjectofDelete = UnDoObject.MakeChangeRepresentationObjectForDelete(((FrameworkElement)((Image)selectdeobject).Parent));
                    UnDoObject.InsertObjectforUndoRedo(ChangeRepresentationObjectofDelete);
                    this.Children.Remove(((UIElement)((Image)selectdeobject).Parent));

                    selectdeobject = null;
                }
            }
            else if (this.Children.Contains(((UIElement)selectdeobject)))
            {
                UndoColorChangeForApbAndDevice();

                ChangeRepresentationObject ChangeRepresentationObjectofDelete = UnDoObject.MakeChangeRepresentationObjectForDelete((FrameworkElement)selectdeobject);
                UnDoObject.InsertObjectforUndoRedo(ChangeRepresentationObjectofDelete);
                this.Children.Remove((UIElement)selectdeobject);

                selectdeobject = null;
            }
        }

        private void ColorChangeForApbAndDevice(object Selectedob)
        {
            if (Selectedob is Rectangle)
            {
                ((Rectangle)Selectedob).Stroke = Brushes.Red;
                ShowResizGrip((Rectangle)Selectedob);
            }
            else if (Selectedob is Ellipse)
            {
                ((Ellipse)Selectedob).Stroke = Brushes.Red;
                ShowResizGrip((Ellipse)Selectedob);
            }

        }

        private void UndoColorChangeForApbAndDevice()
        {
            if (selectdeobject == null) return;

            if (selectdeobject is Rectangle)
            {
                ((Rectangle)selectdeobject).Stroke = Brushes.Black;
                RemoveResizGrip();
            }
            else if (selectdeobject is Ellipse)
            {
                ((Ellipse)selectdeobject).Stroke = Brushes.Black;
                RemoveResizGrip();
            }

        }
        #endregion

        #region Mouse Related Event
        Point ptMouseStartforDrag;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (_isInsert)
            {
                if (Isline)
                {
                    Point point = Mouse.GetPosition(this);
                    PELine line = new PELine(new Point(point.X, point.Y), new Point(point.X + 50, point.Y + 50));
                    ContiniousDrawing = true;
                    this.Children.Add(line.GetShape());
                    listShapes.Add(line);
                }
            }
            else
            {
                _isDown = true;
                _isResize = false;
                _isMove = true;
                Isline = true;
                _startPoint = e.GetPosition(this);
                _originalElement = e.Source as UIElement;
                this.CaptureMouse();
                e.Handled = true;
            }
            if (Isrectangle)
            {
                Point marginpoint = e.GetPosition(this);
                StartPointRectangle = marginpoint;
                rect = new Rectangle();
                rect.Fill = Brushes.Transparent;
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 1;
                this.Children.Add(rect);
                ContiniousDrawing = true;
            };

            if (Isellipse)
            {
                ellipse = new Ellipse();
                ellipse.Fill = Brushes.Transparent;
                ellipse.Stroke = Brushes.Black;
                ellipse.StrokeThickness = 1;
                Point marginpoint = e.GetPosition(this);
                StartPointellipse = marginpoint;

                this.Children.Add(ellipse);
                ContiniousDrawingellipse = true;
            }
            else
            {
                //if (!(e.Source is Line))
                {
                    Point ptMouseStart = e.GetPosition(this);
                    ptMouseStartforDrag = ptMouseStart;

                    object TestPanelOrUI = this.InputHitTest(ptMouseStart) as FrameworkElement;
                    if (TestPanelOrUI != null)
                    {
                        if (TestPanelOrUI is CustomPanel)
                        {
                            UndoColorChangeForApbAndDevice();
                        }
                        else
                        {
                            if (selectdeobject != TestPanelOrUI) UndoColorChangeForApbAndDevice();
                            selectdeobject = TestPanelOrUI;
                            ColorChangeForApbAndDevice(selectdeobject);
                            ((Shape)selectdeobject).MouseMove += new System.Windows.Input.MouseEventHandler(Rect_MouseMove);
                            ((Shape)selectdeobject).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Rect_MouseLeftButtonUp);
                            ((Shape)selectdeobject).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Rect_MouseLeftButtonDown);

                            if (selectdeobject != null)
                            {
                                if (!(selectdeobject is Line))
                                {
                                    previousMarginOfSelectedObject = new Point(((FrameworkElement)selectdeobject).Margin.Left, ((FrameworkElement)selectdeobject).Margin.Top);
                                    previousheightOfSelectedObject = ((FrameworkElement)selectdeobject).Height;
                                    previouswidthOfSelectedObject = ((FrameworkElement)selectdeobject).Width;
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Source != this)
            {
                listShapes.SetActiveShape(e.Source, this);
            }
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved(e.Source.GetHashCode());
                }
            }

            if (Isrectangle && ContiniousDrawing)
            {
                Point endpoint = e.GetPosition(this);
                if (!(endpoint.X >= 0 && endpoint.X <= this.Width && StartPointRectangle.X >= 0 && StartPointRectangle.X <= this.Width && endpoint.Y >= 0 && endpoint.Y <= this.Height && StartPointRectangle.Y >= 0 && StartPointRectangle.Y <= this.Height)) return;
                double left = Math.Min(endpoint.X, StartPointRectangle.X);
                double top = Math.Min(endpoint.Y, StartPointRectangle.Y);
                rect.Margin = new Thickness(left, top, 0, 0);
                rect.Width = Math.Abs(endpoint.X - StartPointRectangle.X);
                rect.Height = Math.Abs(endpoint.Y - StartPointRectangle.Y);
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 2;
            }

            else if (Isellipse && ContiniousDrawingellipse)
            {
                Point endpoint = e.GetPosition(this);
                if (!(endpoint.X >= 0 && endpoint.X <= this.Width && StartPointellipse.X >= 0 && StartPointellipse.X <= this.Width && endpoint.Y >= 0 && endpoint.Y <= this.Height && StartPointellipse.Y >= 0 && StartPointellipse.Y <= this.Height)) return;
                double left = Math.Min(endpoint.X, StartPointellipse.X);
                double top = Math.Min(endpoint.Y, StartPointellipse.Y);
                ellipse.Margin = new Thickness(left, top, 0, 0);
                ellipse.Width = Math.Abs(endpoint.X - StartPointellipse.X);
                ellipse.Height = Math.Abs(endpoint.Y - StartPointellipse.Y);
                ellipse.Stroke = Brushes.Black;
                ellipse.StrokeThickness = 2;
            }


            if (selectdeobject is Rectangle)
            {
                SetEleAfterResize((Rectangle)selectdeobject);
            }
            else if (selectdeobject is Ellipse)
            {
                SetEleAfterResize((Ellipse)selectdeobject);
            }
            if (selectdeobject is Line)
            {
                //SetEleAfterResize((Line)selectdeobject);
                //MessageBox.Show("test");
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            OnMouseLeftButtonUp(null);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            string drawingCode = "";

            if (Isline)
            {
                if (_isDown)
                {
                    DragFinished(false, this.GetHashCode());
                }
                Isline = false;
                foreach (Object obj in listShapes)
                {
                    if (obj is PlanEditor.PELine)
                    {
                        PELine line = (PELine)obj;
                        if (line.active)
                        {
                            this.Cursor = Cursors.Arrow;
                            if (_isInsert)
                            {
                                _isInsert = false;
                                drawingCode = ("3," + line.Left + "," + line.Top + "," + line.Width + "," + line.Height);
                                CallDrawAPB(drawingCode, (Line)line.GetShape());
                                EndDraw();
                            }
                            if (_isMove)
                            {
                                _isMove = false;
                                if (selectdeobject != null)
                                {
                                    ChangeRepresentationObject ChangeRepresentationObjectOfResize = UnDoObject.MakeChangeRepresentationObjectForMove(previousMarginOfSelectedObject, (FrameworkElement)selectdeobject);
                                    UnDoObject.InsertObjectforUndoRedo(ChangeRepresentationObjectOfResize);
                                }

                            }
                        }
                    }
                }
            }
            else if (Isrectangle && ContiniousDrawing)
            {
                ContiniousDrawing = false;
                Isrectangle = false;
                this.Cursor = Cursors.Arrow;
                drawingCode = ("1," +
                        rect.Margin.Left + "," +
                        rect.Margin.Top + "," +
                         rect.Width + "," +
                        rect.Height);
                if (!double.IsNaN(rect.Height))
                {
                    CallDrawAPB(drawingCode, rect);
                }
                else EndDraw();
            }
            else if (Isellipse && ContiniousDrawingellipse)
            {
                ContiniousDrawingellipse = false;
                Isellipse = false;
                this.Cursor = Cursors.Arrow;
                drawingCode = ("3," +
                        ellipse.Margin.Left + "," +
                        ellipse.Margin.Top + "," +
                         ellipse.Width + "," +
                        ellipse.Height);
                if (!double.IsNaN(ellipse.Height))
                {
                    CallDrawAPB(drawingCode, ellipse);
                    EndDraw();
                }
                else EndDraw();
            }

            else
            {

                if (selectdeobject != null)
                {
                    if ((!new Point(((FrameworkElement)selectdeobject).Margin.Left, ((FrameworkElement)selectdeobject).Margin.Top).Equals(previousMarginOfSelectedObject)))
                    {
                        if (e != null)
                        {
                            ChangeRepresentationObject ChangeRepresentationObjectOfResize = UnDoObject.MakeChangeRepresentationObjectForMove(previousMarginOfSelectedObject, (FrameworkElement)selectdeobject);
                            UnDoObject.InsertObjectforUndoRedo(ChangeRepresentationObjectOfResize);
                        }
                    }

                }
            }
        }

        #endregion

        #region Common Function
        private bool IsValidPoint(Point pt)
        {
            if (pt.X <= 0 || pt.Y <= 0 || pt.X >= this.Width || pt.Y >= this.Height)
            {
                return false;
            }
            return true;
        }

        private void EndDraw()
        {

            if (CompleteDraw != null)
            {
                CompleteDraw(null, null);
            }
        }

        private void CallDrawAPB(string drawingCode, Shape oshape)
        {
            oshape.Tag = drawingCode;
            ChangeRepresentationObject ChangeRepresentationObjectforInsert = UnDoObject.MakeChangeRepresentationObjectForInsert(oshape);
            UnDoObject.InsertObjectforUndoRedo(ChangeRepresentationObjectforInsert);
            EndDraw();

        }

        #endregion

        #region Resize Rectangle
        Thumb _LeftThumb;
        Thumb _RightThumb;
        Thumb _TopThumb;
        Thumb _BottomThumb;



        private void ShowResizGrip(Shape oRect)
        {
            if (this.Children.IndexOf(_LeftThumb) > -1) { return; }

            #region Left
            _LeftThumb = new Thumb();
            SetResizeEllipeProperty(_LeftThumb);
            _LeftThumb.Margin = new Thickness(oRect.Margin.Left - 4, oRect.Margin.Top + oRect.Height / 2.0 - 6, 0, 0);
            _LeftThumb.Name = "Left";
            _LeftThumb.Tag = oRect;
            _LeftThumb.Cursor = Cursors.SizeWE;
            this.Children.Add(_LeftThumb);
            #endregion

            #region Right
            _RightThumb = new Thumb();
            SetResizeEllipeProperty(_RightThumb);
            _RightThumb.Margin = new Thickness(oRect.Margin.Left + oRect.Width - 6, oRect.Margin.Top + oRect.Height / 2.0 - 6, 0, 0);
            _RightThumb.Name = "Right";
            _RightThumb.Tag = oRect;
            _RightThumb.Cursor = Cursors.SizeWE;
            this.Children.Add(_RightThumb);
            #endregion

            #region Top
            _TopThumb = new Thumb();
            SetResizeEllipeProperty(_TopThumb);
            _TopThumb.Margin = new Thickness(oRect.Margin.Left + oRect.Width / 2.0 - 6, oRect.Margin.Top - 4, 0, 0);
            _TopThumb.Name = "Top";
            _TopThumb.Tag = oRect;
            _TopThumb.Cursor = Cursors.SizeNS;
            this.Children.Add(_TopThumb);
            #endregion

            #region Bottom
            _BottomThumb = new Thumb();
            SetResizeEllipeProperty(_BottomThumb);
            _BottomThumb.Margin = new Thickness(oRect.Margin.Left + oRect.Width / 2.0 - 6, oRect.Margin.Top + oRect.Height - 4, 0, 0);
            _BottomThumb.Name = "Bottom";
            _BottomThumb.Tag = oRect;
            _BottomThumb.Cursor = Cursors.SizeNS;
            this.Children.Add(_BottomThumb);
            #endregion


        }

        public void RemoveResizGrip()
        {
            if (this.Children.IndexOf(_LeftThumb) > -1)
            {

                this.Children.Remove(_LeftThumb);
                this.Children.Remove(_RightThumb);
                this.Children.Remove(_TopThumb);
                this.Children.Remove(_BottomThumb);
            }
        }

        private void SetEleAfterResize(Shape oRect)
        {
            if (!(selectdeobject is Line))
            {
                _LeftThumb.Margin = new Thickness(oRect.Margin.Left - 4, oRect.Margin.Top + oRect.Height / 2.0 - 6, 0, 0);
                _RightThumb.Margin = new Thickness(oRect.Margin.Left + oRect.Width - 6, oRect.Margin.Top + oRect.Height / 2.0 - 6, 0, 0);
                _TopThumb.Margin = new Thickness(oRect.Margin.Left + oRect.Width / 2.0 - 6, oRect.Margin.Top - 4, 0, 0);
                _BottomThumb.Margin = new Thickness(oRect.Margin.Left + oRect.Width / 2.0 - 6, oRect.Margin.Top + oRect.Height - 4, 0, 0);
            }

        }

        private void SetResizeEllipeProperty(Thumb eleps)
        {
            eleps.DragDelta += new DragDeltaEventHandler(Ellipse_DragDelta);
            eleps.DragStarted += new DragStartedEventHandler(eleps_DragStarted);

            eleps.DragCompleted += new DragCompletedEventHandler(eleps_DragCompleted);
            eleps.Width = 10;
            eleps.Height = 10;

        }

        void eleps_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb oEle = ((Thumb)sender);

            Shape oRect = (Shape)oEle.Tag;
            double tempwidth = Math.Abs(oRect.ActualWidth - _PreviouWidth);
            double tempheight = Math.Abs(oRect.ActualHeight - _PreviouHeight);

            if (tempwidth > 6 || tempheight > 6)
            {
                ChangeRepresentationObject ChangeRepresentationObjectOfResize = UnDoObject.MakeChangeRepresentationObjectForResize(_PreviouMargin, _PreviouWidth, _PreviouHeight, (FrameworkElement)selectdeobject);
                UnDoObject.InsertObjectforUndoRedo(ChangeRepresentationObjectOfResize);
            }
        }


        double _PreviouWidth = 0;
        double _PreviouHeight = 0;
        Point _PreviouMargin;

        void eleps_DragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb oEle = ((Thumb)sender);
            Shape oRect = (Shape)oEle.Tag;
            _PreviouWidth = oRect.Width;
            _PreviouHeight = oRect.Height;
            _PreviouMargin = new Point(((FrameworkElement)oRect).Margin.Left, ((FrameworkElement)oRect).Margin.Top);
        }


        void Ellipse_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb oEle = ((Thumb)sender);
            Shape oRect = (Shape)oEle.Tag;
            double tempwidth, tempheight, prevwidth = oRect.Width, prevheight = oRect.Height;

            switch (oEle.Name)
            {
                case "Right":
                    tempwidth = oRect.ActualWidth + e.HorizontalChange;

                    if (tempwidth > 6 && oRect.Margin.Left + tempwidth <= this.Width)
                    {
                        oRect.Width = tempwidth;
                    }
                    break;

                case "Left":
                    tempwidth = oRect.ActualWidth - e.HorizontalChange;

                    if (tempwidth > 6 && oRect.Margin.Left + e.HorizontalChange > 0)
                    {
                        oRect.Margin = new Thickness(oRect.Margin.Left + e.HorizontalChange, oRect.Margin.Top, 0, 0);
                        oRect.Width = tempwidth;
                    }
                    break;

                case "Top":
                    tempheight = oRect.ActualHeight - e.VerticalChange;

                    if (tempheight > 6 && oRect.Margin.Top + e.VerticalChange > 0)
                    {
                        oRect.Margin = new Thickness(oRect.Margin.Left, oRect.Margin.Top + e.VerticalChange, 0, 0);

                        oRect.Height = tempheight;
                    }
                    break;
                case "Bottom":

                    tempheight = oRect.ActualHeight + e.VerticalChange;
                    if (tempheight > 6 && tempheight + oRect.Margin.Top <= this.Height)
                    {
                        oRect.Height = tempheight;
                    }
                    break;

                default:
                    break;
            }


            SetEleAfterResize(oRect);


        }

        #endregion

        #region Object Move
        bool isDragging;
        FrameworkElement elDragging;
        Point ptMouseStart, ptElementStart;
        double startHeight, startWidth;
        void Rect_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ptMouseStart = e.GetPosition(this);
            elDragging = (this).InputHitTest(ptMouseStart) as FrameworkElement;
            if (elDragging == null) return;

            if (elDragging != null && elDragging is System.Windows.Shapes.Shape)
            {
                ptElementStart = new Point(elDragging.Margin.Left, elDragging.Margin.Top);
                startHeight = ((Shape)elDragging).Height;
                startWidth = ((Shape)elDragging).Width;
                Mouse.Capture(((Shape)elDragging));
                isDragging = true;
                ((Shape)elDragging).Cursor = Cursors.ScrollAll;
            }
        }

        void Rect_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (((Shape)elDragging) == null) return;

            isDragging = false;
            ((Shape)elDragging).Cursor = Cursors.Arrow;
            ((Shape)elDragging).ReleaseMouseCapture();
        }

        void Rect_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (((Shape)elDragging) == null) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point ptMouse = e.GetPosition(this);
                // Move and resize the  Rectangle
                if (isDragging)
                {
                    if (elDragging == null)
                        elDragging = ((Shape)elDragging);
                    double left = ptElementStart.X + ptMouse.X - ptMouseStart.X;
                    double top = ptElementStart.Y + ptMouse.Y - ptMouseStart.Y;
                    if (left > 0 && top > 0 && (left + elDragging.Width) < this.Width && (top + elDragging.Height) < this.Height)
                    {
                        elDragging.Margin = new Thickness(left, top, 0, 0);


                    }
                }
            }
        }
        #endregion
    }
}

