//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Shapes;
//using DeviceControls;
//using FiresecAPI.Models;
//using FiresecClient;
//using Infrastructure;
//using Infrastructure.Common;
//using Infrastructure.Common.Windows;
//using PlansModule.Events;
//using PlansModule.ViewModels;
//using Infrustructure.Plans.Elements;

//namespace PlansModule.Designer
//{
//    public class _DesignerItem : ContentControl
//    {
//        #region Designer Properties
//        public bool IsSelected
//        {
//            get { return (bool)GetValue(IsSelectedProperty); }
//            set
//            {
//                SetValue(IsSelectedProperty, value);
//                if (value)
//                {
//                    if (Element is ElementDevice)
//                    {
//                        ElementDevice elementDevice = Element as ElementDevice;
//                        ServiceFactory.Events.GetEvent<ElementDeviceSelectedEvent>().Publish(elementDevice.DeviceUID);
//                    }
//                    ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Publish(Element.UID);
//                }
//            }
//        }

//        public static readonly DependencyProperty IsSelectedProperty =
//          DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

//        public bool IsSelectable
//        {
//            get { return (bool)GetValue(IsSelectableProperty); }
//            set { SetValue(IsSelectableProperty, value); }
//        }

//        public static readonly DependencyProperty IsSelectableProperty =
//          DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

//        public bool IsPolygon
//        {
//            get { return (bool)GetValue(IsPolygonProperty); }
//            set { SetValue(IsPolygonProperty, value); }
//        }

//        public static readonly DependencyProperty IsPolygonProperty =
//          DependencyProperty.Register("IsPolygon", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

//        public bool IsDevice
//        {
//            get { return (bool)GetValue(IsDeviceProperty); }
//            set { SetValue(IsDeviceProperty, value); }
//        }

//        public static readonly DependencyProperty IsDeviceProperty =
//          DependencyProperty.Register("IsDevice", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

//        public bool IsPolyline
//        {
//            get { return (bool)GetValue(IsPolylineProperty); }
//            set { SetValue(IsPolylineProperty, value); }
//        }

//        public static readonly DependencyProperty IsPolylineProperty =
//          DependencyProperty.Register("IsPolyline", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

//        public static readonly DependencyProperty MoveThumbTemplateProperty =
//            DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

//        public static ControlTemplate GetMoveThumbTemplate(UIElement element)
//        {
//            return (ControlTemplate)element.GetValue(MoveThumbTemplateProperty);
//        }

//        public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
//        {
//            element.SetValue(MoveThumbTemplateProperty, value);
//        }

//        static _DesignerItem()
//        {
//            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
//        }
//        #endregion Properties

//        public DesignerCanvas DesignerCanvas
//        {
//            get { return VisualTreeHelper.GetParent(this) as DesignerCanvas; }
//        }

//        public _DesignerItem()
//        {
//            AddPointCommand = new RelayCommand(OnAddPoint);
//            DeleteCommand = new RelayCommand(OnDelete);
//            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
//            this.Loaded += new RoutedEventHandler(this.DesignerItem_Loaded);
//            this.MouseDoubleClick += new MouseButtonEventHandler((object o, MouseButtonEventArgs x) => { OnShowProperties(); });
//            IsVisibleLayout = true;
//            IsSelectableLayout = true;
//        }

//        public IResizeChromeBase ResizeChromeBase { get; set; }
//        public bool IsPointAdding { get; set; }
//        public ElementBase Element { get; set; }

//        bool _isVisibleLayout;
//        public bool IsVisibleLayout
//        {
//            get { return _isVisibleLayout; }
//            set
//            {
//                _isVisibleLayout = value;
//                if (value)
//                {
//                    Visibility = System.Windows.Visibility.Visible;
//                }
//                else
//                {
//                    Visibility = System.Windows.Visibility.Collapsed;
//                    IsSelected = false;
//                }
//            }
//        }

//        bool _isSelectableLayout;
//        public bool IsSelectableLayout
//        {
//            get { return _isSelectableLayout; }
//            set
//            {
//                _isSelectableLayout = value;
//                IsSelectable = value;
//                if (value == false)
//                {
//                    IsSelected = false;
//                }
//            }
//        }

//        public RelayCommand AddPointCommand { get; private set; }
//        void OnAddPoint()
//        {
//            IsPointAdding = true;
//            DesignerCanvas.IsPointAdding = true;
//        }

//        public RelayCommand DeleteCommand { get; private set; }
//        void OnDelete()
//        {
//            DesignerCanvas.RemoveAllSelected();
//        }

//        public RelayCommand ShowPropertiesCommand { get; private set; }
//        void OnShowProperties()
//        {
//            bool result = false;
//            DesignerCanvas.BeginChange();

//            if (Element is ElementRectangle)
//            {
//                var rectanglePropertiesViewModel = new RectanglePropertiesViewModel(Element as ElementRectangle);
//                result = DialogService.ShowModalWindow(rectanglePropertiesViewModel);
//            }
//            if (Element is ElementEllipse)
//            {
//                var ellipsePropertiesViewModel = new EllipsePropertiesViewModel(Element as ElementEllipse);
//                result = DialogService.ShowModalWindow(ellipsePropertiesViewModel);
//            }
//            if (Element is ElementTextBlock)
//            {
//                var textBlockPropertiesViewModel = new TextBlockPropertiesViewModel(Element as ElementTextBlock);
//                result = DialogService.ShowModalWindow(textBlockPropertiesViewModel);
//            }
//            if (Element is ElementPolygon)
//            {
//                ElementPolygon elementPolygon = Element as ElementPolygon;
//                elementPolygon.Points = new PointCollection((Content as Polygon).Points);

//                var polygonPropertiesViewModel = new PolygonPropertiesViewModel(Element as ElementPolygon);
//                result = DialogService.ShowModalWindow(polygonPropertiesViewModel);
//            }
//            if (Element is ElementPolyline)
//            {
//                ElementPolyline elementPolyline = Element as ElementPolyline;
//                elementPolyline.Points = new PointCollection((Content as Polyline).Points);

//                var polylinePropertiesViewModel = new PolylinePropertiesViewModel(Element as ElementPolyline);
//                result = DialogService.ShowModalWindow(polylinePropertiesViewModel);
//            }
//            if (Element is ElementPolygonZone)
//            {
//                ElementPolygonZone elementPolygonZone = Element as ElementPolygonZone;
//                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementPolygonZone);
//                result = DialogService.ShowModalWindow(zonePropertiesViewModel);
//            }
//            if (Element is ElementRectangleZone)
//            {
//                ElementRectangleZone elementRectangleZone = Element as ElementRectangleZone;
//                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementRectangleZone);
//                result = DialogService.ShowModalWindow(zonePropertiesViewModel);
//            }
//            if (Element is ElementDevice)
//            {
//                var devicePropertiesViewModel = new DevicePropertiesViewModel(Element as ElementDevice);
//                result = DialogService.ShowModalWindow(devicePropertiesViewModel);
//                if (result)
//                {
//                    var device = (Element as ElementDevice).Device;
//                    var devicePicture = DeviceControl.GetDefaultPicture(device.Driver.UID);
//                    devicePicture.IsHitTestVisible = false;
//                    Content = devicePicture;
//                }
//            }
//            if (Element is ElementSubPlan)
//            {
//                var subPlanPropertiesViewModel = new SubPlanPropertiesViewModel(Element as ElementSubPlan);
//                result = DialogService.ShowModalWindow(subPlanPropertiesViewModel);
//            }

//            if (result)
//            {
//                Redraw();
//                ServiceFactory.SaveService.PlansChanged = true;
//                DesignerCanvas.EndChange();
//            }
//        }

//        public void Redraw()
//        {
//            if (Element is IElementZone)
//            {
//                IElementZone elementZone = Element as IElementZone;
//                elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);
//            }

//            FrameworkElement frameworkElement = null;
//            if (Element is ElementDevice)
//            {
//                var elementDevice = Element as ElementDevice;
//                if (elementDevice.Device == null)
//                    elementDevice.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
//                if (elementDevice.Device != null)
//                {
//                    frameworkElement = DeviceControl.GetDefaultPicture(elementDevice.Device.Driver.UID);
//                }
//                else
//                {
//                    frameworkElement = Element.Draw();
//                }
//            }
//            else
//            {
//                frameworkElement = Element.Draw();
//            }

//            if (frameworkElement != null)
//            {
//                frameworkElement.IsHitTestVisible = false;
//                Content = frameworkElement;
//            }

//            Canvas.SetLeft(this, Element.Left);
//            Canvas.SetTop(this, Element.Top);
//            ItemWidth = Element.Width;
//            ItemHeight = Element.Height;

//            UpdateZoomDevice();
//            UpdatePolygonAdorner();
//        }

//        public void UpdatePolygonAdorner()
//        {
//            if (ResizeChromeBase != null)
//            {
//                ResizeChromeBase.Initialize();
//            }
//        }

//        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
//        {
//            base.OnPreviewMouseDown(e);

//            if (DesignerCanvas != null)
//            {
//                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
//                {
//                    this.IsSelected = !this.IsSelected;
//                }
//                else
//                {
//                    if (!this.IsSelected)
//                    {
//                        DesignerCanvas.DeselectAll();
//                        this.IsSelected = true;
//                    }
//                }
//            }

//            e.Handled = false;
//        }

//        private void DesignerItem_Loaded(object sender, RoutedEventArgs e)
//        {
//            if (this.Template != null)
//            {
//                ContentPresenter contentPresenter = this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
//                MoveThumb moveThumb = this.Template.FindName("PART_MoveThumbRectangle", this) as MoveThumb;

//                if (contentPresenter != null && moveThumb != null)
//                {
//                    UIElement contentVisual = null;
//                    if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
//                    {
//                        contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
//                    }

//                    if (contentVisual != null)
//                    {
//                        ControlTemplate controlTemplate = DesignerItem.GetMoveThumbTemplate(contentVisual) as ControlTemplate;

//                        if (controlTemplate != null)
//                        {
//                            moveThumb.Template = controlTemplate;
//                        }
//                    }
//                }
//            }

//            UpdateZoomDevice();
//        }

//        public void Remove()
//        {
//            if (Element is ElementDevice)
//            {
//                var elementDevice = Element as ElementDevice;
//                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
//                device.PlanElementUIDs.Remove(elementDevice.UID);
//                ServiceFactory.Events.GetEvent<DeviceRemovedEvent>().Publish(elementDevice.Device.UID);
//            }
//        }

//        public void SavePropertiesToElementBase()
//        {
//            Element.Left = Canvas.GetLeft(this);
//            Element.Top = Canvas.GetTop(this);
//            Element.Width = this.ItemWidth;
//            Element.Height = this.ItemHeight;

//            if (Element is ElementBasePolygon)
//            {
//                ElementBasePolygon elementPolygon = Element as ElementBasePolygon;
//                elementPolygon.Points = new PointCollection();
//                if (this.Content != null)
//                    foreach (var point in (this.Content as Polygon).Points)
//                    {
//                        elementPolygon.Points.Add(new Point(point.X, point.Y));
//                    }
//            }
//            if (Element is ElementPolyline)
//            {
//                ElementPolyline elementPolyline = Element as ElementPolyline;
//                elementPolyline.Points = new PointCollection();
//                if (this.Content != null)
//                    foreach (var point in (this.Content as Polyline).Points)
//                    {
//                        elementPolyline.Points.Add(new Point(point.X, point.Y));
//                    }
//            }
//            if (Element is ElementDevice)
//            {
//                Element.Left = Canvas.GetLeft(this) + this.Width / 2;
//                Element.Top = Canvas.GetTop(this) + this.Height / 2;
//                Element.Width = 0;
//                Element.Height = 0;
//            }
//        }

//        public void UpdateZoom()
//        {
//            if (ResizeChromeBase != null)
//            {
//                ResizeChromeBase.UpdateZoom();
//            }
//        }

//        public void UpdateZoomDevice()
//        {
//            if (IsDevice)
//            {
//                this.Width = DesignerCanvas.PlanDesignerViewModel.DeviceZoom;
//                this.Height = DesignerCanvas.PlanDesignerViewModel.DeviceZoom;
//                Canvas.SetLeft(this, Element.Left - this.Width / 2);
//                Canvas.SetTop(this, Element.Top - this.Height / 2);
//            }
//        }

//        public double ItemWidth
//        {
//            get
//            {
//                if (IsPolygon || IsPolyline)
//                    return Width - 20;
//                else
//                    return Width;
//            }
//            set
//            {
//                if (IsPolygon || IsPolyline)
//                    Width = value + 20;
//                else
//                    Width = value;
//            }
//        }

//        public double ItemHeight
//        {
//            get
//            {
//                if (IsPolygon || IsPolyline)
//                    return Height - 20;
//                else
//                    return Height;
//            }
//            set
//            {
//                if (IsPolygon || IsPolyline)
//                    Height = value + 20;
//                else
//                    Height = value;
//            }
//        }
//    }
//}