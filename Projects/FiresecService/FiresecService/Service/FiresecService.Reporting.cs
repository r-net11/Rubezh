using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Enums;
using FiresecAPI.Models;
using FiresecAPI.SKD.ReportFilters;
using FiresecService.Report.Helpers;
using FiresecService.Report.Templates;
using SKDDriver;
using System.Collections.Generic;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
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
