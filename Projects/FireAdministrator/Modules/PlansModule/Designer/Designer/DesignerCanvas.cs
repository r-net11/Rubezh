using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using System.Windows.Shapes;

namespace PlansModule.Designer
{
    public class DesignerCanvas : Canvas
    {
        public Plan Plan { get; set; }
        private Point? dragStartPoint = null;

        public DesignerCanvas()
        {
            PreviewMouseDown += new MouseButtonEventHandler(On_PreviewMouseDown);
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

        public void DeselectAll()
        {
            foreach (DesignerItem item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
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
                    RubberbandAdorner adorner = new RubberbandAdorner(this, this.dragStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                e.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            DesignerItemData designerItemData = e.Data.GetData("DESIGNER_ITEM") as DesignerItemData;

            var elementBase = designerItemData.PlansElement as ElementBase;
            Point position = e.GetPosition(this);
            elementBase.Left = Math.Max(0, position.X - elementBase.Width / 2);
            elementBase.Top = Math.Max(0, position.Y - elementBase.Height / 2);

            if (designerItemData.PlansElement is ElementRectangle)
            {
                Plan.ElementRectangles.Add(elementBase as ElementRectangle);
            }
            if (designerItemData.PlansElement is ElementEllipse)
            {
                Plan.ElementEllipses.Add(elementBase as ElementEllipse);
            }
            if (designerItemData.PlansElement is ElementPolygon)
            {
                Plan.ElementPolygons.Add(elementBase as ElementPolygon);
            }
            if (designerItemData.PlansElement is ElementTextBlock)
            {
                Plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
            }
            if (designerItemData.PlansElement is ElementDevice)
            {
                Plan.ElementDevices.Add(elementBase as ElementDevice);
            }

            DesignerItem designerItem = null;
            if (designerItemData.PlansElement is ElementDevice)
            {
                var elementDevice = designerItemData.PlansElement as ElementDevice;
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.Id);
                var deviceControl = new DeviceControl()
                {
                    DriverId = device.Driver.UID,
                    StateType = StateType.Norm
                };
                designerItem = Create(elementDevice, frameworkElement: deviceControl);
            }
            else
            {
                designerItem = Create(elementBase);
            }

            this.DeselectAll();
            designerItem.IsSelected = true;

            e.Handled = true;
        }

        public DesignerItem Create(ElementBase elementBase, bool isOpacity = false, FrameworkElement frameworkElement = null)
        {
            if (frameworkElement == null)
            {
                frameworkElement = elementBase.Draw();
            }
            frameworkElement.IsHitTestVisible = false;

            var designerItem = new DesignerItem()
            {
                MinWidth = 10,
                MinHeight = 10,
                Width = elementBase.Width,
                Height = elementBase.Height,
                Content = frameworkElement,
                ElementBase = elementBase,
                IsPolygon = elementBase is ElementBasePolygon
            };

            if (isOpacity)
                designerItem.Opacity = 0.5;

            DesignerCanvas.SetLeft(designerItem, elementBase.Left);
            DesignerCanvas.SetTop(designerItem, elementBase.Top);
            this.Children.Add(designerItem);

            return designerItem;
        }

        public bool IsPointAdding = false;

        void On_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPointAdding)
            {
                if (SelectedItems == null)
                    return;
                var selectedItem = SelectedItems.FirstOrDefault();
                if (selectedItem == null)
                    return;
                if (selectedItem.IsPolygon == false)
                    return;

                IsPointAdding = false;

                var item = selectedItem;
                var polygon = item.Content as Polygon;


                Point currentPoint = e.GetPosition(selectedItem);
                var minDistance = double.MaxValue;
                int minIndex = 0;
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    var polygonPoint = polygon.Points[i];
                    var distance = Math.Pow(currentPoint.X - polygonPoint.X, 2) + Math.Pow(currentPoint.Y - polygonPoint.Y, 2);
                    if (distance < minDistance)
                    {
                        minIndex = i;
                        minDistance = distance;
                    }
                }
                minIndex = minIndex + 1;
                Point point = e.GetPosition(selectedItem);
                polygon.Points.Insert(minIndex, point);


                PolygonResizeChrome.Current.Initialize();

                e.Handled = true;
            }
        }
    }
}
