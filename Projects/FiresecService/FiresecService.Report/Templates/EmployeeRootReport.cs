using System.Data;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.DataSources;
using System.Collections.Generic;
using System;
using FiresecClient;

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
			get { return "Маршрут сотрудника (посетителя)"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<EmployeeRootReportFilter>();
			var ds = new EmployeeRootDataSet();
			var employees = dataProvider.GetEmployees(filter);
			var passJournal = dataProvider.DatabaseService.PassJournalTranslator != null ? dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesRoot(employees.Select(item => item.UID), filter.Zones, filter.DateTimeFrom, filter.DateTimeTo) : null;

			var zoneMap = new Dictionary<Guid, string>();
			if (passJournal != null)
			{
				foreach (var zone in SKDManager.Zones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
				foreach (var zone in GKManager.Zones)
				{
					zoneMap.Add(zone.UID, zone.PresentationName);
				}
			}
			else
			{
				zoneMap = null;
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
					foreach (var pass in passJournal.Where(item => item.EmployeeUID == employee.UID))
					{
						var row = ds.Data.NewDataRow();
						row.EmployeeRow = employeeRow;
						if (zoneMap.ContainsKey(pass.ZoneUID))
							row.Zone = zoneMap[pass.ZoneUID];
						if (filter.DateTimeFrom <= pass.EnterTime && pass.EnterTime <= filter.DateTimeTo)
						{
							row.DateTime = pass.EnterTime;
							ds.Data.AddDataRow(row);
						}
						if (pass.ExitTime.HasValue && filter.DateTimeFrom <= pass.ExitTime && pass.ExitTime <= filter.DateTimeTo)
						{
							var row2 = ds.Data.NewDataRow();
							row2.ItemArray = row.ItemArray;
							row2.DateTime = pass.ExitTime.Value;
							ds.Data.AddDataRow(row2);
						}
					}
			}
			return ds;
		}
	}
}