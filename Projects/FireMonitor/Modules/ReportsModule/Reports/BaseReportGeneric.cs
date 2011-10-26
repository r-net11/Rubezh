using System.Collections.Generic;
using System.IO;
using Common;
using FiresecClient;
using Microsoft.Reporting.WinForms;
using SAPBusinessObjects.WPF.Viewer;
using ReportsModule.CrystalReports;
using CrystalDecisions.CrystalReports.Engine;

namespace ReportsModule.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        protected string ReportFileName;
        protected ReportDocument reportDocument;

        public BaseReportGeneric()
        {
            DataList = new List<T>();
            reportDocument = new ReportDocument();
        }

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }

        public override CrystalReportsViewer CreateCrystalReportViewer()
        {
            if (DataList.IsNotNullOrEmpty() == false)
            {
                return new CrystalReportsViewer();
            }

            var filePath = FileHelper.GetReportFilePath(ReportFileName);
            reportDocument.Load(filePath);
            reportDocument.SetDataSource(DataList);
            var crystalReportsViewer = new CrystalReportsViewer();
            crystalReportsViewer.ViewerCore.ReportSource = reportDocument;
            return crystalReportsViewer;
        }
    }
}
