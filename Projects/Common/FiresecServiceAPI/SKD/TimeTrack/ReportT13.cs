using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD
{
	public class ReportT13
	{
		public ReportT13()
		{
			EmployeeRepors = new List<EmployeeReport>();
		}

		public List<EmployeeReport> EmployeeRepors { get; set; }
		public string FillName { get; set; }
		public string LeadName { get; set; }
		public string HRName { get; set; }
		public DateTime CreationDateTime { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }

	}

	public class EmployeeReport
	{
		public EmployeeReport()
		{
			Days = new List<EmployeeReportDay>();
			MissReasons = new List<EmployeeReportMissReason>();
		}

		public List<EmployeeReportDay> Days { get; set; }
		public List<EmployeeReportMissReason> MissReasons { get; set; }
		public int No { get; set; }
		public string EmploueeFIO { get; set; }
		public int TabelNo { get; set; }

		public int FirstHalfDaysCount { get; set; }
		public int SecondHalfDaysCount { get; set; }
		public int TotalDaysCount { get; set; }

		public TimeSpan FirstHalfTimeSpan { get; set; }
		public TimeSpan SecondHalfTimeSpan { get; set; }
		public TimeSpan TotalTimeSpan { get; set; }

	}

	public class EmployeeReportDay
	{
		public string Code { get; set; } // Can contain up to 3 codes
		public TimeSpan TimeSpan { get; set; } // Can contain up to 3 timeSpans
	}

	public class EmployeeReportMissReason
	{
		public string Code { get; set; }
		public TimeSpan TimeSpan { get; set; }
	}
}