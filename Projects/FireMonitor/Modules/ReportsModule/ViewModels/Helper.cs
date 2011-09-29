using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Reflection;
using Microsoft.Reporting.WinForms;
using Common;

namespace ReportsModule
{
    public static class Helper
    {
        public static DataTable CreateDataTable<T>(string name, List<T> dataList)
        {
            using (var dataTable = new DataTable(name))
            {
                FillDataTable<T>(dataList, dataTable);
                return dataTable;
            }
        }

        static void FillDataTable<T>(List<T> dataList, DataTable dataTable)
        {
            var propertyInfos = typeof(T).GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                dataTable.Columns.Add();
            }
            foreach (var dataClass in dataList)
            {

                dataTable.Rows.Add(CreateParamsString<T>(dataClass, propertyInfos));
            }
        }

        static object[] CreateParamsString<T>(T dataClass, PropertyInfo[] propertyInfos)
        {
            object[] obj = new object[propertyInfos.Length];
            int i = 0;
            foreach (var propertyInfo in propertyInfos)
            {
                obj[i] = propertyInfo.GetValue(dataClass, null);
                i++;
            }
            return obj;
        }

        public static ReportViewer CreateReportViewer<T>(List<T> dataList,string dataListName, string rdlcFileName)
        {
            if (dataList.IsNotNullOrEmpty() == false)
            {
                return null;
            }
            var reportViewer = new ReportViewer();
            //var startDate = new ReportParameter("StartDate",reportJournalDataTable.StartDate.ToString(),true);
            //var endDate = new ReportParameter("EndDate", reportJournalDataTable.EndDate.ToString(),true);
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = rdlcFileName;
            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { startDate2 });
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataListName, dataList));
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Start", "abc"));
            reportViewer.RefreshReport();
            return reportViewer;
        }
    }
}
