using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;
using Controls.MessageBox;

namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        public void UpdateDeviceInZones()
        {
            foreach (var designerItem in DesignerCanvas.Items)
            {
                ElementDevice elementDevice = designerItem.ElementBase as ElementDevice;
                if (elementDevice != null)
                {
                    var device = elementDevice.Device;
                    if (device.Driver.IsZoneDevice == false)
                        return;
                    var zones = new List<ulong>();

                    foreach (var elementPolygonZoneItem in DesignerCanvas.Items)
                    {
                        ElementPolygonZone elementPolygonZone = elementPolygonZoneItem.ElementBase as ElementPolygonZone;
                        if (elementPolygonZone != null)
                        {
                            var point = new Point((int)Canvas.GetLeft(designerItem) - (int)Canvas.GetLeft(elementPolygonZoneItem),
                                (int)Canvas.GetTop(designerItem) - (int)Canvas.GetTop(elementPolygonZoneItem));
                            bool isInPolygon = IsPointInPolygon(point, elementPolygonZoneItem.Content as Polygon);
                            if (isInPolygon)
                                if (elementPolygonZone.ZoneNo.HasValue)
                                    zones.Add(elementPolygonZone.ZoneNo.Value);
                        }

                        ElementRectangleZone elementRectangleZone = elementPolygonZoneItem.ElementBase as ElementRectangleZone;
                        if (elementRectangleZone != null)
                        {
                            var point = new Point((int)Canvas.GetLeft(designerItem) - (int)Canvas.GetLeft(elementPolygonZoneItem),
                                (int)Canvas.GetTop(designerItem) - (int)Canvas.GetTop(elementPolygonZoneItem));

                            bool isInRectangle = ((point.X > 0) && (point.X < elementRectangleZone.Width) && (point.Y > 0) && (point.Y < elementRectangleZone.Height));

                            if (isInRectangle)
                                if (elementRectangleZone.ZoneNo.HasValue)
                                    zones.Add(elementRectangleZone.ZoneNo.Value);
                        }
                    }

                    var deviceName = device.Driver.ShortName + " - " + device.DottedAddress;

                    var firstZoneName = "";
                    if (zones.Count > 0)
                    {
                        firstZoneName = zones[0].ToString();
                        var firsZone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zones[0]);
                        if (firsZone != null)
                            firstZoneName = firsZone.PresentationName;
                    }

                    var deviceZoneName = "";
                    if (device.ZoneNo.HasValue)
                    {
                        deviceZoneName = device.ZoneNo.ToString();
                        var deviceZone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                        if (deviceZone != null)
                            deviceZoneName = deviceZone.PresentationName;
                    }

                    if (device.ZoneNo.HasValue)
                    {
                        var isInZone = zones.Any(x => x == device.ZoneNo.Value);
                        if (isInZone == false)
                        {
                            if (zones.Count > 0)
                            {
                                if (MessageBoxService.ShowQuestion("Изменить зону устройства " + deviceName + " с " + deviceZoneName + " на " + firstZoneName + " ?") == System.Windows.MessageBoxResult.Yes)
                                {
                                    designerItem.IsSelected = true;
                                    device.ZoneNo = zones[0];
                                    ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
                                }
                            }
                            else
                            {
                                if (MessageBoxService.ShowQuestion("Удалить устройство " + deviceName + " из зоны " + deviceZoneName + " ?") == System.Windows.MessageBoxResult.Yes)
                                {
                                    designerItem.IsSelected = true;
                                    device.ZoneNo = null;
                                    ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (zones.Count > 0)
                        {
                            if (MessageBoxService.ShowQuestion("Добавить устройство " + deviceName + " в зону " + firstZoneName + " ?") == System.Windows.MessageBoxResult.Yes)
                            {
                                designerItem.IsSelected = true;
                                device.ZoneNo = zones[0];
                                ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
                            }
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
