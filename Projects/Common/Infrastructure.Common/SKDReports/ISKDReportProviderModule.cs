using System.Collections.Generic;

namespace Infrastructure.Common.SKDReports
{
	public interface ISKDReportProviderModule
	{
		IEnumerable<ISKDReportProvider> GetSKDReportProviders();
	}
}