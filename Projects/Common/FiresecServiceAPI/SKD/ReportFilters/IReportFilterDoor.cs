using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterDoor
	{
		List<Guid> Doors { get; set; }
	}
}