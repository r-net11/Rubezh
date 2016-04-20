using CodeReason.Reports;
using Common;
using Controls.PDF;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Resources;

namespace ReportsModule.ViewModels
{
	public class ReportViewModel : BaseViewModel
	{
		private readonly IReportProvider _reportProvider;

		public ReportViewModel(IReportProvider reportProvider)
		{
			_reportProvider = reportProvider;
			IsActive = false;
		}

		public string Title
		{
			get { return _reportProvider.Title; }
		}
		public bool IsFilterable
		{
			get { return _reportProvider is IFilterableReport; }
		}
		public bool IsEnabled
		{
			get { return _reportProvider.IsEnabled; }
		}
		public bool IsActive { get; set; }

		public DocumentPaginator GenerateReport()
		{
			using (new TimeCounter("Build report: {0}"))
			{
				ReportDocument reportDocument = new ReportDocument();
				reportDocument.XamlData = GetXaml();
				DocumentPaginator documentPaginator = GetPaginator(reportDocument);
				return documentPaginator;
			}
		}

		public void Filter(RelayCommand refreshCommand)
		{
			((IFilterableReport)_reportProvider).Filter(refreshCommand);
		}
		public void PreparePrinting(PrintTicket printTicket, Size pageSize)
		{
			printTicket.PageOrientation = pageSize.Height >= pageSize.Width ? PageOrientation.Portrait : PageOrientation.Landscape;
			var printExtension = _reportProvider as IReportPrintExtension;
			if (printExtension != null)
				printExtension.PreparePrinting(printTicket, pageSize);
		}

		private DocumentPaginator GetPaginator(ReportDocument reportDocument)
		{
			var singleReport = _reportProvider as ISingleReportProvider;
			if (singleReport != null)
				return new ReportPaginator(reportDocument, singleReport.GetData());
			var multiReport = _reportProvider as IMultiReportProvider;
			if (multiReport != null)
				return new MultipleReportPaginator(reportDocument, multiReport.GetData());
			return null;
		}
		private string GetXaml()
		{
			StreamResourceInfo info = Application.GetResourceStream(ResourceService.ComposeResourceUri(_reportProvider.GetType().Assembly, _reportProvider.Template));
			using (var reader = new StreamReader(info.Stream))
				return reader.ReadToEnd();
		}

		public bool CanPdfPrint
		{
			get { return _reportProvider != null && _reportProvider.PdfProvider != null && _reportProvider.PdfProvider.CanPrint; }
		}
		public void PdfPrint()
		{
			var fileName = PDFHelper.ShowSavePdfDialog();
			if (!string.IsNullOrEmpty(fileName))
				WaitHelper.Execute(() =>
				{
					using (var fs = new FileStream(fileName, FileMode.Create))
					using (var pdf = new PDFDocument(fs, _reportProvider.PdfProvider.PageFormat))
					{
						AddReportHeader(pdf.Document);
						_reportProvider.PdfProvider.Print(pdf.Document);
#if DEBUG
						Process.Start(fileName);
#endif
					}
				});
		}
		private void AddReportHeader(iTextSharp.text.Document document)
		{
			document.AddAuthor("Рубеж / Оперативные задачи");
			document.AddTitle(_reportProvider.Title);
			document.AddCreator("ОЗ");
		}
	}
}