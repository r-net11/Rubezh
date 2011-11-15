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
using PlansModule.ViewModels;
using PlansModule.Events;

namespace PlansModule.Designer
{
    public class DesignerItem : ContentControl
    {
        public ElementBase ElementBase { get; set; }

        #region Properties
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected", typeof(bool),
                                      typeof(DesignerItem),
                                      new FrameworkPropertyMetadata(false));

        public bool IsPolygon
        {
            get { return (bool)GetValue(IsPolygonProperty); }
            set { SetValue(IsPolygonProperty, value); }
        }

        public static readonly DependencyProperty IsPolygonProperty =
          DependencyProperty.Register("IsPolygon", typeof(bool),
                                      typeof(DesignerItem),
                                      new FrameworkPropertyMetadata(false));

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

        public DesignerItem()
        {
            AddPointCommand = new RelayCommand(OnAddPoint);
            DeleteCommand = new RelayCommand(OnDelete);
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            this.Loaded += new RoutedEventHandler(this.DesignerItem_Loaded);
        }

        public RelayCommand AddPointCommand { get; private set; }
        void OnAddPoint()
        {
            DesignerCanvas designerCanvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            designerCanvas.IsPointAdding = true;
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            DesignerCanvas designerCanvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            designerCanvas.Children.Remove(this);
            if (ElementBase is ElementDevice)
            {
                ElementDevice elementDevice = ElementBase as ElementDevice;
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                device.PlanUIDs.Remove(elementDevice.UID);
                ServiceFactory.Events.GetEvent<DeviceRemovedEvent>().Publish(elementDevice.DeviceUID);
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            if (ElementBase is ElementRectangle)
            {
                var rectanglePropertiesViewModel = new RectanglePropertiesViewModel(ElementBase as ElementRectangle);
                if (ServiceFactory.UserDialogs.ShowModalWindow(rectanglePropertiesViewModel))
                {
                    Redraw();
                    PlansModule.HasChanges = true;
                }
            }
            if (ElementBase is ElementEllipse)
            {
                var ellipsePropertiesViewModel = new EllipsePropertiesViewModel(ElementBase as ElementEllipse);
                if (ServiceFactory.UserDialogs.ShowModalWindow(ellipsePropertiesViewModel))
                {
                    Redraw();
                    PlansModule.HasChanges = true;
                }
            }
            if (ElementBase is ElementTextBlock)
            {
                var textBlockPropertiesViewModel = new TextBlockPropertiesViewModel(ElementBase as ElementTextBlock);
                if (ServiceFactory.UserDialogs.ShowModalWindow(textBlockPropertiesViewModel))
                {
                    Redraw();
                    PlansModule.HasChanges = true;
                }
            }
            if (ElementBase is ElementPolygon)
            {
                ElementPolygon elementPolygon = ElementBase as ElementPolygon;
                elementPolygon.PolygonPoints = new PointCollection((Content as Polygon).Points);

                var polygonPropertiesViewModel = new PolygonPropertiesViewModel(ElementBase as ElementPolygon);
                if (ServiceFactory.UserDialogs.ShowModalWindow(polygonPropertiesViewModel))
                {
                    DesignerCanvas designerCanvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
                    designerCanvas.Children.Remove(this);
                    designerCanvas.Create(ElementBase);
                    //Redraw();
                    PlansModule.HasChanges = true;
                }
            }
            if (ElementBase is ElementPolygonZone)
            {
                ElementPolygonZone elementPolygonZone = ElementBase as ElementPolygonZone;
                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementPolygonZone.ZoneNo);
                if (ServiceFactory.UserDialogs.ShowModalWindow(zonePropertiesViewModel))
                {
                    elementPolygonZone.ZoneNo = zonePropertiesViewModel.SelectedZone.No;
                    PlansModule.HasChanges = true;
                }
            }
            if (ElementBase is ElementRectangleZone)
            {
                ElementRectangleZone elementRectangleZone = ElementBase as ElementRectangleZone;
                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementRectangleZone.ZoneNo);
                if (ServiceFactory.UserDialogs.ShowModalWindow(zonePropertiesViewModel))
                {
                    elementRectangleZone.ZoneNo = zonePropertiesViewModel.SelectedZone.No;
                    PlansModule.HasChanges = true;
                }
            }
            if (ElementBase is ElementDevice)
            {
                var devicePropertiesViewModel = new DevicePropertiesViewModel(ElementBase as ElementDevice);
                if (ServiceFactory.UserDialogs.ShowModalWindow(devicePropertiesViewModel))
                {
                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == (ElementBase as ElementDevice).DeviceUID);
                    var devicePicture = DeviceControl.GetDefaultPicture(device.Driver.UID);
                    devicePicture.IsHitTestVisible = false;
                    Content = devicePicture;

                    PlansModule.HasChanges = true;
                }
            }
            if (ElementBase is ElementSubPlan)
            {
                var subPlanPropertiesViewModel = new SubPlanPropertiesViewModel(ElementBase as ElementSubPlan);
                if (ServiceFactory.UserDialogs.ShowModalWindow(subPlanPropertiesViewModel))
                {
                    PlansModule.HasChanges = true;
                }
            }
        }

        public void Redraw()
        {
            var framaworkElement = ElementBase.Draw();
            framaworkElement.IsHitTestVisible = false;
            Content = framaworkElement;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designerCanvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            if (designerCanvas != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    this.IsSelected = !this.IsSelected;
                }
                else
                {
                    if (!this.IsSelected)
                    {
                        designerCanvas.DeselectAll();
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
                MoveThumb moveThumb = this.Template.FindName("PART_MoveThumb", this) as MoveThumb;

                if (contentPresenter != null && moveThumb != null)
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;

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
        }

        public void Add()
        {
            if (ElementBase is ElementDevice)
            {
                var elementDevice = ElementBase as ElementDevice;
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                device.PlanUIDs.Add(elementDevice.UID);
                ServiceFactory.Events.GetEvent<DeviceAddedEvent>().Publish(device.UID);
            }
        }

        public void Remove()
        {
            if (ElementBase is ElementDevice)
            {
                var elementDevice = ElementBase as ElementDevice;
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                device.PlanUIDs.Add(elementDevice.UID);
                ServiceFactory.Events.GetEvent<DeviceRemovedEvent>().Publish(device.UID);
            }
        }
    }
}
