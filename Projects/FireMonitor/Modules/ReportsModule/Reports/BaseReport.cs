using SAPBusinessObjects.WPF.Viewer;
using CrystalDecisions.CrystalReports.Engine;

namespace ReportsModule.Reports
{
    public class BaseReport
    {
        public virtual void LoadData(){ }

        public virtual ReportDocument CreateCrystalReportDocument()
        {
            return new ReportDocument();
        }

    }
}
