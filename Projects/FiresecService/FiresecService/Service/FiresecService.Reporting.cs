using System;
using FiresecService.Report.Helpers;
using FiresecService.Report.Services;
using ReportSystem.Api.DTO;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using StrazhDAL;
using System.Collections.Generic;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<IEnumerable<ReportDTO>> GetCardTemplateReportsForPrint(EmployeeFilter filter, Guid? selectedTemplate)
		{
			using (var db = new SKDDatabaseService())
			{
				var service = new SearchReportService(filter, selectedTemplate, db);
				service.Execute();
				return new OperationResult<IEnumerable<ReportDTO>>(service.SearchResult);
			}
		}

		public OperationResult<List<string>> GetAllReportNames()
		{
			return new OperationResult<List<string>>(ReportingHelpers.GetReportNames());
		}

		public OperationResult<bool> SaveReportFilter(SKDReportFilter filter, User user)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.ReportFiltersTranslator.Save(filter, user);
			}
		}

		public OperationResult<bool> RemoveReportFilter(SKDReportFilter filter, User user)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.ReportFiltersTranslator.Remove(filter, user);
			}
		}

		public OperationResult<List<SKDReportFilter>> GetReportFiltersByType(User user, ReportType type)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.ReportFiltersTranslator.GetReportFiltersByType(user, type);
			}
		}

		public OperationResult<List<SKDReportFilter>> GetReportFiltersForUser(User user)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.ReportFiltersTranslator.GetForUser(user);
			}
		}

		public OperationResult<List<SKDReportFilter>> GetAllFilters()
		{
			using (var db = new SKDDatabaseService())
			{
				return db.ReportFiltersTranslator.GetAllFilters();
			}
		}
	}
}
