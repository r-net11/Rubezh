using System.Collections.Generic;
using CodeReason.Reports;

namespace Infrastructure.Common.Windows.Reports
{
	public interface IMultiReportProvider : IReportProvider
	{
		IEnumerable<ReportData> GetData();
	}
}
