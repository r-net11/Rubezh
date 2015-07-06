using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterDepartment
	{
		List<Guid> Departments { get; set; }
	}
}