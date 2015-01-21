using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using FiresecAPI.SKD;
using SKDDriver;

namespace FiresecService.Report.Templates
{
	public class Filter431Helper
	{
		public List<SKDDoor> GetDoorsByFilter(ReportFilter431 filter)
		{
			var doors = new List<SKDDoor>();
			foreach (var door in SKDManager.Doors)
			{
				if (filter.Doors != null && filter.Doors.Count > 0)
				{
					if (!filter.Doors.Contains(door.UID))
						continue;
				}

				if (filter.Zones != null && filter.Zones.Count > 0)
				{
					var hasZone = false;
					if (door.InDevice != null && filter.Zones.Contains(door.InDevice.ZoneUID))
						hasZone = true;
					if (door.OutDevice != null && filter.Zones.Contains(door.OutDevice.ZoneUID))
						hasZone = true;
					if (!hasZone)
						continue;
				}

				var databaseService = new SKDDatabaseService();

				if (filter.Organisations != null && filter.Organisations.Count > 0)
				{
					var doorUIDs = new List<Guid>();
					foreach(var organisationUID in filter.Organisations)
					{
						var organisationResult = databaseService.OrganisationTranslator.GetSingle(organisationUID);
						if (organisationResult.Result != null)
						{
							doorUIDs.AddRange(organisationResult.Result.DoorUIDs);
						}
					}
					if (!doorUIDs.Contains(door.UID))
						continue;
				}
			}
			return doors;
		}
	}
}