using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterZone
	{
		List<Guid> Zones { get; set; }
	}
}