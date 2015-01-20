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
	public partial class Report418 : BaseSKDReport
	{
		public Report418()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Справка о сотруднике/посетителе"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter418>();
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

			var dataSet = new DataSet418();
			if (employeesResult.Result != null)
			{
				foreach (var employee in employeesResult.Result)
				{
					var dataRow = dataSet.Data.NewDataRow();
					dataRow.Document = employee.DocumentType.ToDescription();
					dataRow.DocumentNumber = employee.DocumentNumber;
					dataRow.FirstName = employee.FirstName;
					dataRow.SecondName = employee.SecondName;
					dataRow.Sex = employee.Gender.ToDescription();

					var organisationResult = databaseService.OrganisationTranslator.GetSingle(employee.OrganisationUID);
					if (organisationResult.Result != null)
					{
						dataRow.Organisation = organisationResult.Result.Name;
					}
					if (employee.Department != null)
					{
						dataRow.Department = employee.Department.Name;
					}
					if (employee.Position != null)
					{
						dataRow.Position = employee.Position.Name;
					}

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