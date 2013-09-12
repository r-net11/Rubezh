using iTextSharp.text;
namespace Infrastructure.Common.Reports
{
	public interface IReportProvider
	{
		string Template { get; }
		string Title { get; }
		bool IsEnabled { get; }
		bool CanPdfPrint { get; }
		void PdfPrint(Document document);
	}
}
