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
	public partial class Report402 : BaseSKDReport
	{
		public Report402()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Маршрут сотрудника/посетителя"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter402>();

			var ds = new DataSet402();
			var databaseService = new SKDDatabaseService();

			var employees = new List<Employee>();
			if (filter.Employees == null)
				filter.Employees = new List<Guid>();
			if (filter.Departments == null)
				filter.Departments = new List<Guid>();
			if (filter.Positions == null)
				filter.Positions = new List<Guid>();
			if (filter.Organisations == null)
				filter.Organisations = new List<Guid>();

			var employeeFilter = new EmployeeFilter();
			employeeFilter.OrganisationUIDs = filter.Organisations;
			employeeFilter.DepartmentUIDs = filter.Departments;
			employeeFilter.PositionUIDs = filter.Positions;
			employeeFilter.UIDs = filter.Employees;
			var employeesResult = databaseService.EmployeeTranslator.Get(employeeFilter);
			if (employeesResult.Result != null)
			{
				employees = employeesResult.Result.ToList();
			}

			var zoneUIDs = new List<Guid>();
			if (filter.Zones != null)
				zoneUIDs.AddRange(filter.Zones);
			if (filter.Doors != null)
				foreach (var doorUID in filter.Doors)
				{
					var door = SKDManager.Doors.FirstOrDefault(x => x.UID == doorUID);
					if (door != null)
					{
						if (door.InDevice != null && door.InDevice.ZoneUID != Guid.Empty)
							zoneUIDs.Add(door.InDevice.ZoneUID);
						if (door.OutDevice != null && door.OutDevice.ZoneUID != Guid.Empty)
							zoneUIDs.Add(door.OutDevice.ZoneUID);
					}
				}
			var organisations = databaseService.OrganisationTranslator.Get(new OrganisationFilter());

			var result = new List<SKDDriver.DataAccess.PassJournal>();
			foreach (var employee in employees)
			{
				var employeeRow = ds.Employee.NewEmployeeRow();
				employeeRow.UID = employee.UID;
				employeeRow.Name = employee.Name;
				if (employee.Department != null)
					employeeRow.Department = employee.Department.Name;
				if (employee.Position != null)
					employeeRow.Position = employee.Position.Name;
				employeeRow.Organisation = organisations.Result.Where(item => item.UID == employee.OrganisationUID).Select(item => item.Name).FirstOrDefault();
				ds.Employee.AddEmployeeRow(employeeRow);

				if (databaseService.PassJournalTranslator != null)
				{
					var passJournal2 = databaseService.PassJournalTranslator.GetEmployeeRoot(employee.UID, zoneUIDs, filter.DateTimeFrom, filter.DateTimeTo);
					if (passJournal2 != null)
						foreach (var pass in passJournal2)
						{
							var row = ds.Data.NewDataRow();
							row.EmployeeRow = employeeRow;
							//row.Zone;
							//row.PassCard;
							//row.Door;
							row.DateTime = pass.EnterTime;
							ds.Data.AddDataRow(row);
							if (pass.ExitTime.HasValue)
							{
								var row2 = ds.Data.NewDataRow();
								row2.ItemArray = row.ItemArray;
								row2.DateTime = pass.ExitTime.Value;
								ds.Data.AddDataRow(row2);
							}
						}
				}
			}
			return ds;
		}
	}
}
