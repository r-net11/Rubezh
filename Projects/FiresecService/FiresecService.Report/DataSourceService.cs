using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Composition;

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

		#endregion IDataSourceService Members
	}
}