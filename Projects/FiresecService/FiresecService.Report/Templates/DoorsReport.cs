using Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
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

			IEnumerable<SKDDoor> doors = SKDManager.Doors;
			if (!filter.Doors.IsEmpty())
				doors = doors.Where(item => filter.Doors.Contains(item.UID));
			if (!filter.Zones.IsEmpty())
				doors = doors.Where(item =>
					(filter.ZoneIn && item.InDevice != null && filter.Zones.Contains(item.InDevice.ZoneUID)) ||
					(filter.ZoneOut && item.OutDevice != null && filter.Zones.Contains(item.OutDevice.ZoneUID)));

			var organisationsResult = dataProvider.DatabaseService.OrganisationTranslator.Get(new OrganisationFilter() { UIDs = filter.Organisations ?? new List<Guid>() });
			if (organisationsResult.Result != null)
				organisationsResult.Result.ForEach(organisation =>
					doors.Where(item => organisation.DoorUIDs.Contains(item.UID)).ForEach(door =>
					{
						var dataRow = dataSet.Data.NewDataRow();
						dataRow.Number = door.No;
						dataRow.Door = door.Name;
						dataRow.Comment = door.Description;
						if (door.InDevice != null)
						{
							dataRow.EnterReader = door.InDevice.Name;
							if (door.InDevice.Zone != null)
								dataRow.EnterZone = door.InDevice.Zone.Name;
							if (door.InDevice.Parent != null)
							{
								dataRow.Controller = door.InDevice.Parent.Name;
								dataRow.IP = door.InDevice.Parent.Address;
							}
						}
						if (door.OutDevice != null)
						{
							dataRow.ExitReader = door.OutDevice.Name;
							if (door.OutDevice.Zone != null)
								dataRow.ExitZone = door.OutDevice.Zone.Name;
						}
						dataRow.Organisation = organisation.Name;
						dataSet.Data.Rows.Add(dataRow);
					}));
			return dataSet;
		}
	}
}