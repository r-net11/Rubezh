using ResursAPI;
using System;

namespace Resurs.Reports
{
	public class ReportFilter
	{
		public ReportFilter()
		{
			StartDate = DateTime.Today;
			EndDate = DateTime.Today;
		}
		public DateTime StartDate {get; set;}
		public DateTime EndDate { get; set; }
		public Device Device { get; set; }

	}
}