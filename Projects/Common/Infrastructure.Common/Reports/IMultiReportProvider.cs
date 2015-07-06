using CodeReason.Reports;
using System.Collections.Generic;

namespace Infrastructure.Common.Reports
{
	public interface IMultiReportProvider : IReportProvider
	{
		IEnumerable<ReportData> GetData();
	}
}