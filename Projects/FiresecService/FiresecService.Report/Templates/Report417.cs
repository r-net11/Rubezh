using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using System.Linq;
using FiresecService.Report.DataSources;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecAPI;

namespace FiresecService.Report.Templates
{
	public partial class Report417 : BaseSKDReport
	{
		public Report417()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Местонахождение сотрудников/посетителей"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter417>();
			var databaseService = new SKDDatabaseService();

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

			var dataSet = new DataSet417();
			if (employeesResult.Result != null)
			{
				foreach (var employee in employeesResult.Result)
				{
					var dataRow = dataSet.Data.NewDataRow();

					dataRow.Employee = employee.Name;
					var organisationResult = databaseService.OrganisationTranslator.GetSingle(employee.OrganisationUID);
					if (organisationResult.Result != null)
					{
						dataRow.Orgnisation = organisationResult.Result.Name;
					}
					if (employee.Department != null)
					{
						dataRow.Department = employee.Department.Name;
					}
					if (employee.Position != null)
					{
						dataRow.Position = employee.Position.Name;
					}

					var passJournal = databaseService.PassJournalTranslator.GetEmployeeLastPassJournal(employee.UID);
					if (passJournal != null)
					{
						dataRow.EnterDateTime = passJournal.EnterTime;
						if (passJournal.ExitTime.HasValue)
						{
							dataRow.ExitDateTime = passJournal.ExitTime.Value;
							dataRow.Period = passJournal.ExitTime.Value - passJournal.EnterTime;
						}
						var zone = SKDManager.Zones.FirstOrDefault(x=>x.UID == passJournal.ZoneUID);
						if(zone != null)
						{
							dataRow.Zone = zone.PresentationName;
						}
					}

					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}
	}
}