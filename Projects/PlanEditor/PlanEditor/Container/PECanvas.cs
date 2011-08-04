using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Markup;
using System.Windows.Input;


namespace PlanEditor
{
    public partial class PECanvas_old : Canvas
    {
        #region Declaration

        public event EventHandler CompleteDraw;

        #endregion

        #region Properties

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


        Thumb thumb = null;
        public static ListObjects listShapes = new ListObjects();
        private bool _isDown;
        private bool _isResize;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _originalElement;
        private LineAdorner _overlayElementLine;
        private RectangleAdorner _overlayElementRect;
        private EllipseAdorner _overlayElementEllipse;
        Rect resultRect;
        private double _originalTop;
        private double _originalLeft;
        private double _originalX;
        private double _originalY;
        

        
        public Canvas GetCanvas()
        {
            return this;
        }
        
        
        
        public bool GetDragging()
        {
            return _isDragging;
        }
        
     
        
        #region Constructors

        public PECanvas_old()
        {

            this.Background = Brushes.LightBlue;
            this.Height = 490;
            this.Width = 490;

            //canvas = new Canvas();
            this.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MyMouseLeftButtonDown);
            this.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(MyPreviewMouseMove);
            this.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MouseLeftButtonUp);
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
            _isDragging = false;
            _isDown = false;
            _isResize = false;

        }
        void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false, e.Source.GetHashCode(), this);
                e.Handled = true;
            }
            if (_isResize)
            {
                foreach (Object obj in listShapes)
                {
                    if (obj is PlanEditor.PELine)
                    {
                        PELine line = (PELine)obj;
                        if (line.active)
                        {
                            Line l = (Line)line.GetShape();
                            double x = Math.Abs(e.GetPosition(this).X) - Canvas.GetLeft(l);
                            double y = Math.Abs(e.GetPosition(this).Y) - Canvas.GetTop(l);

                            if (thumb.Name.Equals("thumb2"))
                            {
                                line.X2 = x;
                                line.Y2 = y;
                            }
                            if (thumb.Name.Equals("thumb1"))
                            {
                                line.X1 = x;
                                line.Y1 = y;
                            }
                        }
                        _overlayElementLine = null;
                    }
                    if (obj is PlanEditor.PERectangle)
                    {
                        PERectangle rect = (PERectangle)obj;
                        if (rect.active)
                        {
                            //Rectangle r = (Rectangle)rect.GetShape();
                            double tmpX = Math.Abs(e.GetPosition(this).X);
                            double tmpY = Math.Abs(e.GetPosition(this).Y);
                            Rect r = _overlayElementRect.GetResultRect();
                            rect.Width = r.Width;
                            rect.Height = r.Height;
                            rect.X = r.X;
                            rect.Y = r.Y;
                            _overlayElementRect = null;
                        }
                    }
                }


            }
        }
        public void DragFinished(bool cancelled, int _code, Canvas canvas)
        {
            System.Windows.Input.Mouse.Capture(null);
            if (_isDragging && _overlayElementLine != null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElementLine.AdornedElement).Remove(_overlayElementLine);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElementLine.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementLine.LeftOffset);
                    listShapes.DragFinished(_code, canvas, _originalTop + _overlayElementLine.TopOffset, _originalLeft + _overlayElementLine.LeftOffset);
                }
                //_overlayElementLine = null;
            }
            if (_isDragging && _overlayElementRect != null )
            {
                AdornerLayer.GetAdornerLayer(_overlayElementRect.AdornedElement).Remove(_overlayElementRect);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElementRect.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementRect.LeftOffset);
                    listShapes.DragFinished(_code, canvas, _originalTop + _overlayElementRect.TopOffset, _originalLeft + _overlayElementRect.LeftOffset);
                }
                //_overlayElementRect = null;
            }
            if (_isDragging && _overlayElementEllipse!= null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElementEllipse.AdornedElement).Remove(_overlayElementEllipse);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElementEllipse.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementEllipse.LeftOffset);
                    listShapes.DragFinished(_code, canvas, _originalTop + _overlayElementEllipse.TopOffset, _originalLeft + _overlayElementEllipse.LeftOffset);
                }
                //_overlayElementRect = null;
            }

            _isDragging = false;
            _isDown = false;
            //_isResize = false;
            //listShapes.SetActiveLineToResize(false);
        }
  
        void MyPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

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
                    DragMoved(e.Source.GetHashCode(), this);
                }
            }

        }
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
                            
                            //MessageBox.Show(ellipse.X.ToString());

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

        private void DragMoved(int _code, Canvas canvas)
        {
            if (_overlayElementLine != null)
            {
                Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(canvas);
                _overlayElementLine.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementLine.TopOffset = CurrentPosition.Y - _startPoint.Y;
                listShapes.DragMove(_code, canvas);
            }
            if (_overlayElementRect != null)
            {
                Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(canvas);
                _overlayElementRect.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementRect.TopOffset = CurrentPosition.Y - _startPoint.Y;
                listShapes.DragMove(_code, canvas);
            }
            if (_overlayElementEllipse != null)
            {
                Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(canvas);
                _overlayElementEllipse.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementEllipse.TopOffset = CurrentPosition.Y - _startPoint.Y;
                listShapes.DragMove(_code, canvas);
            }
        }

        void MyMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            string drawingCode = "";
            if (e.Source == this)
            {
                //_startPoint = e.GetPosition(canvas);
            }
            else
            {
                if (e.Source is System.Windows.Controls.Primitives.Thumb)
                {
                    _isDown = true;
                    _isResize = true;
                    thumb = (Thumb)e.Source;
                    //UIElement element=null;
                    foreach (Object obj in listShapes)
                    {
                        if (obj is PlanEditor.PELine)
                        {
                            PELine l = (PELine)obj;
                            if (l.active)
                            {
                                Line line = new Line();
                                line.X1 = 0;
                                line.Y1 = 0;
                                line.X2 = 10;
                                line.Y2 = 10;
                                double tmpX = Mouse.GetPosition(this).X;
                                double tmpY = Mouse.GetPosition(this).X;
                                this.Children.Add(line);
                                _startPoint = e.GetPosition(this);
                                _originalElement = line as UIElement;
                                this.CaptureMouse();
                                e.Handled = true;
                                break;
                            }
                        }
                        if (obj is PlanEditor.PERectangle)
                        {
                            PERectangle PERect = (PERectangle)obj;
                            if (PERect.active)
                            {
                                Rectangle rect = new Rectangle();
                                rect.Height = PERect.Height;
                                rect.Width = PERect.Width;
                                Canvas.SetLeft(rect, Canvas.GetLeft(PERect));
                                Canvas.SetTop(rect, Canvas.GetTop(PERect));
                                this.Children.Add(rect);
                                _startPoint = e.GetPosition(this);
                                _originalElement = rect as UIElement;
                                this.CaptureMouse();
                                e.Handled = true;
                                break;
                            }
                        }

                        if (obj is PlanEditor.PEEllipse)
                        {
                            PEEllipse PEellipse = (PEEllipse)obj;
                            if (PEellipse.active)
                            {
                                Ellipse ellipse = new Ellipse();
                                ellipse.Height = PEellipse.Height;
                                ellipse.Width = PEellipse.Width;
                                Canvas.SetLeft(ellipse, Canvas.GetLeft(PEellipse));
                                Canvas.SetTop(ellipse, Canvas.GetTop(PEellipse));
                                this.Children.Add(ellipse);
                                _startPoint = e.GetPosition(this);
                                _originalElement = ellipse as UIElement;
                                this.CaptureMouse();
                                e.Handled = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    _isDown = true;
                    _isResize = false;
                    _startPoint = e.GetPosition(this);
                    _originalElement = e.Source as UIElement;
                    this.CaptureMouse();
                    e.Handled = true;
                }
            }
        }

        public void AddShape(Object _object)
        {

            Type tLine = typeof(PlanEditor.PELine);
            Type tRect = typeof(PlanEditor.PERectangle);
            Type tEllipse = typeof(PlanEditor.PEEllipse);
            Type tTextBox = typeof(System.Windows.Controls.TextBox);

            if (_object.GetType() == tLine)
            {
                PELine line = (PELine)_object;
                this.Children.Add(line.GetShape());
            };
            if (_object.GetType() == tRect)
            {
                PERectangle rect = (PERectangle)_object;
                this.Children.Add(rect.GetShape());
            };
            if (_object.GetType() == tTextBox)
            {
                TextBox txt = (TextBox)_object;
                this.Children.Add(txt);
            };
            if (_object.GetType() == tEllipse)
            {
                PEEllipse ellipse = (PEEllipse)_object;
                this.Children.Add(ellipse.GetShape());
            };
            listShapes.Add(_object);

        }

    }
         
}
