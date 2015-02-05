using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterEmployeeAndVisitor : IReportFilterEmployee
	{
		bool IsEmployee { get; set; }
	}
}
