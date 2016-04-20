using RubezhAPI.SKD.ReportFilters;
using GKModule.ViewModels;
using Infrastructure.Common.SKDReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI.Models;

namespace GKModule.Reports.Providers
{
	public class ReflectionReportProvider : FilteredSKDReportProvider<ReflectionReportFilter>
	{
		public ReflectionReportProvider()
			: base("Список отражений", 432, SKDReportGroup.Configuration, PermissionType.Oper_Reports_Mirror)
		{
		}

		public override FilterModel InitializeFilterModel()
		{
			return new FilterModel()
			{
				AllowSort = false,
				Pages = new List<FilterContainerViewModel>()
				{
					new ReflectionPageViewModel()
				},
			};
		}
	}
}