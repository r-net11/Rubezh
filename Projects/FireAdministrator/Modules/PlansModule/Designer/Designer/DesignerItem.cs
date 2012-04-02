using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using PlansModule.ViewModels;

namespace PlansModule.Designer
{
    public class DesignerItem : ContentControl
    {
        #region Designer Properties
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                if (value)
                {
                    if (ElementBase is ElementDevice)
                    {
                        ElementDevice elementDevice = ElementBase as ElementDevice;
                        ServiceFactory.Events.GetEvent<ElementDeviceSelectedEvent>().Publish(elementDevice.DeviceUID);
                    }
                    ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Publish(ElementBase.UID);
                }
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

        public bool IsSelectable
        {
            get { return (bool)GetValue(IsSelectableProperty); }
            set { SetValue(IsSelectableProperty, value); }
        }

        public static readonly DependencyProperty IsSelectableProperty =
          DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

        public bool IsPolygon
        {
            get { return (bool)GetValue(IsPolygonProperty); }
            set { SetValue(IsPolygonProperty, value); }
        }

        public static readonly DependencyProperty IsPolygonProperty =
          DependencyProperty.Register("IsPolygon", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

        public bool IsDevice
        {
            get { return (bool)GetValue(IsDeviceProperty); }
            set { SetValue(IsDeviceProperty, value); }
        }

        public static readonly DependencyProperty IsDeviceProperty =
          DependencyProperty.Register("IsDevice", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

        public bool IsPolyline
        {
            get { return (bool)GetValue(IsPolylineProperty); }
            set { SetValue(IsPolylineProperty, value); }
        }

        public static readonly DependencyProperty IsPolylineProperty =
          DependencyProperty.Register("IsPolyline", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MoveThumbTemplateProperty =
            DependencyProperty.RegisterAttached("MoveThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetMoveThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(MoveThumbTemplateProperty);
        }

        public static void SetMoveThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(MoveThumbTemplateProperty, value);
        }

        static DesignerItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }
        #endregion Properties

        public DesignerCanvas DesignerCanvas
        {
            get { return VisualTreeHelper.GetParent(this) as DesignerCanvas; }
        }

        public DesignerItem()
        {
            AddPointCommand = new RelayCommand(OnAddPoint);
            DeleteCommand = new RelayCommand(OnDelete);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            this.Loaded += new RoutedEventHandler(this.DesignerItem_Loaded);
            this.MouseDoubleClick += new MouseButtonEventHandler((object o, MouseButtonEventArgs x) => {OnShowProperties();});
            IsVisibleLayout = true;
            IsSelectableLayout = true;
        }

        public IResizeChromeBase ResizeChromeBase { get; set; }
        public bool IsPointAdding { get; set; }
        public ElementBase ElementBase { get; set; }

        bool _isVisibleLayout;
        public bool IsVisibleLayout
        {
            get { return _isVisibleLayout; }
            set
            {
                _isVisibleLayout = value;
                if (value)
                {
                    Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Visibility = System.Windows.Visibility.Collapsed;
                    IsSelected = false;
                }
            }
        }

        bool _isSelectableLayout;
        public bool IsSelectableLayout
        {
            get { return _isSelectableLayout; }
            set
            {
                _isSelectableLayout = value;
                IsSelectable = value;
                if (value == false)
                {
                    IsSelected = false;
                }
            }
        }

        public RelayCommand AddPointCommand { get; private set; }
        void OnAddPoint()
        {
            IsPointAdding = true;
            DesignerCanvas.IsPointAdding = true;
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            DesignerCanvas.RemoveAllSelected();
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            bool result = false;
            DesignerCanvas.BeginChange();

            if (ElementBase is ElementRectangle)
            {
                var rectanglePropertiesViewModel = new RectanglePropertiesViewModel(ElementBase as ElementRectangle);
                result = ServiceFactory.UserDialogs.ShowModalWindow(rectanglePropertiesViewModel);
            }
            if (ElementBase is ElementEllipse)
            {
                var ellipsePropertiesViewModel = new EllipsePropertiesViewModel(ElementBase as ElementEllipse);
                result = ServiceFactory.UserDialogs.ShowModalWindow(ellipsePropertiesViewModel);
            }
            if (ElementBase is ElementTextBlock)
            {
                var textBlockPropertiesViewModel = new TextBlockPropertiesViewModel(ElementBase as ElementTextBlock);
                result = ServiceFactory.UserDialogs.ShowModalWindow(textBlockPropertiesViewModel);
            }
            if (ElementBase is ElementPolygon)
            {
                ElementPolygon elementPolygon = ElementBase as ElementPolygon;
                elementPolygon.PolygonPoints = new PointCollection((Content as Polygon).Points);

                var polygonPropertiesViewModel = new PolygonPropertiesViewModel(ElementBase as ElementPolygon);
                result = ServiceFactory.UserDialogs.ShowModalWindow(polygonPropertiesViewModel);
            }
            if (ElementBase is ElementPolyline)
            {
                ElementPolyline elementPolyline = ElementBase as ElementPolyline;
                elementPolyline.PolygonPoints = new PointCollection((Content as Polyline).Points);

                var polylinePropertiesViewModel = new PolylinePropertiesViewModel(ElementBase as ElementPolyline);
                result = ServiceFactory.UserDialogs.ShowModalWindow(polylinePropertiesViewModel);
            }
            if (ElementBase is ElementPolygonZone)
            {
                ElementPolygonZone elementPolygonZone = ElementBase as ElementPolygonZone;
                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementPolygonZone);
                result = ServiceFactory.UserDialogs.ShowModalWindow(zonePropertiesViewModel);
            }
            if (ElementBase is ElementRectangleZone)
            {
                ElementRectangleZone elementRectangleZone = ElementBase as ElementRectangleZone;
                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementRectangleZone);
                result = ServiceFactory.UserDialogs.ShowModalWindow(zonePropertiesViewModel);
            }
            if (ElementBase is ElementDevice)
            {
                var devicePropertiesViewModel = new DevicePropertiesViewModel(ElementBase as ElementDevice);
                result = ServiceFactory.UserDialogs.ShowModalWindow(devicePropertiesViewModel);
                if (result)
                {
                    var device = (ElementBase as ElementDevice).Device;
                    var devicePicture = DeviceControl.GetDefaultPicture(device.Driver.UID);
                    devicePicture.IsHitTestVisible = false;
                    Content = devicePicture;
                }
            }
            if (ElementBase is ElementSubPlan)
            {
                var subPlanPropertiesViewModel = new SubPlanPropertiesViewModel(ElementBase as ElementSubPlan);
                result = ServiceFactory.UserDialogs.ShowModalWindow(subPlanPropertiesViewModel);
            }

            if (result)
            {
                Redraw();
                ServiceFactory.SaveService.PlansChanged = true;
                DesignerCanvas.EndChange();
            }
        }

        public void Redraw()
        {
            if (ElementBase is IElementZone)
            {
                IElementZone elementZone = ElementBase as IElementZone;
                elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo);
            }

            FrameworkElement frameworkElement = null;
            if (ElementBase is ElementDevice)
            {
                var elementDevice = ElementBase as ElementDevice;
                if (elementDevice.Device == null)
                    elementDevice.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                frameworkElement = DeviceControl.GetDefaultPicture(elementDevice.Device.Driver.UID);
            }
            else
            {
                frameworkElement = ElementBase.Draw();
            }

            if (frameworkElement != null)
            {
                frameworkElement.IsHitTestVisible = false;
                Content = frameworkElement;
            }

            Canvas.SetLeft(this, ElementBase.Left);
            Canvas.SetTop(this, ElementBase.Top);
            ItemWidth = ElementBase.Width;
            ItemHeight = ElementBase.Height;

            UpdateZoomDevice();
            UpdatePolygonAdorner();
        }

        public void UpdatePolygonAdorner()
        {
            if (ResizeChromeBase != null)
            {
                ResizeChromeBase.Initialize();
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (DesignerCanvas != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    this.IsSelected = !this.IsSelected;
                }
                else
                {
                    if (!this.IsSelected)
                    {
                        DesignerCanvas.DeselectAll();
                        this.IsSelected = true;
                    }
                }
            }

            e.Handled = false;
        }

        private void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Template != null)
            {
                ContentPresenter contentPresenter = this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                MoveThumb moveThumb = this.Template.FindName("PART_MoveThumbRectangle", this) as MoveThumb;

                if (contentPresenter != null && moveThumb != null)
                {
                    UIElement contentVisual = null;
                    if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                    {
                        contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                    }

                    if (contentVisual != null)
                    {
                        ControlTemplate controlTemplate = DesignerItem.GetMoveThumbTemplate(contentVisual) as ControlTemplate;

                        if (controlTemplate != null)
                        {
                            moveThumb.Template = controlTemplate;
                        }
                    }
                }
            }

            UpdateZoomDevice();
        }

        public void Remove()
        {
            if (ElementBase is ElementDevice)
            {
                var elementDevice = ElementBase as ElementDevice;
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                device.PlanElementUIDs.Remove(elementDevice.UID);
                ServiceFactory.Events.GetEvent<DeviceRemovedEvent>().Publish(elementDevice.Device.UID);
            }
        }

        public void SavePropertiesToElementBase()
        {
            ElementBase.Left = Canvas.GetLeft(this);
            ElementBase.Top = Canvas.GetTop(this);
            ElementBase.Width = this.ItemWidth;
            ElementBase.Height = this.ItemHeight;

            if (ElementBase is ElementBasePolygon)
            {
                ElementBasePolygon elementPolygon = ElementBase as ElementBasePolygon;
                elementPolygon.PolygonPoints = new PointCollection();
                foreach (var point in (this.Content as Polygon).Points)
                {
                    elementPolygon.PolygonPoints.Add(new Point(point.X, point.Y));
                }
            }
            if (ElementBase is ElementPolyline)
            {
                ElementPolyline elementPolyline = ElementBase as ElementPolyline;
                elementPolyline.PolygonPoints = new PointCollection();
                foreach (var point in (this.Content as Polyline).Points)
                {
                    elementPolyline.PolygonPoints.Add(new Point(point.X, point.Y));
                }
            }
            if (ElementBase is ElementDevice)
            {
                ElementBase.Left = Canvas.GetLeft(this) + this.Width / 2;
                ElementBase.Top = Canvas.GetTop(this) + this.Height / 2;
                ElementBase.Width = 0;
                ElementBase.Height = 0;
            }
        }

        public void UpdateZoom()
        {
            if (ResizeChromeBase != null)
            {
                ResizeChromeBase.UpdateZoom();
            }
        }

        public void UpdateZoomDevice()
        {
            if (IsDevice)
            {
                double zoom = DesignerCanvas.PlanDesignerViewModel.DeviceZoom / DesignerCanvas.PlanDesignerViewModel.Zoom;
                this.Width = zoom;
                this.Height = zoom;
                Canvas.SetLeft(this, ElementBase.Left - this.Width / 2);
                Canvas.SetTop(this, ElementBase.Top - this.Height / 2);
            }
        }

        public double ItemWidth
        {
            get
            {
                if (IsPolygon || IsPolyline)
                    return Width - 20;
                else
                    return Width;
            }
            set
            {
                if (IsPolygon || IsPolyline)
                    Width = value + 20;
                else
                    Width = value;
            }
        }

        public double ItemHeight
        {
            get
            {
                if (IsPolygon || IsPolyline)
                    return Height - 20;
                else
                    return Height;
            }
            set
            {
                if (IsPolygon || IsPolyline)
                    Height = value + 20;
                else
                    Height = value;
            }
        }
    }
}