﻿using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DocumentsReportProvider : FilteredSKDReportProvider<DocumentsReportFilter>
	{
		public DocumentsReportProvider()
			: base("Отчет по оправдательным документам", 423, SKDReportGroup.TimeTracking, PermissionType.Oper_Reports_Documents)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Employee", "Сотрудник" },
					{ "StartDateTime", "Дата начала" },
					{ "EndDateTime", "Дата окончания" },
					//{ "StartDateTime2", "Время начала" },
					//{ "EndDateTime2", "Время окончания" },
					{ "DocumentType", "Тип" },
					{ "DocumentName", "Документ" },
					{ "DocumentShortName", "Буквенный код" },
					{ "DocumentCode", "Числовой код" },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new OrganizationPageViewModel(false),
					new DepartmentPageViewModel(),
					new EmployeePageViewModel(),
					new DocumentFilterPageViewModel(),
				},
			};
		}
	}
}
