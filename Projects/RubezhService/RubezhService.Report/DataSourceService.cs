using System.ComponentModel.Composition;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.UI;

namespace RubezhService.Report
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
