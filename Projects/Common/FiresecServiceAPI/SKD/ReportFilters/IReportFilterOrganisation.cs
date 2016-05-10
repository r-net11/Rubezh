using System;
using System.Collections.Generic;

namespace StrazhAPI.SKD.ReportFilters
{
	public interface IReportFilterOrganisation
	{
		List<Guid> Organisations { get; set; }
	}
}