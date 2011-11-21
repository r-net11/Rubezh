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
using System.Collections.Generic;

namespace PlansModule.Designer
{
    public class DesignerItem : ContentControl
    {
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

        public DesignerCanvas DesignerCanvas
        {
            get { return VisualTreeHelper.GetParent(this) as DesignerCanvas; }
        }

        public PolygonResizeChrome PolygonResizeChrome { get; set; }

        public bool IsPointAdding { get; set; }

        public ElementBase ElementBase { get; set; }

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
                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == (ElementBase as ElementDevice).DeviceUID);
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
                PlansModule.HasChanges = true;
                DesignerCanvas.EndChange();
            }
        }

        public void Redraw()
        {
            var framaworkElement = ElementBase.Draw();
            if (framaworkElement != null)
            {
                framaworkElement.IsHitTestVisible = false;
                Content = framaworkElement;
            }

            Canvas.SetLeft(this, ElementBase.Left);
            Canvas.SetTop(this, ElementBase.Top);
            Width = ElementBase.Width;
            Height = ElementBase.Height;
            UpdatePolygonAdorner();
        }

        public void UpdatePolygonAdorner()
        {
            if (PolygonResizeChrome != null)
            {
                PolygonResizeChrome.Initialize();
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
            }
        }

        public void SavePropertiesToElementBase()
        {
            ElementBase.Left = Canvas.GetLeft(this);
            ElementBase.Top = Canvas.GetTop(this);
            ElementBase.Width = this.Width;
            ElementBase.Height = this.Height;
            if (ElementBase is ElementBasePolygon)
            {
                ElementBasePolygon elementPolygon = ElementBase as ElementBasePolygon;
                elementPolygon.PolygonPoints = new System.Windows.Media.PointCollection((this.Content as Polygon).Points);
            }
        }
    }
}
