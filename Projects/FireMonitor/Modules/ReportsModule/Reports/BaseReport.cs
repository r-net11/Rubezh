using CrystalDecisions.CrystalReports.Engine;

namespace ReportsModule.Reports
{
	public class BaseReport
	{
		public string ReportFileName { get; protected set; }

		public virtual void LoadData()
		{
		}

		public virtual void LoadCrystalReportDocument(ReportDocument reportDocument)
		{
		}
	}
}