using System.Collections.Generic;
using Localization.SKD.Common;
using StrazhAPI.Models;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using SKDModule.Reports.ViewModels;

namespace SKDModule.Reports.Providers
{
	public class DoorsReportProvider : FilteredSKDReportProvider<DoorsReportFilter>
	{
		public DoorsReportProvider()
			: base(CommonResources.DoorList, 431, SKDReportGroup.Configuration, PermissionType.Oper_Reports_Doors)
		{
		}

		public override FilterModel GetFilterModel()
		{
			return new FilterModel()
			{
				Columns = new Dictionary<string, string> 
				{ 
					{ "Number", CommonResources.DoorNumber },
					{ "Door", CommonResources.Name },
					{ "Controller", CommonResources.Controller },
					{ "IP", CommonResources.IPAddress },
					{ "Organisation", CommonResources.Organization },
					{ "EnterReader", CommonResources.Reader1 },
					{ "ExitReader", CommonResources.Reader2 },
					{ "EnterZone", CommonResources.Zone1 },
					{ "ExitZone", CommonResources.Zone2 },
					{ "Comment", CommonResources.Note },
				},
				Pages = new List<FilterContainerViewModel>()
				{
					new DoorPageViewModel(),
					new ZonePageViewModel(),
					new OrganizationPageViewModel(true),
				},
			};
		}
	}
}