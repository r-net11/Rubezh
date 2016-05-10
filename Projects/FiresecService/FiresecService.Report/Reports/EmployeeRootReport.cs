using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RubezhAPI;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeRootReport : BaseReport<EmployeeRootReportData>
	{
		public override EmployeeRootReportData CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<EmployeeRootReportFilter>(f);
			var result = new EmployeeRootReportData();
			result.Employees = new List<EmployeeRootData>();
			result.Data = new List<EmployeeAdditionalData>();

			var employees = dataProvider.GetEmployees(filter);
			var passJournal = dataProvider.DbService.PassJournalTranslator != null ?
				dataProvider.DbService.PassJournalTranslator.GetEmployeesRoot(employees.Select(item => item.UID), filter.Zones, filter.DateTimeFrom, filter.DateTimeTo) : null;

			var zoneMap = new Dictionary<Guid, string>();
			if (passJournal != null)
			{
				foreach (var zone in GKManager.SKDZones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
			}

			foreach (var employee in employees)
			{
				var employeeRow = new EmployeeRootData();
				employeeRow.UID = employee.UID;
				employeeRow.Name = employee.Name;
				employeeRow.Department = employee.Department;
				employeeRow.Position = employee.Position;
				employeeRow.Organisation = employee.Organisation;

				if (!filter.IsEmployee)
				{
					var escortUID = employee.Item.EscortUID;
					var escort = escortUID.HasValue ? dataProvider.GetEmployee(escortUID.Value) : null;
					if (escort != null)
					{
						employeeRow.Escort = escort.Name;
					}
					employeeRow.Description = employee.Item.Description;
				}

				result.Employees.Add(employeeRow);
				if (passJournal != null)
				{
					var dayPassJournals = passJournal.Where(item => item.EmployeeUID == employee.UID).GroupBy(x => x.EnterTime.Date);
					var timeTrackParts = new List<RubezhDAL.DataClasses.PassJournal>();
					foreach (var dayPassJournal in dayPassJournals)
					{
						var timeTrackDayParts = NormalizePassJournals(dayPassJournal);
						timeTrackParts.AddRange(timeTrackDayParts);
					}

					foreach (var pass in timeTrackParts)
					{
						var row = new EmployeeAdditionalData();
						row.EmployeeUID = employeeRow.UID;
						if (zoneMap.ContainsKey(pass.ZoneUID))
							row.Zone = zoneMap[pass.ZoneUID];
						if (filter.DateTimeFrom.Ticks <= pass.EnterTime.Ticks && (!pass.ExitTime.HasValue || pass.ExitTime.Value.Ticks <= filter.DateTimeTo.Ticks))
						{
							row.DateTime = new DateTime(pass.EnterTime.Ticks);
							result.Data.Add(row);
						}
					}
				}
			}
			return result;
		}

		public static List<RubezhDAL.DataClasses.PassJournal> NormalizePassJournals(IEnumerable<RubezhDAL.DataClasses.PassJournal> passJournals)
		{
			if (passJournals.Count() == 0)
				return new List<RubezhDAL.DataClasses.PassJournal>();

			var result = passJournals.OrderBy(x => x.EnterTime).ToList();

			for (int i = result.Count - 1; i > 0; i--)
			{
				if (result[i].EnterTime == result[i - 1].ExitTime && result[i].ZoneUID == result[i - 1].ZoneUID)
				{
					result[i].EnterTime = result[i - 1].ExitTime.Value;
					result.RemoveAt(i - 1);
				}
			}
			return result;
		}
	}
}
