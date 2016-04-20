using System.Collections.Generic;

namespace Infrastructure.Common.Windows.SKDReports
{
	public interface ISKDReportProviderModule
	{
		IEnumerable<ISKDReportProvider> GetSKDReportProviders();
	}
}