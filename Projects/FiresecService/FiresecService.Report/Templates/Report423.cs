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
	public partial class Report423 : BaseSKDReport
	{
		public Report423()
		{
			InitializeComponent();
		}

		public override string ReportTitle
		{
			get { return "Отчет по оправдательным документам"; }
		}
		protected override DataSet CreateDataSet()
		{
			var filter = GetFilter<ReportFilter423>();

			var databaseService = new SKDDatabaseService();

			var employees = new List<Employee>();
			if (filter.Employees == null)
				filter.Employees = new List<Guid>();
			if (filter.Departments == null)
				filter.Departments = new List<Guid>();
			if (filter.Organisations == null)
				filter.Organisations = new List<Guid>();

			var employeeFilter = new EmployeeFilter();
			employeeFilter.OrganisationUIDs = filter.Organisations;
			employeeFilter.DepartmentUIDs = filter.Departments;
			employeeFilter.UIDs = filter.Employees;
			var employeesResult = databaseService.EmployeeTranslator.Get(employeeFilter);
			if (employeesResult.Result != null)
			{
				employees = employeesResult.Result.ToList();
			}

			var ds = new DataSet423();
			foreach (var employee in employees)
			{
				var documentsResult = databaseService.TimeTrackDocumentTranslator.Get(employee.UID, filter.DateTimeFrom, filter.DateTimeTo);
				if (documentsResult.Result != null)
				{
					foreach (var document in documentsResult.Result)
					{
						var documentTypesResult = databaseService.TimeTrackDocumentTypeTranslator.Get(employee.OrganisationUID);
						if (documentTypesResult.Result != null)
						{
							var documentType = documentTypesResult.Result.FirstOrDefault(x => x.Code == document.DocumentCode);
							if (documentType == null)
							{
								documentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == document.DocumentCode);
							}
							if (documentType != null)
							{
								if (filter.Abcense && documentType.DocumentType == DocumentType.Absence ||
								   filter.Presence && documentType.DocumentType == DocumentType.Presence ||
									filter.Overtime && documentType.DocumentType == DocumentType.Overtime)
								{
									var row = ds.Data.NewDataRow();
									row.Employee = employee.Name;
									if (employee.Department != null)
										row.Department = employee.Department.Name;
									row.StartDateTime = document.StartDateTime;
									row.EndDateTime = document.EndDateTime;
									row.DocumentCode = documentType.Code;
									row.DocumentName = documentType.Name;
									row.DocumentShortName = documentType.ShortName;
									row.DocumentType = documentType.DocumentType.ToDescription();
									ds.Data.AddDataRow(row);
								}
							}
						}
					}
				}
			}
			return ds;
		}
	}
}
