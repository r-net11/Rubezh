using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using PlansModule.ViewModels;

namespace PlansModule.Designer
{
    public class DesignerCanvas : Canvas
    {
        public Plan Plan { get; set; }
        public PlanDesignerViewModel PlanDesignerViewModel { get; set; }
        private Point? dragStartPoint = null;
        public bool IsPointAdding = false;
        List<ElementBase> initialElements;

        public DesignerCanvas()
        {
            AllowDrop = true;
            Background = new SolidColorBrush(Colors.DarkGray);
            Width = 100;
            Height = 100;

            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
            PreviewMouseDown += new MouseButtonEventHandler(On_PreviewMouseDown);
            DataContext = this;
        }

        public IEnumerable<DesignerItem> Items
        {
            get
            {
                return from item in this.Children.OfType<DesignerItem>()
                       select item;
            }
        }

        public IEnumerable<DesignerItem> SelectedItems
        {
            get
            {
                return from item in this.Children.OfType<DesignerItem>()
                       where item.IsSelected == true
                       select item;
            }
        }

        public List<ElementBase> Elements
        {
            get
            {
                return (from item in this.Children.OfType<DesignerItem>()
                        select item.ElementBase).ToList();
            }
        }

        public List<ElementBase> SelectedElements
        {
            get
            {
                return (from item in this.Children.OfType<DesignerItem>()
                       where item.IsSelected == true
                       select item.ElementBase).ToList();
            }
        }

        public void DeselectAll()
        {
            foreach (DesignerItem item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        public void RemoveAllSelected()
        {
            ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Publish(new List<ElementBase>(SelectedElements));

            for (int i = Items.Count(); i > 0; i--)
            {
                var designerItem = Children[i - 1] as DesignerItem;
                if (designerItem.IsSelected)
                {
                    (Children[i - 1] as DesignerItem).Remove();
                    Children.RemoveAt(i - 1);
                }
            }

            ServiceFactory.SaveService.PlansChanged = true;
        }

        public void SelectAll()
        {
            foreach(var designerItem in Items)
                designerItem.IsSelected = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                this.dragStartPoint = new Point?(e.GetPosition(this));
                this.DeselectAll();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    var rubberbandAdorner = new RubberbandAdorner(this, this.dragStartPoint);
                    if (rubberbandAdorner != null)
                    {
                        adornerLayer.Add(rubberbandAdorner);
                    }
                }

                e.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var elementBase = e.Data.GetData("DESIGNER_ITEM") as ElementBase;

            Point position = e.GetPosition(this);
            elementBase.Left = Math.Max(0, position.X - elementBase.Height / 2);
            elementBase.Top = Math.Max(0, position.Y - elementBase.Height / 2);

            var designerItem = AddElement(elementBase);
            if (designerItem != null)
            {
                this.DeselectAll();
                designerItem.IsSelected = true;
                PlanDesignerViewModel.MoveToFrontCommand.Execute();

                ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(new List<ElementBase>() { elementBase });
            }

            e.Handled = true;
        }

        public DesignerItem AddElement(ElementBase elementBase)
        {
            if (elementBase is ElementRectangle)
            {
                Plan.ElementRectangles.Add(elementBase as ElementRectangle);
            }
            if (elementBase is ElementEllipse)
            {
                Plan.ElementEllipses.Add(elementBase as ElementEllipse);
            }
            if (elementBase is ElementPolygon)
            {
                Plan.ElementPolygons.Add(elementBase as ElementPolygon);
            }
            if (elementBase is ElementPolyline)
            {
                Plan.ElementPolylines.Add(elementBase as ElementPolyline);
            }
            if (elementBase is ElementTextBlock)
            {
                Plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
            }
            if (elementBase is ElementRectangleZone)
            {
                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementBase as IElementZone);
                if (ServiceFactory.UserDialogs.ShowModalWindow(zonePropertiesViewModel) == false)
                    return null;
                Plan.ElementRectangleZones.Add(elementBase as ElementRectangleZone);
            }
            if (elementBase is ElementPolygonZone)
            {
                var zonePropertiesViewModel = new ZonePropertiesViewModel(elementBase as IElementZone);
                if (ServiceFactory.UserDialogs.ShowModalWindow(zonePropertiesViewModel) == false)
                    return null;
                Plan.ElementPolygonZones.Add(elementBase as ElementPolygonZone);
            }
            if (elementBase is ElementSubPlan)
            {
                Plan.ElementSubPlans.Add(elementBase as ElementSubPlan);
            }
            if (elementBase is ElementDevice)
            {
                Plan.ElementDevices.Add(elementBase as ElementDevice);
            }

            var designerItem = Create(elementBase);

            if (elementBase is ElementDevice)
            {
                var elementDevice = elementBase as ElementDevice;
                if (elementDevice.Device == null)
                {
                    elementDevice.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                }
                elementDevice.Device.PlanElementUIDs.Add(elementBase.UID);
                ServiceFactory.Events.GetEvent<DeviceAddedEvent>().Publish(elementDevice.Device.UID);
            }

            return designerItem;
        }

        public DesignerItem Create(ElementBase elementBase)
        {
            var designerItem = new DesignerItem()
            {
                MinWidth = 10,
                MinHeight = 10,
                ElementBase = elementBase,
                IsPolygon = elementBase is ElementBasePolygon,
                IsDevice = elementBase is ElementDevice,
                IsPolyline = elementBase is ElementPolyline,
                Opacity = ((elementBase is IElementZone) || (elementBase is ElementSubPlan)) ? 0.5 : 1
            };

            this.Children.Add(designerItem);
            designerItem.Redraw();
            SetZIndex(designerItem, elementBase);
            return designerItem;
        }

        void SetZIndex(DesignerItem designerItem, ElementBase elementBase)
        {
            int bigConstatnt = 100000;

            if (elementBase is IZIndexedElement)
                Panel.SetZIndex(designerItem, (elementBase as IZIndexedElement).ZIndex);

            if (elementBase is ElementSubPlan)
                Panel.SetZIndex(designerItem, 1 * bigConstatnt);

            if (elementBase is IElementZone)
            {
                Panel.SetZIndex(designerItem, 2 * bigConstatnt);
                IElementZone elementZone = elementBase as IElementZone;
                if (elementZone.Zone != null)
                {
                    if (elementZone.Zone.ZoneType == ZoneType.Fire)
                        Panel.SetZIndex(designerItem, 3 * bigConstatnt);

                    if (elementZone.Zone.ZoneType == ZoneType.Guard)
                        Panel.SetZIndex(designerItem, 4 * bigConstatnt);
                }
            }

            if (elementBase is ElementDevice)
                Panel.SetZIndex(designerItem, 5 * bigConstatnt);
        }

        void On_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPointAdding)
            {
                if (SelectedItems == null)
                    return;
                var selectedItem = SelectedItems.FirstOrDefault();
                if (selectedItem == null)
                    return;
                if ((selectedItem.IsPolygon == false) && (selectedItem.IsPolyline == false))
                    return;

                IsPointAdding = false;

                PointCollection pointCollection = null;
                if (selectedItem.Content is Polygon)
                {
                    var polygon = selectedItem.Content as Polygon;
                    pointCollection = polygon.Points;
                }
                if (selectedItem.Content is Polyline)
                {
                    var polyline = selectedItem.Content as Polyline;
                    pointCollection = polyline.Points;
                }

                Point currentPoint = e.GetPosition(selectedItem);
                var minDistance = double.MaxValue;
                int minIndex = 0;
                for (int i = 0; i < pointCollection.Count; ++i)
                {
                    var polygonPoint = pointCollection[i];
                    var distance = Math.Pow(currentPoint.X - polygonPoint.X, 2) + Math.Pow(currentPoint.Y - polygonPoint.Y, 2);
                    if (distance < minDistance)
                    {
                        minIndex = i;
                        minDistance = distance;
                    }
                }
                minIndex = minIndex + 1;
                Point point = e.GetPosition(selectedItem);
                pointCollection.Insert(minIndex, point);

                var designerItem = Items.FirstOrDefault(x => x.IsPointAdding);
                designerItem.UpdatePolygonAdorner();

                e.Handled = true;
            }

            return;
            bool isPolyLineDrawing = true;
            if (isPolyLineDrawing)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point point = e.GetPosition(this);
                    Ellipse ellipse = new Ellipse()
                    {
                        Width = 10,
                        Height = 10,
                        Fill = new SolidColorBrush(Colors.Red)
                    };
                    Canvas.SetLeft(ellipse, point.X);
                    Canvas.SetTop(ellipse, point.Y);
                    this.Children.Add(ellipse);
                }
                else
                {
                    isPolyLineDrawing = false;
                }

                e.Handled = true;
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
            var designerPropertiesViewModel = new DesignerPropertiesViewModel(Plan);
            if (ServiceFactory.UserDialogs.ShowModalWindow(designerPropertiesViewModel))
            {
                Update();
                ServiceFactory.SaveService.PlansChanged = true;
            }
        }

        public void Update()
        {
            Width = Plan.Width;
            Height = Plan.Height;

            Background = new SolidColorBrush(Plan.BackgroundColor);

            if (Plan.BackgroundPixels != null)
            {
                Background = PlanElementsHelper.CreateBrush(Plan.BackgroundPixels);
            }
        }

        public List<ElementBase> CloneSelectedElements()
        {
            initialElements = new List<ElementBase>();

            foreach (var designerItem in SelectedItems)
            {
                designerItem.SavePropertiesToElementBase();
                initialElements.Add(designerItem.ElementBase.Clone());
            }

            return initialElements;
        }

        public void BeginChange()
        {
            initialElements = CloneSelectedElements();
        }

        public void EndChange()
        {
            ServiceFactory.Events.GetEvent<ElementChangedEvent>().Publish(initialElements);
        }

        public void UpdateZoom()
        {
            foreach (DesignerItem designerItem in this.SelectedItems)
            {
                designerItem.UpdateZoom();
            }

            foreach (DesignerItem designerItem in this.Items)
            {
                designerItem.UpdateZoomDevice();
            }
        }
    }
}