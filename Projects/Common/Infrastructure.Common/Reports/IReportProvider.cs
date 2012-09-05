using FiresecAPI.Models;

namespace Infrastructure.Common.Reports
{
	public interface IReportProvider
	{
		string Template { get; }
		string Title { get; }
		bool IsEnabled { get; }
	}
}
