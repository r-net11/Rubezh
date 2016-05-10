using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RubezhAPI;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmployeeZonesReport : BaseReport<List<EmployeeZonesData>>
	{
		public override List<EmployeeZonesData> CreateDataSet(DataProvider dataProvider, SKDReportFilter f)
		{
			var filter = GetFilter<EmployeeZonesReportFilter>(f);
			if (filter.UseCurrentDate)
				filter.ReportDateTime = DateTime.Now;

			var result = new List<EmployeeZonesData>();
			if (dataProvider.DbService.PassJournalTranslator != null)
			{
				var employees = dataProvider.GetEmployees(filter);
				var zoneMap = new Dictionary<Guid, string>();
				foreach (var zone in GKManager.SKDZones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
				var enterJournal = dataProvider.DbService.PassJournalTranslator.GetEmployeesLastEnterPassJournal(
					employees.Select(item => item.UID), filter.Zones, filter.ReportDateTime);
				foreach (var record in enterJournal)
					AddRecord(dataProvider, result, record, filter, true, zoneMap);
			}
			return result;
		}

		private void AddRecord(DataProvider dataProvider, List<EmployeeZonesData> ds, RubezhDAL.DataClasses.PassJournal record, EmployeeZonesReportFilter filter, bool isEnter, Dictionary<Guid, string> zoneMap)
		{
			if (record.EmployeeUID == null)
				return;
			var data = new EmployeeZonesData();
			var employee = dataProvider.GetEmployee(record.EmployeeUID.Value);
			data.Employee = employee.Name;
			data.Organisation = employee.Organisation;
			data.Department = employee.Department;
			data.Position = employee.Position;
			data.Zone = zoneMap.ContainsKey(record.ZoneUID) ? zoneMap[record.ZoneUID] : null;
			data.EnterDateTime = record.EnterTime;
			if (record.ExitTime.HasValue)
			{
				data.ExitDateTime = record.ExitTime.Value;
				data.Period = data.ExitDateTime - data.EnterDateTime;
			}
			else
			{
				data.ExitDateTime = filter.ReportDateTime;
				data.Period = filter.ReportDateTime - data.EnterDateTime;
			}

			if (!filter.IsEmployee)
			{
				var escortUID = employee.Item.EscortUID;
				if (escortUID.HasValue)
				{
					var escort = dataProvider.GetEmployee(escortUID.Value);
					data.Escort = escort.Name;
				}
			}
			ds.Add(data);
		}

	}
}
