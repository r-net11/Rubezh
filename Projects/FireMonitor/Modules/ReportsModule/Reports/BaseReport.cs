using Microsoft.Reporting.WinForms;

namespace ReportsModule.Reports
{
    public class BaseReport
    {
        public virtual void LoadData()
        {

        }

        public virtual ReportViewer CreateReportViewer()
        {
            return new ReportViewer();
        }
    }
}
