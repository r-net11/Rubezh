using CodeReason.Reports;
using iTextSharp.text;

namespace Infrastructure.Common.Windows.Reports
{
	public interface IReportPdfProvider
	{
		Rectangle PageFormat { get; }
		ReportData ReportData { get; set; }
		bool CanPrint { get; }
		void Print(Document document);
	}
}
