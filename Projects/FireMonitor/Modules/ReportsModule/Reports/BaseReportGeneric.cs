using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using FiresecClient;
using System.Data;
using System.Reflection;

namespace ReportsModule.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        public BaseReportGeneric()
        {
            DataList = new List<T>();
        }

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }

		public override void LoadCrystalReportDocument(ReportDocument reportDocument)
        {
			reportDocument.FileName = FileHelper.GetReportFilePath(ReportFileName);
            reportDocument.SetDataSource(DataList);
        }

		public static DataTable ListToDataTable<T>(List<T> list)
		{
			DataTable dt = new DataTable();

			foreach (PropertyInfo info in typeof(T).GetProperties())
			{
				dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
			}
			foreach (T t in list)
			{
				DataRow row = dt.NewRow();
				foreach (PropertyInfo info in typeof(T).GetProperties())
				{
					row[info.Name] = info.GetValue(t, null);
				}
				dt.Rows.Add(row);
			}
			return dt;
		}
    }
}