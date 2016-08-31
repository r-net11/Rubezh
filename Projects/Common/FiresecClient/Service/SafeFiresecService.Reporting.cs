using System;
using Common;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.Printing;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using System.Collections.Generic;

namespace FiresecClient
{
	public partial class SafeFiresecService
    {
		public OperationResult<IEnumerable<ReportDTO>> GetCardTemplateReportsForPrint(EmployeeFilter filter, Guid? selectedTemplate)
		{
			return SafeContext.Execute(() => FiresecService.GetCardTemplateReportsForPrint(filter, selectedTemplate));
		}
		public OperationResult<List<string>> GetAllReportNames()
		{
			return SafeContext.Execute(() => FiresecService.GetAllReportNames());
		}

		public OperationResult<bool> SaveReportFilter(SKDReportFilter filter, User user)
		{
			return  SafeContext.Execute(() => FiresecService.SaveReportFilter(filter, user));
		}

		public OperationResult<bool> RemoveReportFilter(SKDReportFilter filter, User user)
		{
			return SafeContext.Execute(() => FiresecService.RemoveReportFilter(filter, user));
		}

		public OperationResult<List<SKDReportFilter>> GetReportFiltersByType(User user, ReportType type)
		{
			return SafeContext.Execute(() => FiresecService.GetReportFiltersByType(user, type));
		}

		public OperationResult<List<SKDReportFilter>> GetReportFiltersForUser(User user)
		{
			return SafeContext.Execute(() => FiresecService.GetReportFiltersForUser(user));
		}

		public OperationResult<List<SKDReportFilter>> GetAllFilters()
		{
			return SafeContext.Execute(() => FiresecService.GetAllFilters());
		}
    }
}
