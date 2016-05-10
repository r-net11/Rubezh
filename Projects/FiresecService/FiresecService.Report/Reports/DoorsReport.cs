using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class DoorsReport : BaseReport<List<DoorData>>
	{
		public override List<DoorData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<DoorsReportFilter>(f);
			if (!filter.ZoneIn && !filter.ZoneOut)
			{
				filter.ZoneIn = true;
				filter.ZoneOut = true;
			}

			var result = new List<DoorData>();

			IEnumerable<GKDoor> doors = GKManager.Doors;
			if (!filter.Doors.IsEmpty())
				doors = doors.Where(item => filter.Doors.Contains(item.UID));
			if (!filter.Zones.IsEmpty())
				doors = doors.Where(item =>
					(filter.ZoneIn && filter.Zones.Contains(item.EnterZoneUID)) ||
					(filter.ZoneOut && filter.Zones.Contains(item.ExitZoneUID)));

			var organisationsResult = dataProvider.DbService.OrganisationTranslator.Get(new OrganisationFilter() { UIDs = filter.Organisations ?? new List<Guid>() });
			if (organisationsResult.Result != null)
				organisationsResult.Result.ForEach(organisation =>
					doors.Where(item => organisation.DoorUIDs.Contains(item.UID)).ForEach(door =>
					{
						var data = new DoorData();
						data.Number = door.No;
						data.Door = door.Name;
						data.Type = ((Enum)door.DoorType).ToDescription();
						data.Comment = door.Description;
						if (door.EnterDevice != null)
						{
							data.EnterReader = door.EnterDevice.PresentationName;
							var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
							if (enterZone != null)
							{
								data.EnterZone = enterZone.Name;
							}
						}
						if (door.ExitDevice != null)
						{
							data.ExitReader = door.ExitDevice.PresentationName;
							var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
							if (exitZone != null)
							{
								data.ExitZone = exitZone.Name;
							}
						}
						data.Organisation = organisation.Name;
						result.Add(data);
					}));
			return result;
		}
	}
}
