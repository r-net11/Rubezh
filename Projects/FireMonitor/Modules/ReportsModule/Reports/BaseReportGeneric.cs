using System.Collections.Generic;
using System.IO;
using Common;
using FiresecClient;
using Microsoft.Reporting.WinForms;

namespace ReportsModule.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        public string RdlcFileName;
        public string DataTableName;

        public BaseReportGeneric()
        {
            DataList = new List<T>();
        }

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }

        public override void LoadData()
        {

        }

        public override ReportViewer CreateReportViewer()
        {
            if (DataList.IsNotNullOrEmpty() == false)
            {
                return new ReportViewer();
            }

            var reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            var filePath = FileHelper.GetReportFilePath(RdlcFileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                reportViewer.LocalReport.LoadReportDefinition(fs);
            }
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(DataTableName, DataList));

            return reportViewer;
        }
        
    }
}
