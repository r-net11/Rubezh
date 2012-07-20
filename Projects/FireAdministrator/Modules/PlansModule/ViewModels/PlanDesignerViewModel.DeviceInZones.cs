using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Events;
using PlansModule.Designer.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace PlansModule.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel
	{
		private void UpdateDevice(List<ElementBase> elements)
		{
			foreach (var element in elements.OfType<ElementDevice>())
				Helper.ResetDevice(element);
		}
		public void UpdateDeviceInZones()
		{
			var deviceInZones = new Dictionary<Device, ulong?>();
			foreach (var designerItem in DesignerCanvas.Items)
			{
				ElementDevice elementDevice = designerItem.Element as ElementDevice;
				if (elementDevice != null)
				{
					var designerItemCenterX = Canvas.GetLeft(designerItem) + designerItem.Width / 2;
					var designerItemCenterY = Canvas.GetTop(designerItem) + designerItem.Height / 2;
					var device = Helper.GetDevice(elementDevice);
					if (device == null || device.Driver == null || !device.Driver.IsZoneDevice)
						continue;
					var zones = new List<ulong>();
					foreach (var elementPolygonZoneItem in DesignerCanvas.Items)
					{
						var point = new Point((int)(designerItemCenterX - Canvas.GetLeft(elementPolygonZoneItem)), (int)(designerItemCenterY - Canvas.GetTop(elementPolygonZoneItem)));
						ElementPolygonZone elementPolygonZone = elementPolygonZoneItem.Element as ElementPolygonZone;
						if (elementPolygonZone != null)
						{
							bool isInPolygon = IsPointInPolygon(point, elementPolygonZoneItem.Content as Polygon);
							if (isInPolygon && elementPolygonZone.ZoneNo.HasValue)
								zones.Add(elementPolygonZone.ZoneNo.Value);
						}
						ElementRectangleZone elementRectangleZone = elementPolygonZoneItem.Element as ElementRectangleZone;
						if (elementRectangleZone != null)
						{
							bool isInRectangle = ((point.X > 0) && (point.X < elementRectangleZone.Width) && (point.Y > 0) && (point.Y < elementRectangleZone.Height));
							if (isInRectangle && elementRectangleZone.ZoneNo.HasValue)
								zones.Add(elementRectangleZone.ZoneNo.Value);
						}
					}

					if (device.ZoneNo.HasValue)
					{
						var isInZone = zones.Any(x => x == device.ZoneNo.Value);
						if (!isInZone)
						{
							if (!deviceInZones.ContainsKey(device))
								deviceInZones.Add(device, zones.Count > 0 ? (ulong?)zones[0] : null);
							else if (zones.Count > 0)
								deviceInZones[device] = zones[0];
						}
					}
					else if (zones.Count > 0)
					{
						device.ZoneNo = zones[0];
						ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Publish(device.UID);
					}
				}
			}
			if (deviceInZones.Count > 0)
			{
				var deviceInZoneViewModel = new DevicesInZoneViewModel(deviceInZones);
				var result = DialogService.ShowModalWindow(deviceInZoneViewModel);
			}
		}

		bool IsPointInPolygon(Point point, Polygon polygon)
		{
			if (polygon == null)
				return false;

			var j = polygon.Points.Count - 1;
			var oddNodes = false;

			for (var i = 0; i < polygon.Points.Count; i++)
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

		private void ShowDeviceProperties(ShowPropertiesEventArgs e)
		{
			ElementDevice element = e.Element as ElementDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(element);
		}
	}
}