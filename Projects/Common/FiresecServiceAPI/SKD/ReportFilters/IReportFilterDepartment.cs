using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterDepartment
	{
		List<Guid> Departments { get; set; }
	}
}