using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using FiresecService.Report.DataSources;
using System.Data;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using System.Collections.Generic;
using FiresecAPI.SKD;

namespace FiresecService.Report.Templates
{
    public partial class Report424: BaseSKDReport
	{
        public Report424()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
            get { return "Справка по отработанному времени"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter424>();
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

			var dataSet = new DataSet424();
			if (employeesResult.Result != null)
			{
				foreach (var employee in employeesResult.Result)
				{
					var dataRow = dataSet.Data.NewDataRow();

					dataRow.Employee = employee.Name;
					if (employee.Department != null)
					{
						dataRow.Department = employee.Department.Name;
					}
					if (employee.Position != null)
					{
						dataRow.Position = employee.Position.Name;
					}

					var timeTrackResult = databaseService.TimeTrackTranslator.GetTimeTracks(employeeFilter, filter.DateTimeFrom, filter.DateTimeTo);

					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}

		//protected override void UpdateDataSource()
		//{
		//    base.UpdateDataSource();
		//    FillTestData();
		//}
    }
}