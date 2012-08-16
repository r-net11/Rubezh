using FiresecClient;
using Microsoft.Reporting.WinForms;
namespace ReportsModule.Reports
{
	public class BaseReport
	{
		public string ReportFileName { get; protected set; }
		public string DataSourceFileName { get; protected set; }

		public virtual void LoadData()
		{
		}

		public virtual ReportDataSource CreateDataSource()
		{
			return new ReportDataSource();
		}

		public virtual void LoadReportViewer(ReportViewer reportViewer)
		{
			reportViewer.LocalReport.ReportPath = FileHelper.GetReportFilePath(ReportFileName);
			reportViewer.LocalReport.DataSources.Add(CreateDataSource());
		}
	}
}