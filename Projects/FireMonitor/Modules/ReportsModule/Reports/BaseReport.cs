using System.Text;
using System.Windows.Xps.Packaging;
using System.IO;
using Microsoft.Reporting.WinForms;
using System.Collections;
using FiresecClient;
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