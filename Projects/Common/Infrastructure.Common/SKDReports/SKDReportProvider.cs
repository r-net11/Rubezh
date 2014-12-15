using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace Infrastructure.Common.SKDReports
{
	public class SKDReportProvider : ISKDReportProvider
	{
		public SKDReportProvider(string name, string title, int index, SKDReportGroup? group = null)
		{
			Name = name;
			Title = title;
			Index = index;
			Group = group;
		}

		#region ISKDReportProvider Members

		public string Name { get; set; }
		public string Title { get; set; }
		public PermissionType? Permission { get; set; }
		public int Index { get; set; }
		public string IconSource { get; set; }
		public SKDReportGroup? Group { get; set; }

		#endregion
	}
}
