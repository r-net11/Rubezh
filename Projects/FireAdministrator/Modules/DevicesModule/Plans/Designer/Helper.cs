using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;

namespace DevicesModule.Plans.Designer
{
	public static class Helper
	{
		public static Zone GetZone(IElementZone element)
		{
			return element.ZoneNo.HasValue ? FiresecManager.Zones.FirstOrDefault(x => x.No == element.ZoneNo.Value) : null;
		}
		public static Plan GetPlan(ElementSubPlan element)
		{
			return FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == element.PlanUID);
		}
		public static void SetZone(IElementZone element)
		{
			Zone zone = GetZone(element);
			element.BackgroundColor = GetZoneColor(zone);
		}
		public static void SetZone(IElementZone element, Zone zone)
		{
			element.ZoneNo = zone == null ? null : (int?)zone.No;
			element.BackgroundColor = GetZoneColor(zone);
		}
		private static Color GetZoneColor(Zone zone)
		{
			Color color = Colors.Gray;
			if (zone != null)
			{
				if (zone.ZoneType == ZoneType.Fire)
					color = Colors.Green;

				if (zone.ZoneType == ZoneType.Guard)
					color = Colors.Brown;
			}
			return color;
		}

		public static Device GetDevice(ElementDevice element)
		{
			return element.DeviceUID == null ? null : FiresecManager.Devices.FirstOrDefault(x => x.UID == element.DeviceUID);
		}
		public static void SetDevice(ElementDevice element, Device device)
		{
			ResetDevice(element);
			element.DeviceUID = device.UID;
			device.PlanElementUIDs.Add(element.UID);
		}
		public static Device SetDevice(ElementDevice element)
		{
			Device device = GetDevice(element);
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
			return device;
		}
		public static void ResetDevice(ElementDevice element)
		{
			Device device = GetDevice(element);
			if (device != null)
				device.PlanElementUIDs.Remove(element.UID);
		}

		public static string GetZoneTitle(IElementZone element)
		{
			Zone zone = GetZone(element);
			return zone == null ? "Несвязанная зона" : zone.PresentationName;
		}
		public static string GetSubPlanTitle(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			return plan == null ? "Несвязанный подплан" : plan.Caption;
		}
		public static string GetDeviceTitle(ElementDevice element)
		{
			var device = GetDevice(element);
			return device == null ? "Неизвестное устройство" : device.DottedAddress + " " + device.Driver.ShortName;
		}

		public static bool IsPointInPolygon(Point point, Polygon polygon)
		{
			if (polygon == null)
				return false;

			var j = polygon.Points.Count - 1;
			var oddNodes = false;

			for (var i = 0; i < polygon.Points.Count; i++)
			{
				if ((polygon.Points[i].Y < point.Y && polygon.Points[j].Y >= point.Y || polygon.Points[j].Y < point.Y && polygon.Points[i].Y >= point.Y) &&
					(polygon.Points[i].X + (point.Y - polygon.Points[i].Y) / (polygon.Points[j].Y - polygon.Points[i].Y) * (polygon.Points[j].X - polygon.Points[i].X) < point.X))
					oddNodes = !oddNodes;
				j = i;
			}

			return oddNodes;
		}
	}
}
