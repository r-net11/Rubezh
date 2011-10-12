using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Common;
using Microsoft.Reporting.WinForms;
using System.IO;
using System.Windows.Markup;
using FiresecClient;

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

        public static ReportViewer CreateReportViewer<T>(List<T> dataList, string dataListName, string rdlcFileName)
        {
            if (dataList.IsNotNullOrEmpty() == false)
            {
                return new ReportViewer();
            }
            var reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            var filePath = FileHelper.GetReportFilePath(rdlcFileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                reportViewer.LocalReport.LoadReportDefinition(fs);
            }
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataListName, dataList));
            return reportViewer;
        }
    }
}
