using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;

namespace ReportSystem
{
	public class MasterReportFactory
	{
		private XtraReport _masterReport; //мастер-отчёт, который генерируется автоматически
		private XtraReport _templateReport; //входной отчёт-шаблон
		private DataSet _dataSet;

		public MasterReportFactory(DataSet dataset, XtraReport inputReport)
		{
			_templateReport = inputReport;

		}

		private void CreateMasterReport()
		{
			foreach (var VARIABLE in _dataSet.Tables[0].Rows)
			{
				
			}
		}
	}
}
