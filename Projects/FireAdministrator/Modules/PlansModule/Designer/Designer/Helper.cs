using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using FiresecClient;
using System.Windows.Media;

namespace PlansModule.Designer.Designer
{
	public static class Helper
	{
		public static Zone GetZone(IElementZone element)
		{
			return element.ZoneNo.HasValue ? FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == element.ZoneNo.Value) : null;
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
			element.ZoneNo = zone == null ? null : (ulong?)zone.No;
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
	}
}
