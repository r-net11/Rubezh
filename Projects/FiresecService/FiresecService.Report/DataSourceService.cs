using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Service.Extensions;
using System.ComponentModel.Composition;
using DevExpress.XtraReports.UI;

namespace FiresecService.Report
{
	[Export(typeof(IDataSourceService))]
	public class DataSourceService : IDataSourceService
	{
		#region IDataSourceService Members

		public void FillDataSources(XtraReport report, string reportName, bool isDesignSessionActive)
		{
		}

		public void RegisterDataSources(XtraReport report, string reportName)
		{
		}

		#endregion
	}
}
