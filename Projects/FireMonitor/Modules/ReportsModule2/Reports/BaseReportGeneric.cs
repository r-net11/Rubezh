using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ReportsModule2.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        public BaseReportGeneric()
        {
            DataList = new List<T>();
			ReportDataTable = new DataTable();
        }

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }
		public DataTable ReportDataTable { get; protected set; }

		//public override void LoadCrystalReportDocument(ReportDocument reportDocument)
		//{
		//    reportDocument.FileName = FileHelper.GetReportFilePath(ReportFileName);
		//    reportDocument.SetDataSource(DataList);
		//}

		public void ListToDataTable()
		{
			DataTable dt = new DataTable();

			foreach (PropertyInfo info in typeof(T).GetProperties())
			{
				dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
			}

			foreach (T t in DataList)
			{
				DataRow row = dt.NewRow();
				foreach (PropertyInfo info in typeof(T).GetProperties())
				{
					row[info.Name] = info.GetValue(t, null);
				}
				dt.Rows.Add(row);
			}
			ReportDataTable = dt;
		}
		
    }
}

//public static DataTable ListToDataTable<D>(List<D> list)
//        {
//            DataTable dt = new DataTable();

//            foreach (PropertyInfo info in typeof(D).GetProperties())
//            {
//                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
//            }

//            foreach (D d in list)
//            {
//                DataRow row = dt.NewRow();
//                foreach (PropertyInfo info in typeof(D).GetProperties())
//                {
//                    row[info.Name] = info.GetValue(d, null);
//                }
//                dt.Rows.Add(row);
//            }
//            return dt;
//        }