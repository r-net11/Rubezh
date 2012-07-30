using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void SetZoneGuard(Zone zone)
		{
			if ((zone.ZoneType == ZoneType.Guard) && (zone.SecPanelUID != Guid.Empty))
			{
				var localNo = GetZoneLocalSecNo(zone);
				if (localNo >= 0)
				{
					FiresecService.SetZoneGuard(zone.SecPanelUID, localNo);
				}
			}
		}

		public static void UnSetZoneGuard(Zone zone)
		{
			if ((zone.ZoneType == ZoneType.Guard) && (zone.SecPanelUID != Guid.Empty))
			{
				var localNo = GetZoneLocalSecNo(zone);
				FiresecService.UnSetZoneGuard(zone.SecPanelUID, localNo);
			}
		}
	}
}