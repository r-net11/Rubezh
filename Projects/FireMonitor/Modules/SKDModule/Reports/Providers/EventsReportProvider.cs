using Localization.SKD.Common;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using SKDModule.Reports.ViewModels;
using StrazhAPI.Models;

namespace SKDModule.Reports.Providers
{
	public class EventsReportProvider : FilteredSKDReportProvider<EventsReportFilter>
	{
		public EventsReportProvider()
			: base(CommonResources.EventsReport, 401, SKDReportGroup.Events, PermissionType.Oper_Reports_Events)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel
			{
				Columns = new Dictionary<string, string>
				{
					{ "SystemDateTime", CommonResources.SystemDate },
					{ "DeviceDateTime", CommonResources.DeviceDate },
					{ "Name", CommonResources.Name },
					{ "Object", CommonResources.Object },
					{ "User", CommonResources.User },
					{ "System", CommonResources.Subsystem },
				},
				Pages = new List<FilterContainerViewModel>
				{
					new EventTypePageViewModel(),
					new SKDObjectPageViewModel(),
					new EmployeePageViewModel(),
					new UserPageViewModel(),
				},
			};
		}
	}
}