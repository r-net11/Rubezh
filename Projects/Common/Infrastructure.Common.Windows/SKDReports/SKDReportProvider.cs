using RubezhAPI.Models;

namespace Infrastructure.Common.SKDReports
{
	public class SKDReportProvider : ISKDReportProvider
	{
		public SKDReportProvider(string title, int index, SKDReportGroup? group = null, PermissionType? permission = null)
		{
			Title = title;
			Index = index;
			Group = group;
			Permission = permission;
		}

		#region ISKDReportProvider Members

		public string Title { get; private set; }
		public PermissionType? Permission { get; private set; }
		public int Index { get; private set; }
		public string IconSource { get; private set; }
		public SKDReportGroup? Group { get; private set; }

		#endregion
	}
}