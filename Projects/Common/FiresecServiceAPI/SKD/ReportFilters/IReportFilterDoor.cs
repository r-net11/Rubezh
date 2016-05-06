using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterDoor
	{
		List<Guid> Doors { get; set; }
	}
}