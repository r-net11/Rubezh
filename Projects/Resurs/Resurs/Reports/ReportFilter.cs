using ResursAPI;
using System;

namespace Resurs.Reports
{
	public class ReportFilter
	{
		public DateTime StartDate {get; set;}
		public DateTime EndDate { get; set; }
		public Device Device { get; set; }
	}
}