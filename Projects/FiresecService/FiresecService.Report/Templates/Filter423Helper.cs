using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using SKDDriver;
using FiresecAPI.SKD;

namespace FiresecService.Report.Templates
{
	public class Filter423Helper
	{
		public List<TimeTrackDocument> GetData(ReportFilter423 filter)
		{
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

			var documents = new List<TimeTrackDocument>();
			foreach (var employee in employees)
			{
				var documentsResult = databaseService.TimeTrackDocumentTranslator.Get(employee.UID, filter.DateTimeFrom, filter.DateTimeTo);
				if (documentsResult.Result != null)
				{
					foreach (var document in documents)
					{
						if (filter.Abcense && document.TimeTrackDocumentType.DocumentType == DocumentType.Absence ||
						   filter.Presence && document.TimeTrackDocumentType.DocumentType == DocumentType.Presence ||
							filter.Overtime && document.TimeTrackDocumentType.DocumentType == DocumentType.Overtime)
						{
							documents.Add(document);
						}
					}
				}
			}

			return documents;
		}
	}
}