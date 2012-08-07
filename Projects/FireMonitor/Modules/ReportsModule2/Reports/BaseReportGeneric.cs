using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Xps.Packaging;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace ReportsModule2.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        public BaseReportGeneric()
        {
            DataList = new List<T>();
        }

		public override ReportDataSource CreateDataSource()
		{
			return new ReportDataSource(ReportFileName , DataList);
		}

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }
    }
}
