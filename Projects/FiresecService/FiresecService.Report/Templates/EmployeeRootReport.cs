using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecClient;
using FiresecService.Report.DataSources;

namespace FiresecService.Report.Templates
{
	public partial class EmployeeRootReport : BaseReport
	{
		public EmployeeRootReport()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Портретная ориентация листа согласно требованиям http://172.16.6.113:26000/pages/viewpage.action?pageId=6948166
		/// </summary>
		protected override bool ForcedLandscape
		{
			get { return false; }
		}

		public override string ReportTitle
		{
			get { return "Маршрут " + (GetFilter<EmployeeRootReportFilter>().IsEmployee ? "сотрудника" : "посетителя"); }
		}

		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
			Name = ReportTitle;
		}

		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeRootReportFilter>();
			var ds = new EmployeeRootDataSet();
			var employees = dataProvider.GetEmployees(filter);
			var passJournal = dataProvider.DatabaseService.PassJournalTranslator != null ? 
				dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesRoot(employees.Select(item => item.UID), filter.Zones, filter.DateTimeFrom, filter.DateTimeTo) : null;

			var zoneMap = new Dictionary<Guid, string>();
			if (passJournal != null)
			{
				foreach (var zone in SKDManager.Zones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
				foreach (var zone in GKManager.SKDZones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
			}

			foreach (var employee in employees)
			{
				var employeeRow = ds.Employee.NewEmployeeRow();
				employeeRow.UID = employee.UID;
				employeeRow.Name = employee.Name;
				employeeRow.Department = employee.Department;
				employeeRow.Position = employee.Position;
				employeeRow.Organisation = employee.Organisation;
				ds.Employee.AddEmployeeRow(employeeRow);
				if (passJournal != null)
				{
					var dayPassJournals = passJournal.Where(item => item.EmployeeUID == employee.UID).GroupBy(x => x.EnterTime.Date);
					var timeTrackParts = new List<TimeTrackPart>();
					foreach (var item in dayPassJournals)
					{
						var timeTrackDayParts = new List<TimeTrackPart>();
						foreach (var pass in item)
						{
							timeTrackDayParts.Add(new TimeTrackPart { StartTime = new TimeSpan(pass.EnterTime.Ticks), EndTime = new TimeSpan(pass.ExitTime.HasValue ? pass.ExitTime.Value.Ticks : 0), PassJournalUID = pass.UID, ZoneUID = pass.ZoneUID });
						}
						timeTrackDayParts = DayTimeTrack.NormalizeTimeTrackParts(timeTrackDayParts);
						timeTrackParts.AddRange(timeTrackDayParts);
					}
					foreach (var pass in timeTrackParts)
					{
						var row = ds.Data.NewDataRow();
						row.EmployeeRow = employeeRow;
						if (zoneMap.ContainsKey(pass.ZoneUID))
							row.Zone = zoneMap[pass.ZoneUID];
						if (filter.DateTimeFrom.Ticks <= pass.StartTime.Ticks && pass.EndTime.Ticks <= filter.DateTimeTo.Ticks)
						{
							row.DateTime = new DateTime(pass.StartTime.Ticks);
							ds.Data.AddDataRow(row);
						}
					}
				}
			}
			return ds;
		}
	}
}