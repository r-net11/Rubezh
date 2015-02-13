using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;

namespace SKDModule.Reports.ViewModels
{
	public class PlacementMainPageViewModel : FilterContainerViewModel
	{
		private bool _useCurrentDate;
		public bool UseCurrentDate
		{
			get { return _useCurrentDate; }
			set
			{
				_useCurrentDate = value;
				OnPropertyChanged(() => UseCurrentDate);
			}
		}
		private DateTime _reportDateTime;
		public DateTime ReportDateTime
		{
			get { return _reportDateTime; }
			set
			{
				_reportDateTime = value;
				OnPropertyChanged(() => ReportDateTime);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var placementFilter = filter as EmployeeZonesReportFilter;
			if (placementFilter == null)
				return;
			UseCurrentDate = placementFilter.UseCurrentDate;
			ReportDateTime = placementFilter.ReportDateTime;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var placementFilter = filter as EmployeeZonesReportFilter;
			if (placementFilter == null)
				return;
			placementFilter.UseCurrentDate = UseCurrentDate;
			placementFilter.ReportDateTime = ReportDateTime;
		}
	}
}
