using Infrastructure.Common.SKDReports;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common.Reports
{
	public class NewReportProviderEvent : CompositePresentationEvent<ISKDReportProvider>
	{
	}
}
