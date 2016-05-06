using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterPosition
	{
		List<Guid> Positions { get; set; }
	}
}