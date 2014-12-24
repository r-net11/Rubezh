using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterPosition
	{
		List<Guid> Positions { get; set; }
	}
}
