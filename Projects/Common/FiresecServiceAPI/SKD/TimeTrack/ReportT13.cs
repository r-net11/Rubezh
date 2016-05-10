using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD
{
	public class ReportT13
	{
		public ReportT13()
		{
			EmployeeRepors = new List<EmployeeReport>();
		}

		public List<EmployeeReport> EmployeeRepors { get; set; }

		public string FillPosition { get; set; }

		public string LeadPosition { get; set; }

		public string HRPosition { get; set; }

		public string FillName { get; set; }

		public string LeadName { get; set; }

		public string HRName { get; set; }

		public string OrganisationName { get; set; }

		public string DepartmentName { get; set; }

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

		public string EmployeeFIO { get; set; }

		public Guid DepartmenuUID { get; set; }

		public string DepartmentName { get; set; }

		public string TabelNo { get; set; }

		public int FirstHalfDaysCount { get; set; }

		public int SecondHalfDaysCount { get; set; }

		public int TotalDaysCount { get; set; }

		public TimeSpan FirstHalfTimeSpan { get; set; }

		public TimeSpan SecondHalfTimeSpan { get; set; }

		public TimeSpan TotalTimeSpan { get; set; }
	}

	public class EmployeeReportDay
	{
		public string CodeStrings { get; set; }

		public string TimeSpanStrings { get; set; }
	}

	public class EmployeeReportMissReason
	{
		public string Code { get; set; }

		public TimeSpan TimeSpan { get; set; }
	}
}