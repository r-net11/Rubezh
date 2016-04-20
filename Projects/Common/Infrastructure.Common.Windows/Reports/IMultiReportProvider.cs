using System.Collections.Generic;
using CodeReason.Reports;

namespace Infrastructure.Common.Reports
{
	public interface IMultiReportProvider : IReportProvider
	{
		IEnumerable<ReportData> GetData();
	}
}
