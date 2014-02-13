using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKDModule.Models
{
	public class EmployeeReportFilter
	{
		public EmployeeReportFilter()
		{
			EmployeeReportType = EmployeeReportType.Время_присутствия;
			StartDateTime = DateTime.Now.AddDays(-1);
			EndDateTime = DateTime.Now;
		}

		public EmployeeReportType EmployeeReportType { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
	}
}