using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System;

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

		private DateTime _reportDate;

		public DateTime ReportDate
		{
			get { return _reportDate; }
			set
			{
				if (_reportDate == value) return;
				_reportDate = value;
				OnPropertyChanged(() => ReportDate);
			}
		}

		private TimeSpan _reportTime;

		public TimeSpan ReportTime
		{
			get { return _reportTime; }
			set
			{
				if (_reportTime == value) return;
				_reportTime = value;
				OnPropertyChanged(() => ReportTime);
			}
		}


		public override void LoadFilter(SKDReportFilter filter)
		{
			var placementFilter = filter as EmployeeZonesReportFilter;
			if (placementFilter == null)
				return;
			UseCurrentDate = placementFilter.UseCurrentDate;
			ReportDate = placementFilter.ReportDateTime.GetValueOrDefault().Date;
			ReportTime = placementFilter.ReportDateTime.GetValueOrDefault().TimeOfDay;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var placementFilter = filter as EmployeeZonesReportFilter;
			if (placementFilter == null)
				return;
			placementFilter.UseCurrentDate = UseCurrentDate;
			placementFilter.ReportDateTime = ReportDate + ReportTime;
		}
	}
}
