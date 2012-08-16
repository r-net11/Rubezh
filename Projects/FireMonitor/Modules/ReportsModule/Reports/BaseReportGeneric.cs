using System.Collections.Generic;
using Microsoft.Reporting.WinForms;

namespace ReportsModule.Reports
{
	public class BaseReportGeneric<T> : BaseReport
	{
		public BaseReportGeneric()
		{
			DataList = new List<T>();
		}

		public override ReportDataSource CreateDataSource()
		{
			return new ReportDataSource(DataSourceFileName, DataList);
		}

		protected List<T> _dataList;
		public List<T> DataList { get; protected set; }
	}
}