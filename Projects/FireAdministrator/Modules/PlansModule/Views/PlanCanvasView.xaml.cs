using System;
using Infrastructure;
using Infrastructure.Common;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Collections.Generic;
using FiresecClient;
using FiresecAPI.Models;
using PlansModule.ViewModels;
using System.Windows.Media.Imaging;
using FiresecAPI.Models.Plans;


namespace PlansModule.Views
{

    public partial class PlanCanvasView : UserControl
    {
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;

        public static PlanCanvasView Current { get; set; }
        private Plan Plan { get; set; }
        private int idElement;
        string typeElement;

        private bool ActiveElement;
        private string UpdateElement;
        private bool _isDown;
        private bool _isResize;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _originalElement;
        private TextBox _originalElementTextBox;
        private PolygonAdorner _overlayElementPolygon;
        private RectangleAdorner _overlayElementRectangle;
        private TextBoxAdorner _overlayElementTexBox;
        private double _originalTop;
        private double _originalLeft;
        public static double dTop = 30;

        public PlanCanvasView()
        {
            Current = this;

            InitializeComponent();
        }

        public void ChangeSelectedPlan(Plan plan)
        {
            Plan = plan;
            idElement = 0;

            MainCanvas.Children.Clear();
            //MainCanvas.ActualWidth = plan.Width;
            //MainCanvas.ActualHeight= plan.Height;
            //MainCanvas.Width = MainCanvas.ActualWidth;
            //MainCanvas.Height = MainCanvas.ActualHeight;
            MainCanvas.Width = plan.Width;
            MainCanvas.Height = plan.Height;
            var imageBrush = new ImageBrush();
            BitmapImage image = null;
            if (plan.BackgroundPixels != null)
            {

                using (MemoryStream imageStream = new MemoryStream(plan.BackgroundPixels))
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = imageStream;
                    image.EndInit();
                }
                imageBrush.ImageSource = image;
                //if (image.Width>plan.Width) MainCanvas.Width = image.Width;
                //if (image.Height > plan.Height) MainCanvas.Height = image.Height;
            }

            //MainCanvas.Width = image.Width;
            //MainCanvas.Height = image.Height;

            Rectangle rectangle = new Rectangle();
            //rectangle.Name = "canv" + idElement.ToString();
            //rectangle.Height = MainCanvas.ActualHeight;
            //rectangle.Width = MainCanvas.ActualWidth;
            //rectangle.Height = image.Height;
            //rectangle.Width = image.Width;
            //rectangle.Fill = imageBrush;
            //Canvas.SetLeft(rectangle, 0);
            //Canvas.SetTop(rectangle, 0);
            //MainCanvas.Children.Add(rectangle);
            if (plan.Rectangls != null)
            {
                foreach (var rect in plan.Rectangls)
                {
                    var imageBrushRect = new ImageBrush();
                    rectangle = new Rectangle();
                    rect.idElementCanvas = idElement;
                    rectangle.Name = "rect" + idElement.ToString();
                    rectangle.Height = rect.Height;
                    rectangle.Width = rect.Width;
                    //BitmapImage image;
                    using (MemoryStream imageStream = new MemoryStream(rect.BackgroundPixels))
                    {
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = imageStream;
                        image.EndInit();
                    }

                    imageBrushRect.ImageSource = image;
                    rectangle.Fill = imageBrushRect;
                    Canvas.SetLeft(rectangle, rect.Left);
                    Canvas.SetTop(rectangle, rect.Top);
                    //MainCanvas.Children.Add(rectangle);
                    MainCanvas.Children.Add(rectangle);
                    idElement++;
                }
            }

            if (plan.ElementZones != null)
            {
                foreach (var zona in plan.ElementZones)
                {
                    Polygon myPolygon = new Polygon();
                    zona.idElementCanvas = idElement;
                    myPolygon.Name = "zona" + idElement.ToString();
                    myPolygon.ToolTip = "Зона №" + zona.ZoneNo;
                    myPolygon.Stroke = System.Windows.Media.Brushes.Black;
                    myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
                    myPolygon.StrokeThickness = 2;
                    PointCollection myPointCollection = new PointCollection();
                    var point = new Point();
                    double minX = -1;
                    double minY = -1;
                    foreach (var _point in zona.PolygonPoints)
                    {
                        if ((minX > _point.X) || (minX == -1)) minX = _point.X;
                        if ((minY > _point.Y) || (minY == -1)) minY = _point.Y;
                        point.X = _point.X;
                        point.Y = _point.Y;
                        myPointCollection.Add(point);
                    }
                    myPolygon.Points = myPointCollection;
                    //MainCanvas.Children.Add(myPolygon);
                    MainCanvas.Children.Add(myPolygon);
                    Canvas.SetLeft(myPolygon, 0);
                    Canvas.SetTop(myPolygon, 0);
                    idElement++;
                }
            }
            if (plan.TextBoxes != null)
            {
                foreach (var text in plan.TextBoxes)
                {
                    TextBox textbox = new TextBox();
                    text.idElementCanvas = idElement;
                    textbox.Name = "text" + idElement.ToString();
                    textbox.Text = text.Text;
                    textbox.AllowDrop = true;
                    textbox.IsReadOnly = true;
                    Canvas.SetLeft(textbox, text.Left);
                    Canvas.SetTop(textbox, text.Top);
                    //MainCanvas.Children.Add(textbox);
                    MainCanvas.Children.Add(textbox);
                    idElement++;
                }
            }
            if (plan.ElementDevices != null)
            {
                foreach (var device in plan.ElementDevices)
                {
                    rectangle = new Rectangle();
                    device.idElementCanvas = idElement;
                    rectangle.Name = "devs" + idElement.ToString();
                    rectangle.ToolTip = "Устройство";
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Colors.Green;
                    rectangle.Fill = brush;
                    rectangle.Height = device.Height;
                    rectangle.Width = device.Width;
                    Canvas.SetLeft(rectangle, device.Left);
                    Canvas.SetTop(rectangle, device.Top);
                    //MainCanvas.Children.Add(rectangle);
                    MainCanvas.Children.Add(rectangle);
                    idElement++;
                }
            }
        }

        private void ClearAllSelected()
        {
            //MainCanvas.Cursor = Cursors.Arrow;
            MainCanvas.Cursor = Cursors.Arrow;
            //foreach (var element in MainCanvas.Children)
            foreach (var element in MainCanvas.Children)
            {
                if (element is Thumb)
                {
                    //MainCanvas.Children.Remove((UIElement)element);
                    MainCanvas.Children.Remove((UIElement)element);
                    ClearAllSelected();
                    break;
                }
            }
        }

        private void GetTypeElement()
        {
            if (_originalElement is Polygon)
            {
                Polygon polygon = (_originalElement as Polygon);
                typeElement = polygon.Name.Substring(0, 4);
            }
            if (_originalElement is Rectangle)
            {
                Rectangle rectangle = (_originalElement as Rectangle);
                typeElement = rectangle.Name.Substring(0, 4);
            }
        }

        private void DragStarted()
        {
            if (_originalElement != null)
            {
                _isDragging = true;

                _originalLeft = Canvas.GetLeft(_originalElement);
                _originalTop = Canvas.GetTop(_originalElement);
                if (_originalElement is Polygon)
                {
                    GetTypeElement();
                    _overlayElementPolygon = new PolygonAdorner(_originalElement);
                    if (!_isResize) //перемещение
                    {
                        _overlayElementPolygon.SetOperationMove(true);
                    }
                    else
                    {
                        _overlayElementPolygon.SetOperationMove(false);
                    }
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                    layer.Add(_overlayElementPolygon);
                };
                if (_originalElement is Rectangle)
                {
                    GetTypeElement();
                    _overlayElementRectangle = new RectangleAdorner(_originalElement);
                    if (!_isResize) //перемещение
                    {
                        _overlayElementRectangle.SetOperationMove(true);
                    }
                    else
                    {
                        _overlayElementRectangle.SetOperationMove(false);
                    }
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                    layer.Add(_overlayElementRectangle);
                };
            }
        }
        private void DragMoved()
        {
            if (_overlayElementPolygon != null)
            {
                _overlayElementPolygon.Cursor = Cursors.Cross;
                //Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementPolygon.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementPolygon.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            };
            if (_overlayElementRectangle != null)
            {
                _overlayElementRectangle.Cursor = Cursors.Cross;
                //Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);

                _overlayElementRectangle.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementRectangle.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            }
            if (_overlayElementTexBox != null)
            {
                _overlayElementTexBox.Cursor = Cursors.Cross;
                //Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementTexBox.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementTexBox.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            }


        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var element = e.Source;
            if (_isDown && (ActiveElement != null))
            {
                if (_isDragging == false)
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
            else
            {
                //if (e.Source != this.MainCanvas)
                if (e.Source != this.MainCanvas)
                {
                    bool IsCanavs = false;
                    if (e.Source is Rectangle)
                    {
                        var rect = e.Source as Rectangle;
                        if (rect.Name == "canv0")
                        {
                            IsCanavs = true;
                            _originalElementTextBox = null;
                            ActiveElement = false;
                            ClearAllSelected();

                        }
                        else IsCanavs = false;
                    }
                    if (!IsCanavs)
                    {
                        if (e.Source is TextBox)
                        {
                            TextBox textbox = e.Source as TextBox;
                            _originalElementTextBox = textbox;
                            Rectangle rectangle = new Rectangle();
                            rectangle.Name = "textbox";
                            rectangle.Height = textbox.ActualHeight;
                            rectangle.Width = textbox.ActualWidth;
                            SolidColorBrush brush = new SolidColorBrush();
                            rectangle.Fill = brush;
                            Canvas.SetLeft(rectangle, Canvas.GetLeft(textbox));
                            Canvas.SetTop(rectangle, Canvas.GetTop(textbox));
                            //MainCanvas.Children.Add(rectangle);
                            MainCanvas.Children.Add(rectangle);
                        }
                        else
                        {
                            if (e.Source is Rectangle)
                            {
                                MainCanvasViewModel canvasViewModel = new MainCanvasViewModel((UIElement)e.Source);
                                ActiveElement = true;
                            }
                            else
                            {
                                MainCanvasViewModel canvasViewModel = new MainCanvasViewModel((UIElement)e.Source);
                                ActiveElement = true;
                                _originalElementTextBox = null;
                            }
                        }
                    }
                    else
                    {
                        _originalElementTextBox = null;
                        ActiveElement = false;
                        ClearAllSelected();
                    }

                }
                else
                {
                    _originalElementTextBox = null;
                    ActiveElement = false;
                    ClearAllSelected();
                }
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (ActiveElement)
            {
                _isDown = true;
                _isResize = false;
                //_startPoint = Mouse.GetPosition(MainCanvas);
                _startPoint = Mouse.GetPosition(MainCanvas);
                _startPoint.Y = _startPoint.Y + PlanCanvasView.dTop;
                double x = scaleTransform.CenterX;
                double y = scaleTransform.CenterY;
                double v = slider.Value;
                _originalElement = e.Source as UIElement;
                e.Handled = true;
            }
        }
        public void DragFinished(bool cancelled)
        {
            if (_isDragging)
            {
                if (_originalElement is Polygon)
                {
                    UpdateElement = (_originalElement as Polygon).Name;
                    AdornerLayer.GetAdornerLayer(_overlayElementPolygon.AdornedElement).Remove(_overlayElementPolygon);
                    if (cancelled == false)
                    {
                        Canvas.SetTop(_originalElement, _originalTop + _overlayElementPolygon.TopOffset / ZoomValue + PlanCanvasView.dTop);
                        Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementPolygon.LeftOffset / ZoomValue);
                    }
                }
                if (_originalElement is Rectangle)
                {
                    var rect = (_originalElement as Rectangle);
                    string name = rect.Name;
                    if (name != "textbox")
                    {
                        UpdateElement = (_originalElement as Rectangle).Name;
                        AdornerLayer.GetAdornerLayer(_overlayElementRectangle.AdornedElement).Remove(_overlayElementRectangle);
                        if (cancelled == false)
                        {
                            Canvas.SetTop(_originalElement, _originalTop + _overlayElementRectangle.TopOffset / ZoomValue + PlanCanvasView.dTop);
                            Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementRectangle.LeftOffset / ZoomValue);
                        }
                    }
                    else
                    {
                        if (_originalElementTextBox != null)
                        {
                            UpdateElement = (_originalElementTextBox as TextBox).Name;
                            AdornerLayer.GetAdornerLayer(_overlayElementRectangle.AdornedElement).Remove(_overlayElementRectangle);
                            if (cancelled == false)
                            {
                                Double d = ZoomValue;
                                Canvas.SetTop(_originalElementTextBox, _originalTop + _overlayElementRectangle.TopOffset / ZoomValue + PlanCanvasView.dTop);
                                Canvas.SetLeft(_originalElementTextBox, _originalLeft + _overlayElementRectangle.LeftOffset / ZoomValue);
                            }
                        }
                        //MainCanvas.Children.Remove(_originalElement);
                        MainCanvas.Children.Remove(_originalElement);
                    }
                }
            }
            UpdatePlan();
            _isDragging = false;
            _isDown = false;

        }

        private void UpdatePlan()
        {
            if (UpdateElement != null)
            {
                int index = int.Parse(UpdateElement.Substring(4));
                switch (typeElement)
                {
                    case "rect":
                        foreach (var rect in Plan.Rectangls)
                        {
                            if (_overlayElementRectangle != null)
                            {
                                if (rect.idElementCanvas == index)
                                {
                                    rect.Left = rect.Left + _overlayElementRectangle.LeftOffset;
                                    rect.Top = rect.Top + _overlayElementRectangle.TopOffset + PlanCanvasView.dTop;
                                }
                            }
                        }
                        break;
                    case "devs":
                        foreach (var device in Plan.ElementDevices)
                        {
                            if (_overlayElementRectangle != null)
                            {
                                if (device.idElementCanvas == index)
                                {
                                    device.Left = device.Left + _overlayElementRectangle.LeftOffset;
                                    device.Top = device.Top + _overlayElementRectangle.TopOffset + PlanCanvasView.dTop;
                                }
                            }
                        }
                        break;
                    case "text":
                        foreach (var text in Plan.TextBoxes)
                        {
                            if (_originalElementTextBox != null)
                            {
                                if (text.idElementCanvas == index)
                                {
                                    text.Left = Canvas.GetLeft(_originalElementTextBox);
                                    text.Top = Canvas.GetTop(_originalElementTextBox);
                                }
                            }
                        }
                        break;
                    case "zona":
                        foreach (var zona in Plan.ElementZones)
                        {
                            if (zona.idElementCanvas == index)
                            {
                                if (_overlayElementPolygon != null)
                                {
                                    PointCollection PointCollection = new PointCollection();
                                    foreach (var point in zona.PolygonPoints)
                                    {
                                        Point Point = new Point();
                                        Point.X = point.X + _overlayElementPolygon.LeftOffset;
                                        Point.Y = point.Y + _overlayElementPolygon.TopOffset + PlanCanvasView.dTop;
                                        PointCollection.Add(Point);
                                    }
                                    zona.PolygonPoints = PointCollection;
                                }
                            }
                        }

                        break;
                }
                string type = typeElement;
                UIElement element = _originalElement;
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
                ClearAllSelected();
            }
        }
        double initialScale = 1;
        double ZoomValue = 1;

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (scaleTransform != null)
            {
                if (e.NewValue == 0)
                    return;
                ZoomValue = e.NewValue;
                scaleTransform.ScaleX = e.NewValue * initialScale;
                scaleTransform.ScaleY = e.NewValue * initialScale;

                var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, MainCanvas);
                if (ZoomValue != 1)
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
                else
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    FullSize();
                }
            }

        }

        private void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                // Ага!
                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                        //Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, MainCanvas);
                        Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, MainCanvas);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    //targetNow = Mouse.GetPosition(MainCanvas);
                    targetNow = Mouse.GetPosition(MainCanvas);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    //double multiplicatorX = e.ExtentWidth / MainCanvas.ActualWidth;
                    double multiplicatorX = e.ExtentWidth / MainCanvas.ActualWidth;
                    //double multiplicatorY = e.ExtentHeight / MainCanvas.ActualHeight;
                    double multiplicatorY = e.ExtentHeight / MainCanvas.ActualHeight;

                    double newOffsetX = scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }

        }

        void FullSize()
        {
            if (CanvasLoaded)
            {
                dResizeWindowX = scrollViewer.ActualWidth / MainCanvas.Width;
                dResizeWindowY = scrollViewer.ActualHeight / MainCanvas.Height;
                if (dResizeWindowX <= dResizeWindowY)
                {
                    scaleTransform.ScaleX = dResizeWindowX;
                    scaleTransform.ScaleY = dResizeWindowX;
                }
                else
                {
                    scaleTransform.ScaleX = dResizeWindowY;
                    scaleTransform.ScaleY = dResizeWindowY;
                }
                if (dResizeWindowX == 1 && ZoomValue == 1)
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
                if (dResizeWindowY == 1 && ZoomValue == 1)
                {
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
            }

        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FullSize();
        }

        double dResizeWindowX = 1;
        double dResizeWindowY = 1;
        bool CanvasLoaded = false;
        private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasLoaded = true;

            MainCanvas.Width = MainCanvas.ActualWidth;
            MainCanvas.Height = MainCanvas.ActualHeight;
        }

    }
}