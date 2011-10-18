using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FiresecAPI.Models;
using FiresecAPI.Models.Plans;
using PlansModule.Resize;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
    public partial class PlanCanvasView : UserControl
    {
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;

        public static PlanCanvasView Current { get; set; }
        public static ListBox PropertiesCaption;
        public static ListBox PropertiesValue;
        public static ListBox PropertiesType = new ListBox();
        public static ListBox PropertiesName = new ListBox();
        public static TabItem TabItem;
        public static string ElementProperties;
        Plan Plan { get; set; }
        int idElement;
        string typeElement;

        bool ActiveElement;
        UIElement CurrentElement;
        string UpdateElement;
        bool _isDown;
        bool _isResize;
        bool _isDragging;
        Point _startPoint;
        UIElement _originalElement;
        UIElement _originalElementToResize;
        TextBox _originalElementTextBox;
        PolygonAdorner _overlayElementPolygon;
        RectangleAdorner _overlayElementRectangle;
        TextBoxAdorner _overlayElementTexBox;
        double _originalTop;
        double _originalLeft;
        public static double dTop = 0;

        public PlanCanvasView()
        {
            Current = this;
            InitializeComponent();
        }

        public void ChangeSelectedPlan(Plan plan)
        {
            Plan = plan;
            MainCanvasViewModel.Plan = Plan;
            PropertiesView.plan = Plan;
            idElement = 0;

            MainCanvas.Children.Clear();

            MainCanvas.Width = plan.Width;
            MainCanvas.Height = plan.Height;

            var imageBrush = new ImageBrush();
            Rectangle rectangle = new Rectangle();
            if (plan.ElementRectangles != null)
            {
                foreach (var rect in plan.ElementRectangles)
                {
                    rectangle = new Rectangle();
                    ContextMenuCanvasViewModel contextMenu = new ContextMenuCanvasViewModel(GetPropertiesRect);
                    rectangle.ContextMenu = contextMenu.GetElement(rectangle, rect);
                    rect.idElementCanvas = idElement;
                    rectangle.Name = "rect" + idElement.ToString();
                    rectangle.Height = rect.Height;
                    rectangle.Width = rect.Width;
                    rectangle.MinWidth = 5;
                    rectangle.MinHeight = 5;

                    BitmapImage image = null;
                    using (var imageStream = new MemoryStream(rect.BackgroundPixels))
                    {
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = imageStream;
                        image.EndInit();
                    }

                    rectangle.Fill = new ImageBrush(image);
                    Canvas.SetLeft(rectangle, rect.Left);
                    Canvas.SetTop(rectangle, rect.Top);

                    MainCanvas.Children.Add(rectangle);
                    idElement++;
                }
            }

            if (plan.ElementZones != null)
            {
                foreach (var zona in plan.ElementZones)
                {
                    var contextMenu = new ContextMenuCanvasViewModel(GetPropertiesZona);
                    zona.idElementCanvas = idElement;
                    Polygon myPolygon = new Polygon();
                    myPolygon.Name = "zona" + idElement.ToString();
                    myPolygon.ToolTip = "Зона №" + zona.ZoneNo;
                    myPolygon.Stroke = System.Windows.Media.Brushes.Black;
                    myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
                    myPolygon.StrokeThickness = 2;
                    myPolygon.ContextMenu = contextMenu.GetElement(myPolygon, zona);
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
                    myPolygon.MinHeight = 5;
                    myPolygon.MinWidth = 5;
                    MainCanvas.Children.Add(myPolygon);
                    Canvas.SetLeft(myPolygon, 0);
                    Canvas.SetTop(myPolygon, 0);
                    idElement++;
                }
            }
            if (plan.ElementTextBlocks != null)
            {
                foreach (var text in plan.ElementTextBlocks)
                {
                    text.idElementCanvas = idElement;
                    var textbox = new TextBox()
                    {
                        Name = "text" + idElement.ToString(),
                        Text = text.Text,
                        AllowDrop = true,
                        IsReadOnly = true,
                        Tag = text,
                        MinHeight = 5,
                        MinWidth = 5
                    };

                    if (text.FontSize == 0)
                        text.FontSize = textbox.FontSize;
                    else
                        textbox.FontSize = text.FontSize;

                    Canvas.SetLeft(textbox, text.Left);
                    Canvas.SetTop(textbox, text.Top);
                    MainCanvas.Children.Add(textbox);
                    idElement++;
                }
            }

            if (plan.ElementDevices == null)
                return;

            foreach (var device in plan.ElementDevices)
            {
                device.idElementCanvas = idElement;
                rectangle = new Rectangle();
                rectangle.Name = "devs" + idElement.ToString();
                rectangle.ToolTip = "Устройство";
                rectangle.Fill = new SolidColorBrush(Colors.Green);
                rectangle.Height = device.Height;
                rectangle.Width = device.Width;
                rectangle.MinWidth = 5;
                rectangle.MinHeight = 5;
                Canvas.SetLeft(rectangle, device.Left);
                Canvas.SetTop(rectangle, device.Top);
                MainCanvas.Children.Add(rectangle);
                idElement++;
            }
        }

        void ClearPropertiesWindow()
        {
            PropertiesCaption.Items.Clear();
            PropertiesValue.Items.Clear();
            PropertiesType.Items.Clear();
        }

        void GetPropertiesZona(object sender, RoutedEventArgs e)
        {
            var Zona = sender as MenuItem;
            var test = Zona.Tag;
            ElementZone zona = Zona.Tag as ElementZone;
            TabItem.Header = "Свойства зоны";
            ClearPropertiesWindow();

            PropertiesCaption.Items.Add("Номер зоны");
            PropertiesValue.Items.Add(zona.ZoneNo);
            PropertiesName.Items.Add("zoneno");
            PropertiesType.Items.Add("string");

            PropertiesCaption.Items.Add("Полигон");
            string str = "";
            foreach (var point in zona.PolygonPoints)
            {
                str += "{" + point.X + "," + point.Y + "}";
            }
            PropertiesValue.Items.Add(zona.PolygonPoints);
            PropertiesName.Items.Add("polygonpoints");
            PropertiesType.Items.Add("Polygon");
            PropertiesView.IndexElement = zona.idElementCanvas;
        }

        void GetPropertiesRect(object sender, RoutedEventArgs e)
        {
            Rectangle rect = (sender as MenuItem).Tag as Rectangle;
            TabItem.Header = "Свойства прямоугольника";
            ClearPropertiesWindow();

            //PropertiesCaption.Items.Add("Оступ слева");
            //PropertiesValue.Items.Add(rect.Left);
            //PropertiesName.Items.Add("left");
            //PropertiesType.Items.Add("_number");

            //PropertiesCaption.Items.Add("Оступ сверху");
            //PropertiesValue.Items.Add(rect.Top);
            //PropertiesName.Items.Add("top");
            //PropertiesType.Items.Add("_number");

            PropertiesCaption.Items.Add("Ширина");
            PropertiesValue.Items.Add(rect.Width);
            PropertiesName.Items.Add("width");
            PropertiesType.Items.Add("_number");

            PropertiesCaption.Items.Add("Высота");
            PropertiesValue.Items.Add(rect.Height);
            PropertiesName.Items.Add("height");
            PropertiesType.Items.Add("_number");

            PropertiesCaption.Items.Add("Изображение");
            //PropertiesValue.Items.Add(rect.BackgroundPixels.Length.ToString());
            PropertiesName.Items.Add("background");
            PropertiesType.Items.Add("picture");
            //PropertiesView.IndexElement = rect.idElementCanvas;
        }

        void GetPropertiesText(object sender, RoutedEventArgs e)
        {
            ElementTextBlock text = (sender as MenuItem).Tag as ElementTextBlock;
            TabItem.Header = "Свойства текста";
            ClearPropertiesWindow();

            PropertiesCaption.Items.Add("Текст");
            PropertiesValue.Items.Add(text.Text);
            PropertiesName.Items.Add("text");
            PropertiesType.Items.Add("string");

            PropertiesCaption.Items.Add("Размер шрифта");
            PropertiesValue.Items.Add(text.FontSize);
            PropertiesName.Items.Add("fontsize");
            PropertiesType.Items.Add("_number");

            PropertiesCaption.Items.Add("Отступ слева");
            PropertiesValue.Items.Add(text.Left);
            PropertiesName.Items.Add("left");
            PropertiesType.Items.Add("_number");

            PropertiesCaption.Items.Add("Отступ сверху");
            PropertiesValue.Items.Add(text.Top);
            PropertiesName.Items.Add("top");
            PropertiesType.Items.Add("_number");

            PropertiesCaption.Items.Add("Цвет фона");
            PropertiesValue.Items.Add(text.Color);
            PropertiesName.Items.Add("color");
            PropertiesType.Items.Add("string");

            PropertiesCaption.Items.Add("Цвет бордюра");
            PropertiesValue.Items.Add(text.BorderColor);
            PropertiesName.Items.Add("bordercolor");
            PropertiesType.Items.Add("string");
            PropertiesView.IndexElement = text.idElementCanvas;
        }

        private void ClearAllSelected()
        {
            MainCanvas.Cursor = Cursors.Arrow;
            CurrentElement = null;
            foreach (var element in MainCanvas.Children)
            {
                if (element is Thumb)
                {
                    MainCanvas.Children.Remove((UIElement) element);
                    ClearAllSelected();
                    break;
                }
            }
        }

        void GetTypeElement()
        {
            if (_originalElement is Polygon)
                typeElement = (_originalElement as Polygon).Name.Substring(0, 4);
            else if (_originalElement is Rectangle)
                typeElement = (_originalElement as Rectangle).Name.Substring(0, 4);
        }

        void DragStarted()
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
                    _overlayElementPolygon.SetOperationMove(_isResize ? false : true);

                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                    layer.Add(_overlayElementPolygon);
                }
                if (_originalElement is Rectangle)
                {
                    GetTypeElement();
                    _overlayElementRectangle = new RectangleAdorner(_originalElement);
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                    layer.Add(_overlayElementRectangle);
                };
            }
        }

        void DragMoved()
        {
            if (_overlayElementPolygon != null)
            {
                _overlayElementPolygon.Cursor = Cursors.Cross;

                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementPolygon.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementPolygon.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            }
            if (_overlayElementRectangle != null)
            {
                _overlayElementRectangle.Cursor = Cursors.Cross;

                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementRectangle.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementRectangle.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            }
            if (_overlayElementTexBox != null)
            {
                _overlayElementTexBox.Cursor = Cursors.Cross;

                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementTexBox.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementTexBox.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            }
        }

        void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var element = e.Source;
            if (element is Thumb && Mouse.LeftButton == MouseButtonState.Pressed)
            {
            }
            else if (_isDown && ActiveElement)
            {
                if (_isDragging)
                    DragMoved();
                else
                    DragStarted();
            }
            else if (e.Source != MainCanvas)
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
                        CurrentElement = (UIElement) e.Source;
                    }
                    else
                    {
                        IsCanavs = false;
                    }
                }

                if (IsCanavs)
                {
                    _originalElementTextBox = null;
                    ActiveElement = false;
                    ClearAllSelected();
                }
                else if (!(e.Source is Thumb))
                {
                    ClearAllSelected();
                }
                else if (e.Source is TextBox)
                {
                    MainCanvasViewModel.OriginalElementToResizeTextBox = (e.Source as TextBox);
                    CurrentElement = (UIElement) e.Source;
                    _originalElementTextBox = (e.Source as TextBox);

                    var contextMenu = new ContextMenuCanvasViewModel(GetPropertiesText);
                    Rectangle rectangle = new Rectangle()
                    {
                        Name = "textbox",
                        ContextMenu = contextMenu.GetElement(e.Source as TextBox, (e.Source as TextBox).Tag),
                        Height = (e.Source as TextBox).ActualHeight,
                        Width = (e.Source as TextBox).ActualWidth
                    };
                    rectangle.Fill = new SolidColorBrush();

                    Canvas.SetLeft(rectangle, Canvas.GetLeft(e.Source as TextBox));
                    Canvas.SetTop(rectangle, Canvas.GetTop(e.Source as TextBox));
                    MainCanvas.Children.Add(rectangle);
                }
                else if (e.Source is Rectangle)
                {
                    CurrentElement = (UIElement) e.Source;
                    ActiveElement = true;
                }
                else
                {
                    CurrentElement = (UIElement) e.Source;
                    ActiveElement = true;
                    _originalElementTextBox = null;
                }
            }
            else
            {
                _originalElementTextBox = null;
                ActiveElement = false;
                ClearAllSelected();
            }
        }

        void MainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!ActiveElement)
                return;

            if (e.Source is Thumb)
            {
                _isDown = true;
                _isResize = true;
            }
            else
            {
                _isDown = true;
                _isResize = false;

                _startPoint = Mouse.GetPosition(MainCanvas);
                _startPoint.Y = _startPoint.Y + PlanCanvasView.dTop;
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
                        /*
                        Polygon Polygon = _originalElement as Polygon;
                        foreach (var _point in Polygon.Points)
                        {
                            Polygon.Points.
                            Point point = _point;
                            point.X = point.X + _originalLeft + _overlayElementPolygon.LeftOffset / ZoomValue;
                            point.Y = point.Y + _originalTop + _overlayElementPolygon.TopOffset / ZoomValue + PlanCanvasView.dTop;
                        }
                         * */
                        Canvas.SetTop(_originalElement, _originalTop + _overlayElementPolygon.TopOffset / ZoomValue + PlanCanvasView.dTop);
                        Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementPolygon.LeftOffset / ZoomValue);
                    }
                }
                else if (_originalElement is Rectangle)
                {
                    if ((_originalElement as Rectangle).Name != "textbox")
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
                                Canvas.SetTop(_originalElementTextBox, _originalTop + _overlayElementRectangle.TopOffset / ZoomValue + PlanCanvasView.dTop);
                                Canvas.SetLeft(_originalElementTextBox, _originalLeft + _overlayElementRectangle.LeftOffset / ZoomValue);
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

        public static void UpdateResizePlan(UIElement element, Plan plan)
        {
            PropertiesCaption.Items.Clear();
            PropertiesName.Items.Clear();
            PropertiesValue.Items.Clear();
            PropertiesType.Items.Clear();
            if (element is Polygon)
            {
                int index = int.Parse((element as Polygon).Name.Substring(4));
                foreach (var zona in plan.ElementZones)
                {
                    if (zona.idElementCanvas == index)
                    {
                        zona.ZoneNo = (element as Polygon).ToolTip.ToString().Substring(6);
                        zona.PolygonPoints.Clear();
                        foreach (var _point in (element as Polygon).Points)
                        {
                            Point point = _point;
                            point.X = point.X + Canvas.GetLeft(element as Polygon);
                            point.Y = point.Y + Canvas.GetTop(element as Polygon);
                            zona.PolygonPoints.Add(point);
                        }
                    }
                }
            }
            else if (element is Rectangle)
            {
                string updateElement = (element as Rectangle).Name;
                if (updateElement != "textbox")
                {
                    int index = int.Parse(updateElement.Substring(4));
                    foreach (var rect in plan.ElementRectangles)
                    {
                        if (rect.idElementCanvas == index)
                        {
                            rect.Height = (element as Rectangle).Height;
                            rect.Width = (element as Rectangle).Width;
                            rect.Left = Canvas.GetLeft(element as Rectangle);
                            rect.Top = Canvas.GetTop(element as Rectangle);
                        }
                    }
                }
                else
                {
                    var textBox = MainCanvasViewModel.OriginalElementToResizeTextBox as TextBox;
                    updateElement = textBox.Name;
                    int index = int.Parse(updateElement.Substring(4));
                    foreach (var text in plan.ElementTextBlocks)
                    {
                        if (text.idElementCanvas == index)
                        {
                            text.Left = Canvas.GetLeft(textBox);
                            text.Top = Canvas.GetTop(textBox);
                            text.FontSize = textBox.FontSize;
                        }
                    }
                }
            }
            else if (element is TextBox)
            {
                int index = int.Parse((element as TextBox).Name.Substring(4));
                foreach (var text in plan.ElementTextBlocks)
                {
                    if (text.idElementCanvas == index)
                    {
                        text.Left = Canvas.GetLeft(element as TextBox);
                        text.Top = Canvas.GetTop(element as TextBox);
                        text.FontSize = (element as TextBox).FontSize;
                        text.Text = (element as TextBox).Text;
                    }
                }
            }
        }

        public void UpdatePlan()
        {
            if (UpdateElement == null)
                return;

            int index = int.Parse(UpdateElement.Substring(4));
            switch (typeElement)
            {
                case "rect":
                    foreach (var rect in Plan.ElementRectangles)
                    {
                        if (_overlayElementRectangle != null && rect.idElementCanvas == index)
                        {
                            rect.Left += _overlayElementRectangle.LeftOffset;
                            rect.Top += _overlayElementRectangle.TopOffset + PlanCanvasView.dTop;
                        }
                    }
                    break;

                case "devs":
                    foreach (var device in Plan.ElementDevices)
                    {
                        if (_overlayElementRectangle != null && device.idElementCanvas == index)
                        {
                            device.Left += _overlayElementRectangle.LeftOffset;
                            device.Top += _overlayElementRectangle.TopOffset + PlanCanvasView.dTop;
                        }
                    }
                    break;

                case "text":
                    foreach (var text in Plan.ElementTextBlocks)
                    {
                        if (_originalElementTextBox != null && text.idElementCanvas == index)
                        {
                            text.Left = Canvas.GetLeft(_originalElementTextBox);
                            text.Top = Canvas.GetTop(_originalElementTextBox);
                        }
                    }
                    break;

                case "zona":
                    foreach (var zona in Plan.ElementZones)
                    {
                        if (zona.idElementCanvas == index && _overlayElementPolygon != null)
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
                    break;
            }
            string type = typeElement;
            UIElement element = _originalElement;
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

        void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0 && e.ExtentWidthChange == 0)
                return;

            Point? targetBefore = null;
            Point? targetNow = null;

            if (!lastMousePositionOnTarget.HasValue)
            {
                if (lastCenterPositionOnTarget.HasValue)
                {
                    var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);

                    Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, MainCanvas);

                    targetBefore = lastCenterPositionOnTarget;
                    targetNow = centerOfTargetNow;
                }
            }
            else
            {
                targetBefore = lastMousePositionOnTarget;
                targetNow = Mouse.GetPosition(MainCanvas);
                lastMousePositionOnTarget = null;
            }

            if (targetBefore.HasValue)
            {
                double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                double multiplicatorX = e.ExtentWidth / MainCanvas.ActualWidth;

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

        void FullSize()
        {
            if (!CanvasLoaded)
                return;

            dResizeWindowX = scrollViewer.ActualWidth / MainCanvas.Width;
            dResizeWindowY = scrollViewer.ActualHeight / MainCanvas.Height;
            if (dResizeWindowX > dResizeWindowY)
            {
                scaleTransform.ScaleX = dResizeWindowY;
                scaleTransform.ScaleY = dResizeWindowY;
            }
            else
            {
                scaleTransform.ScaleX = dResizeWindowX;
                scaleTransform.ScaleY = dResizeWindowX;
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
            if (MainCanvasViewModel.Canvas == null) MainCanvasViewModel.Canvas = MainCanvas;
            PolygonResizer.SetCanvas(MainCanvas);
            FullSize();
        }

        void MenuClicked(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            DialogBox.DialogBox.Show("test");
            e.Handled = true;
        }

        void ContextMenuCanvas_Opened(object sender, RoutedEventArgs e)
        {
        }

        void ContextMenuCanvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}