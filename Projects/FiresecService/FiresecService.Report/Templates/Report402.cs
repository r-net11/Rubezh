using System;
using FiresecAPI;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;

namespace FiresecService.Report.Templates
{
	public partial class Report402 : BaseReport
	{
		public Report402()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Маршрут сотрудника/посетителя"; }
		}
		protected override DataSet CreateDataSet(DataProvider dataProvider)
		{
			var filter = GetFilter<ReportFilter402>();
			var ds = new DataSet402();
			var employees = dataProvider.GetEmployees(filter);
			var passJournal = dataProvider.DatabaseService.PassJournalTranslator != null ? dataProvider.DatabaseService.PassJournalTranslator.GetEmployeesRoot(employees.Select(item => item.UID), filter.Zones, filter.DateTimeFrom, filter.DateTimeTo) : null;
			var zoneMap = passJournal != null ? SKDManager.Zones.ToDictionary(item => item.UID) : null;
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
							row.Zone = zoneMap[pass.ZoneUID].PresentationName;
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
