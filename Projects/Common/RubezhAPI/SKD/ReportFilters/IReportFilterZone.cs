using System;
using System.Collections.Generic;

namespace RubezhAPI.SKD.ReportFilters
{
	public interface IReportFilterZone
	{
		List<Guid> Zones { get; set; }
	}
}