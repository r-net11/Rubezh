using System.Collections.Generic;

namespace Infrastructure.Common.Reports
{
	public interface IReportProviderModule
	{
		IEnumerable<IReportProvider> GetReportProviders();
	}
}
