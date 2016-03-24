using Common;
using FiresecService.Report.DataSources;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class DoorsReport : BaseReport
	{
		public DoorsReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Альбомная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return true; }
		}

		public override string ReportTitle
		{
			get { return "Список точек доступа"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<DoorsReportFilter>();
			if (!filter.ZoneIn && !filter.ZoneOut)
			{
				filter.ZoneIn = true;
				filter.ZoneOut = true;
			}

			var dataSet = new DoorsDataSet();

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
						var dataRow = dataSet.Data.NewDataRow();
						dataRow.Number = door.No;
						dataRow.Door = door.Name;
						dataRow.Type = ((Enum)door.DoorType).ToDescription();
						dataRow.Comment = door.Description;
						if (door.EnterDevice != null)
						{
							dataRow.EnterReader = door.EnterDevice.PresentationName;
							var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.EnterZoneUID);
							if (enterZone != null)
							{
								dataRow.EnterZone = enterZone.Name;
							}
						}
						if (door.ExitDevice != null)
						{
							dataRow.ExitReader = door.ExitDevice.PresentationName;
							var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == door.ExitZoneUID);
							if (exitZone != null)
							{
								dataRow.ExitZone = exitZone.Name;
							}
						}
						dataRow.Organisation = organisation.Name;
						dataSet.Data.Rows.Add(dataRow);
					}));
			return dataSet;
		}
	}
}