using RubezhAPI.Models;

namespace Infrastructure.Common.Windows.SKDReports
{
	public interface ISKDReportProvider
	{
		string Title { get; }
		PermissionType? Permission { get; }
		int Index { get; }
		string IconSource { get; }
		SKDReportGroup? Group { get; }
	}
}