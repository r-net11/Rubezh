using System.Data;
using RubezhAPI.SKD.ReportFilters;

namespace FiresecService.Report.Reports
{
	public class EmptyReport : BaseReport<DataSet>
	{
		public override DataSet CreateDataSet(DataProvider dataProvider, SKDReportFilter filter)
		{
			return new DataSet();
		}
	}
}
