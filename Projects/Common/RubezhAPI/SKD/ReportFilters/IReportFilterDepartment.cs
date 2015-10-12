using System;
using System.Collections.Generic;

namespace RubezhAPI.SKD.ReportFilters
{
	public interface IReportFilterDepartment
	{
		List<Guid> Departments { get; set; }
	}
}
