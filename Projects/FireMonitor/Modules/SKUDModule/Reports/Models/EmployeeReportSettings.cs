using System;

namespace SKDModule.Models
{
	public class EmployeeReportSettings
	{
		public EmployeeReportSettings()
		{
			EmployeeReportType = EmployeeReportType.Время_присутствия;
			EmployeeReportPeriod = EmployeeReportPeriod.Day;
			StartDateTime = DateTime.Today;
			EndDateTime = DateTime.Today;
		}

		public EmployeeReportType EmployeeReportType { get; set; }
		public EmployeeReportPeriod EmployeeReportPeriod { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
	}
}