using System.Collections.Generic;
using Common;
using FiresecClient;
using SAPBusinessObjects.WPF.Viewer;
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
            crystalReportsViewer.ShowLogo = false;
            crystalReportsViewer.ShowToggleSidePanelButton = false;
            crystalReportsViewer.ToggleSidePanel = SAPBusinessObjects.WPF.Viewer.Constants.SidePanelKind.None;
            return crystalReportsViewer;
        }
    }
}
