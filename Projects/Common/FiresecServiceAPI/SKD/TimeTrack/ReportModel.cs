using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD
{
	public class ReportModel
	{
		public ReportModel()
		{
			EmployeeRepors = new List<EmployeeReportModel>();
		}

		public List<EmployeeReportModel> EmployeeRepors { get; set; }
		public string Name1 { get; set; }
		public string Name2 { get; set; }
		public string Name3 { get; set; }
		public DateTime CreationDateTime { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }

	}

	public class EmployeeReportModel
	{
		public EmployeeReportModel()
		{
			Days = new List<EmployeeReportModelDay>();
			MissReasons = new List<EmployeeReportModelMissed>();
		}

		public List<EmployeeReportModelDay> Days { get; set; }
		public List<EmployeeReportModelMissed> MissReasons { get; set; }
		public int No { get; set; }
		public string EmploueeFIO { get; set; }
		public int TabelNo { get; set; }
		public int FirstHalfDaysCount { get; set; }
		public TimeSpan FirstHalfTimeSpan { get; set; }
		public int SecondHalfDaysCount { get; set; }
		public TimeSpan SecondHalfTimeSpan { get; set; }

	}

	public class EmployeeReportModelDay
	{
		public EmployeeReportModelDay()
		{

		}

		public string Code { get; set; } // Can contain up to 3 codes
		public TimeSpan TimeSpan { get; set; } // Can contain up to 3 timeSpans
	}

	public class EmployeeReportModelMissed
	{
		public EmployeeReportModelMissed()
		{

		}

		public string Code { get; set; }
		public TimeSpan TimeSpan { get; set; }
	}
}