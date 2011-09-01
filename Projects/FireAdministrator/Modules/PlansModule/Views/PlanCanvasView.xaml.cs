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
                    MainCanvas.Children.Add(myPolygon);
                    Canvas.SetLeft(myPolygon, 0);
                    Canvas.SetTop(myPolygon, 0);
                    idElement++;
                }
            }
            if (plan.Rectangls != null)
            {
                foreach (var rect in plan.Rectangls)
                {
                    var imageBrushRect = new ImageBrush();
                    Rectangle rectangle = new Rectangle();
                    rect.idElementCanvas = idElement;
                    rectangle.Name = "rect" + idElement.ToString();
                    rectangle.Height = rect.Height;
                    rectangle.Width = rect.Width;
                    BitmapImage image;
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
                    MainCanvas.Children.Add(rectangle);
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
                    MainCanvas.Children.Add(textbox);
                    idElement++;
                }
            }
            if (plan.ElementDevices != null)
            {
                foreach (var device in plan.ElementDevices)
                {
                    Rectangle rectangle = new Rectangle();
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
                    MainCanvas.Children.Add(rectangle);
                    idElement++;
                }
            }
            var imageBrush = new ImageBrush();
            if (plan.BackgroundPixels != null)
            {

                BitmapImage image;
                using (MemoryStream imageStream = new MemoryStream(plan.BackgroundPixels))
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = imageStream;
                    image.EndInit();
                }
                imageBrush.ImageSource = image;
            }

            MainCanvas.Background = imageBrush;
        }

        private void ClearAllSelected()
        {
            MainCanvas.Cursor = Cursors.Arrow;
            foreach (var element in MainCanvas.Children)
            {
                if (element is Thumb)
                {
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
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementPolygon.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementPolygon.TopOffset = CurrentPosition.Y - _startPoint.Y;
            };
            if (_overlayElementRectangle != null)
            {
                _overlayElementRectangle.Cursor = Cursors.Cross;
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementRectangle.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementRectangle.TopOffset = CurrentPosition.Y - _startPoint.Y;
            }
            if (_overlayElementTexBox != null)
            {
                _overlayElementTexBox.Cursor = Cursors.Cross;
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementTexBox.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementTexBox.TopOffset = CurrentPosition.Y - _startPoint.Y;
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
                if (e.Source != this.MainCanvas)
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
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (ActiveElement)
            {
                _isDown = true;
                _isResize = false;
                _startPoint = e.GetPosition(this);
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
                        Canvas.SetTop(_originalElement, _originalTop + _overlayElementPolygon.TopOffset + PlanCanvasView.dTop);
                        Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementPolygon.LeftOffset);
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
                            Canvas.SetTop(_originalElement, _originalTop + _overlayElementRectangle.TopOffset + PlanCanvasView.dTop);
                            Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementRectangle.LeftOffset);
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
                                Canvas.SetTop(_originalElementTextBox, _originalTop + _overlayElementRectangle.TopOffset + PlanCanvasView.dTop);
                                Canvas.SetLeft(_originalElementTextBox, _originalLeft + _overlayElementRectangle.LeftOffset);
                            }
                        }
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
                                    rect.Top = rect.Top + _overlayElementRectangle.TopOffset+ PlanCanvasView.dTop;
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
                                    device.Top = device.Top + _overlayElementRectangle.TopOffset+ PlanCanvasView.dTop;
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
                                    PointCollection PointCollection= new PointCollection();
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
    }
}