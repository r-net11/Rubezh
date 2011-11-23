using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        public DesignerCanvas DesignerCanvas;
        public Plan Plan;

        public PlanDesignerViewModel()
        {
            InitializeZIndexCommands();
            ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(x => { UpdateDeviceInZones(); });
        }

        public void Initialize(Plan plan)
        {
            Plan = plan;
            DesignerCanvas.Plan = plan;
            DesignerCanvas.PlanDesignerViewModel = this;
            DesignerCanvas.Update();
            DesignerCanvas.Children.Clear();
            DesignerCanvas.Width = plan.Width;
            DesignerCanvas.Height = plan.Height;
            PlanDesignerView.Update();

            foreach (var elementRectangle in plan.ElementRectangles)
            {
                DesignerCanvas.Create(elementRectangle);
            }

            foreach (var elementEllipse in plan.ElementEllipses)
            {
                DesignerCanvas.Create(elementEllipse);
            }

            foreach (var elementTextBlock in plan.ElementTextBlocks)
            {
                DesignerCanvas.Create(elementTextBlock);
            }

            foreach (var elementPolygon in plan.ElementPolygons)
            {
                DesignerCanvas.Create(elementPolygon);
            }

            foreach (var elementRectangleZone in plan.ElementRectangleZones)
            {
                DesignerCanvas.Create(elementRectangleZone);
            }

            foreach (var ElementPolygonZone in plan.ElementPolygonZones)
            {
                DesignerCanvas.Create(ElementPolygonZone);
            }

            foreach (var elementDevice in plan.ElementDevices)
            {
                if (elementDevice.Device != null)
                {
                    var devicePicture = DeviceControl.GetDefaultPicture(elementDevice.Device.Driver.UID);
                    DesignerCanvas.Create(elementDevice, frameworkElement: devicePicture);
                }
            }

            foreach (var ElementSubPlan in plan.ElementSubPlans)
            {
                DesignerCanvas.Create(ElementSubPlan);
            }

            DesignerCanvas.DeselectAll();
            PlanDesignerView.Update();
        }

        public void Save()
        {
            if (Plan == null)
                return;

            ChangeZoom(1);

            NormalizeZIndex();
            Plan.ClearElements();

            foreach (var designerItem in DesignerCanvas.Items)
            {
                ElementBase elementBase = designerItem.ElementBase;
                elementBase.Left = Canvas.GetLeft(designerItem);
                elementBase.Top = Canvas.GetTop(designerItem);
                elementBase.Width = designerItem.Width;
                elementBase.Height = designerItem.Height;

                if (elementBase is ElementBasePolygon)
                {
                    ElementBasePolygon elementPolygon = elementBase as ElementBasePolygon;
                    elementPolygon.PolygonPoints = new System.Windows.Media.PointCollection((designerItem.Content as Polygon).Points);
                }

                if (elementBase is ElementRectangle)
                {
                    ElementRectangle elementRectangle = elementBase as ElementRectangle;
                    Plan.ElementRectangles.Add(elementRectangle);
                }
                if (elementBase is ElementEllipse)
                {
                    ElementEllipse elementEllipse = elementBase as ElementEllipse;
                    Plan.ElementEllipses.Add(elementEllipse);
                }
                if (elementBase is ElementTextBlock)
                {
                    ElementTextBlock elementTextBlock = elementBase as ElementTextBlock;
                    Plan.ElementTextBlocks.Add(elementTextBlock);
                }
                if (elementBase is ElementPolygon)
                {
                    ElementPolygon elementPolygon = elementBase as ElementPolygon;
                    Plan.ElementPolygons.Add(elementPolygon);
                }
                if (elementBase is ElementPolygonZone)
                {
                    ElementPolygonZone elementPolygonZone = elementBase as ElementPolygonZone;
                    Plan.ElementPolygonZones.Add(elementPolygonZone);
                }
                if (elementBase is ElementRectangleZone)
                {
                    ElementRectangleZone elementRectangleZone = elementBase as ElementRectangleZone;
                    Plan.ElementRectangleZones.Add(elementRectangleZone);
                }
                if (elementBase is ElementDevice)
                {
                    ElementDevice elementDevice = elementBase as ElementDevice;
                    Plan.ElementDevices.Add(elementDevice);
                }
                if (elementBase is ElementSubPlan)
                {
                    ElementSubPlan elementSubPlan = elementBase as ElementSubPlan;
                    Plan.ElementSubPlans.Add(elementSubPlan);
                }
            }
        }

        void UpdateDeviceInZones()
        {
            foreach (var designerItem in DesignerCanvas.Items)
            {
                ElementDevice elementDevice = designerItem.ElementBase as ElementDevice;
                if (elementDevice != null)
                {
                    var device = elementDevice.Device;

                    var zones = new List<ulong>();

                    foreach (var elementPolygonZoneItem in DesignerCanvas.Items)
                    {
                        ElementPolygonZone elementPolygonZone = elementPolygonZoneItem.ElementBase as ElementPolygonZone;
                        if (elementPolygonZone != null)
                        {
                            var point = new Point((int) Canvas.GetLeft(designerItem) - (int) Canvas.GetLeft(elementPolygonZoneItem),
                                (int) Canvas.GetTop(designerItem) - (int) Canvas.GetTop(elementPolygonZoneItem));
                            bool isInPolygon = IsPointInPolygon(point, elementPolygonZoneItem.Content as Polygon);
                            if (isInPolygon)
                                if (elementPolygonZone.ZoneNo.HasValue)
                                    zones.Add(elementPolygonZone.ZoneNo.Value);
                        }

                        ElementRectangleZone elementRectangleZone = elementPolygonZoneItem.ElementBase as ElementRectangleZone;
                        if (elementRectangleZone != null)
                        {
                            var point = new Point((int) Canvas.GetLeft(designerItem) - (int) Canvas.GetLeft(elementPolygonZoneItem),
                                (int) Canvas.GetTop(designerItem) - (int) Canvas.GetTop(elementPolygonZoneItem));

                            bool isInRectangle = ((point.X > 0) && (point.X < elementRectangleZone.Width) && (point.Y > 0) && (point.Y < elementRectangleZone.Height));

                            if (isInRectangle)
                                if (elementRectangleZone.ZoneNo.HasValue)
                                    zones.Add(elementRectangleZone.ZoneNo.Value);
                        }
                    }

                    if (device.ZoneNo.HasValue)
                    {
                        var isInZone = zones.Any(x => x == device.ZoneNo.Value);
                        if (isInZone == false)
                        {
                            if (zones.Count > 0)
                            {
                                device.ZoneNo = zones[0];
                                ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
                            }
                            else
                            {
                                device.ZoneNo = null;
                                ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
                            }
                        }
                    }
                    else
                    {
                        if (zones.Count > 0)
                        {
                            device.ZoneNo = zones[0];
                            ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
                        }
                    }
                }
            }
        }

        bool IsPointInPolygon(Point point, Polygon polygon)
        {
            var j = polygon.Points.Count - 1;
            var oddNodes = false;

            for (var i = 0; i < polygon.Points.Count; ++i)
            {
                if (polygon.Points[i].Y < point.Y && polygon.Points[j].Y >= point.Y ||
                    polygon.Points[j].Y < point.Y && polygon.Points[i].Y >= point.Y)
                {
                    if (polygon.Points[i].X +
                        (point.Y - polygon.Points[i].Y) / (polygon.Points[j].Y - polygon.Points[i].Y) * (polygon.Points[j].X - polygon.Points[i].X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }
    }
}
