using System;
using System.Collections.Generic;

namespace FiresecAPI.SKD.ReportFilters
{
	public interface IReportFilterOrganisation
	{
		List<Guid> Organisations { get; set; }
	}
}