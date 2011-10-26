using Microsoft.Reporting.WinForms;
using SAPBusinessObjects.WPF.Viewer;

namespace ReportsModule.Reports
{
    public class BaseReport
    {
        public virtual void LoadData(){ }

        public virtual CrystalReportsViewer CreateCrystalReportViewer()
        {
            return new CrystalReportsViewer();
        }
    }
}
