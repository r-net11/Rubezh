using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterZone
	{
		List<Guid> Zones { get; set; }
	}
}