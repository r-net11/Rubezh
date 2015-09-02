using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD.ReportFilters
{
	public class ReflectionReportFilter: SKDReportFilter, IReportFilterReflection
	{
		public Guid Mirror { get; set; }
	}
}
