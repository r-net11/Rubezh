using System;
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

namespace PlansModule.Views
{
    public partial class PlanCanvasView : UserControl
    {
        public static PlanCanvasView Current { get; set; }
        private bool IsActiveElement;
        private bool _isDown;
        private bool _isResize;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _originalElement;
        private PolygonAdorner _overlayElementPolygon;
        private double _originalTop;
        private double _originalLeft;


        public PlanCanvasView()
        {
            Current = this;
            InitializeComponent();
        }

        public void ChangeSelectedPlan(Plan plan)
        {
            MainCanvas.Children.Clear();
            
            foreach (var zona in plan.ElementZones)
            {
                Polygon myPolygon = new Polygon();
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
                    if ((minX < _point.X)||(minX==-1)) minX=_point.X;
                    if ((minY< _point.Y) || (minY == -1)) minY = _point.Y;
                    point.X = _point.X;
                    point.Y = _point.Y;
                    myPointCollection.Add(point);
                }
                myPolygon.Points = myPointCollection;
                MainCanvas.Children.Add(myPolygon);
                Canvas.SetLeft(myPolygon, 0);
                Canvas.SetTop(myPolygon, 0);
            }
            foreach (var rect in plan.Rectangls)
            {
                var imageBrushRect = new ImageBrush();
                Rectangle rectangle = new Rectangle();
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
                Canvas.SetLeft(rectangle,rect.Left);
                Canvas.SetTop(rectangle, rect.Top);

                MainCanvas.Children.Add(rectangle);
            }
            foreach (var text in plan.TextBoxes)
            {
                TextBox textbox = new TextBox();
                textbox.Text = text.Text;
                Canvas.SetLeft(textbox, text.Left);
                Canvas.SetTop(textbox, text.Top);
                MainCanvas.Children.Add(textbox);
            }
            foreach (var device in plan.ElementDevices)
            {
                Polygon myPolygon = new Polygon();
                myPolygon.ToolTip = "Устройство № (не реализован shapeId)";
                myPolygon.Stroke = System.Windows.Media.Brushes.Red;
                myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
                myPolygon.StrokeThickness = 2;
                PointCollection myPointCollection = new PointCollection();
                double minX = device.Left;
                double minY = device.Top;
                var point = new Point(device.Left,device.Top);
                myPointCollection.Add(point);
                point = new Point(device.Left+device.Width, device.Top);
                myPointCollection.Add(point);
                point = new Point(device.Left + device.Width, device.Top+device.Height);
                myPointCollection.Add(point);
                point = new Point(device.Left , device.Top + device.Height);
                myPointCollection.Add(point);
                myPolygon.Points = myPointCollection;
                MainCanvas.Children.Add(myPolygon);
                Canvas.SetLeft(myPolygon, 0);
                Canvas.SetTop(myPolygon, 0);
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
            /*
            Polygon testPolygon = new Polygon();
            testPolygon.ToolTip = "Устройство № (не реализован shapeId)";
            testPolygon.Stroke = System.Windows.Media.Brushes.Red;
            testPolygon.Fill = imageBrush;//System.Windows.Media.Brushes.LightSeaGreen;
            testPolygon.StrokeThickness = 2;
            PointCollection testPointCollection = new PointCollection();
            var point1 = new Point(100, 100);
            testPointCollection.Add(point1);
            point1 = new Point(300, 100);
            testPointCollection.Add(point1);
            point1 = new Point(300, 300);
            testPointCollection.Add(point1);
            point1 = new Point(100, 300);
            testPointCollection.Add(point1);
            testPolygon.Points = testPointCollection;
            MainCanvas.Children.Add(testPolygon);
            Canvas.SetLeft(testPolygon, 0);
            Canvas.SetTop(testPolygon, 0);

            */

            MainCanvas.Background = imageBrush;
        }

        private void ClearAllSelected()
        {
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

        private void DragStarted()
        {
            if (_originalElement != null)
            {
                _isDragging = true;

                _originalLeft = Canvas.GetLeft(_originalElement);
                _originalTop = Canvas.GetTop(_originalElement);
                _overlayElementPolygon = new PolygonAdorner(_originalElement);
                if (!_isResize) //перемещение
                {
                    _overlayElementPolygon.SetOperationMove(true);
                }
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                layer.Add(_overlayElementPolygon);
            }
        }
        private void DragMoved()
        {
            if (_overlayElementPolygon != null)
            {
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementPolygon.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementPolygon.TopOffset = CurrentPosition.Y - _startPoint.Y;
            }

        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var element = e.Source;
            if (_isDown && IsActiveElement)
            {
                /*
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(MainCanvas).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(MainCanvas).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                 * */
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
                    MainCanvasViewModel caonvasViewModel = new MainCanvasViewModel((UIElement)e.Source);
                    IsActiveElement = true;
                }
                else
                {
                    IsActiveElement = false;
                    ClearAllSelected();
                }
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (IsActiveElement)
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
            if (_isDragging && _overlayElementPolygon != null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElementPolygon.AdornedElement).Remove(_overlayElementPolygon);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElementPolygon.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementPolygon.LeftOffset);
                }
            }
            _isDragging = false;
            _isDown = false;

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