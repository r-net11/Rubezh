using System;
using Infrastructure.Common.Windows.SKDReports;
using RubezhAPI.SKD.ReportFilters;

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
		public DateTime ReportDateTime
		{
			get { return Date.AddTicks(Time.Ticks); }
		}
		private DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged(() => Date);
			}
		}
		private TimeSpan _time;
		public TimeSpan Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged(() => Time);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var placementFilter = filter as EmployeeZonesReportFilter;
			if (placementFilter == null)
				return;
			UseCurrentDate = placementFilter.UseCurrentDate;
			Date = placementFilter.ReportDateTime.Date;
			Time = placementFilter.ReportDateTime.TimeOfDay;
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