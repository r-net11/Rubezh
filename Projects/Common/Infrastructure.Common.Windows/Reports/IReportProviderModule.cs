using System.Collections.Generic;

namespace Infrastructure.Common.Windows.Reports
{
	public interface IReportProviderModule
	{
		IEnumerable<IReportProvider> GetReportProviders();
	}
}
