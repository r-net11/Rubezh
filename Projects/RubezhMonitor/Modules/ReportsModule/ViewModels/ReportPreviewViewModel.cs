using CodeReason.Reports;
using Common;
using Controls.PDF;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Resources;
using System.Windows.Xps;

namespace ReportsModule.ViewModels
{
	public class ReportPreviewViewModel : SaveCancelDialogViewModel
	{
		private IReportProvider _reportProvider;

		public ReportPreviewViewModel(IReportProvider provider)
		{
			_reportProvider = provider;
			Title = provider.Title;
			SaveCaption = "Печать";
			CancelCaption = "Отмена";
			PrintCommand = new RelayCommand(OnPrint);
			PdfPrintCommand = new RelayCommand(OnPdfPrint, CanPdfPrint);
			FirstPageCommand = new RelayCommand(OnFirstPage, CanFirstPage);
			NextPageCommand = new RelayCommand(OnNextPage, CanNextPage);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, CanPreviousPage);
			LastPageCommand = new RelayCommand(OnLastPage, CanLastPage);
			FitToWidthCommand = new RelayCommand<double>(OnFitToWidth);
			FitlToHeightCommand = new RelayCommand<double>(OnFitlToHeight);
			FitToPageCommand = new RelayCommand(OnFitToPage);
			ZoomInCommand = new RelayCommand(OnZoomIn, CanZoomIn);
			ZoomOutCommand = new RelayCommand(OnZoomOut, CanZoomOut);
			GenerateReport();
		}

		protected override bool Save()
		{
			return Print();
		}

		public bool Print()
		{
			var printDialog = new PrintDialog();
			if (printDialog.ShowDialog() == true)
			{
				PreparePrinting(printDialog.PrintTicket, DocumentPaginator.PageSize);
				XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
				if (writer != null)
					writer.WriteAsync(DocumentPaginator, printDialog.PrintTicket);
				return true;
			}
			return false;
		}

		private DocumentPaginator _documentPaginator;
		public DocumentPaginator DocumentPaginator
		{
			get { return _documentPaginator; }
			private set
			{
				if (DocumentPaginator != null)
				{
					DocumentPaginator.GetPageCompleted -= new GetPageCompletedEventHandler(DocumentPaginator_GetPageCompleted);
					DocumentPaginator.ComputePageCountCompleted -= new AsyncCompletedEventHandler(DocumentPaginator_ComputePageCountCompleted);
					DocumentPaginator.PagesChanged -= new PagesChangedEventHandler(DocumentPaginator_PagesChanged);
				}
				_documentPaginator = value;
				if (DocumentPaginator != null)
				{
					DocumentPaginator.GetPageCompleted += new GetPageCompletedEventHandler(DocumentPaginator_GetPageCompleted);
					DocumentPaginator.ComputePageCountCompleted += new AsyncCompletedEventHandler(DocumentPaginator_ComputePageCountCompleted);
					DocumentPaginator.PagesChanged += new PagesChangedEventHandler(DocumentPaginator_PagesChanged);
				}
				PageNumber = 0;
				Scale = 1;
				OnPropertyChanged(() => DocumentWidth);
				OnPropertyChanged(() => DocumentHeight);
				OnPropertyChanged(() => DocumentPaginator);
				OnPropertyChanged(() => TotalPageNumber);
			}
		}

		private void GenerateReport()
		{
			using (new TimeCounter("Build report: {0}"))
			using (new WaitWrapper())
			{
				var reportDocument = new ReportDocument();
				using (new TimeCounter("\tGet template: {0}"))
					reportDocument.XamlData = GetXaml();
				using (new TimeCounter("\tGet paginator: {0}"))
					DocumentPaginator = GetPaginator(reportDocument);
				PageNumber = 0;
			}
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
		private void PreparePrinting(PrintTicket printTicket, Size pageSize)
		{
			printTicket.PageOrientation = pageSize.Height >= pageSize.Width ? PageOrientation.Portrait : PageOrientation.Landscape;
			var printExtension = _reportProvider as IReportPrintExtension;
			if (printExtension != null)
				printExtension.PreparePrinting(printTicket, pageSize);
		}

		private void DocumentPaginator_GetPageCompleted(object sender, GetPageCompletedEventArgs e)
		{
			OnPropertyChanged(() => TotalPageNumber);
		}
		private void DocumentPaginator_PagesChanged(object sender, PagesChangedEventArgs e)
		{
			OnPropertyChanged(() => TotalPageNumber);
		}
		private void DocumentPaginator_ComputePageCountCompleted(object sender, AsyncCompletedEventArgs e)
		{
			OnPropertyChanged(() => TotalPageNumber);
			PageNumber = 0;
		}

		public RelayCommand PrintCommand { get; private set; }
		private void OnPrint()
		{
			Print();
		}
		public RelayCommand PdfPrintCommand { get; private set; }
		private void OnPdfPrint()
		{
			var fileName = PDFHelper.ShowSavePdfDialog();
			if (!string.IsNullOrEmpty(fileName))
				WaitHelper.Execute(() =>
				{
					using (var fs = new FileStream(fileName, FileMode.Create))
					using (var pdf = new PDFDocument(fs, _reportProvider.PdfProvider.PageFormat))
					{
						pdf.Document.AddAuthor("Рубеж / Оперативные задачи");
						pdf.Document.AddTitle(_reportProvider.Title);
						pdf.Document.AddCreator("ОЗ");
						_reportProvider.PdfProvider.Print(pdf.Document);
#if DEBUG
						Process.Start(fileName);
#endif
					}
				});
		}
		private bool CanPdfPrint()
		{
			return DocumentPaginator.PageCount > 0 && _reportProvider.PdfProvider != null && _reportProvider.PdfProvider.CanPrint;
		}
		public RelayCommand FirstPageCommand { get; private set; }
		private void OnFirstPage()
		{
			CurrentPageNumber = 1;
		}
		private bool CanFirstPage()
		{
			return CurrentPageNumber > 1;
		}
		public RelayCommand PreviousPageCommand { get; private set; }
		private void OnPreviousPage()
		{
			CurrentPageNumber--;
		}
		private bool CanPreviousPage()
		{
			return CurrentPageNumber > 1;
		}
		public RelayCommand NextPageCommand { get; private set; }
		private void OnNextPage()
		{
			CurrentPageNumber++;
		}
		private bool CanNextPage()
		{
			return CurrentPageNumber < TotalPageNumber;
		}
		public RelayCommand LastPageCommand { get; private set; }
		private void OnLastPage()
		{
			CurrentPageNumber = TotalPageNumber;
		}
		private bool CanLastPage()
		{
			return CurrentPageNumber < TotalPageNumber;
		}
		public RelayCommand<double> FitToWidthCommand { get; private set; }
		private void OnFitToWidth(double width)
		{
			Scale = (width - 4 * PageBorderThickness) / DocumentWidth;
		}
		public RelayCommand<double> FitlToHeightCommand { get; private set; }
		private void OnFitlToHeight(double height)
		{
			Scale = (height - 4 * PageBorderThickness) / DocumentHeight;
		}
		public RelayCommand FitToPageCommand { get; private set; }
		private void OnFitToPage()
		{
			Scale = 1;
		}
		public RelayCommand ZoomInCommand { get; private set; }
		private void OnZoomIn()
		{
			Scale += 0.1;
		}
		private bool CanZoomIn()
		{
			return Scale < MaxScale;
		}
		public RelayCommand ZoomOutCommand { get; private set; }
		private void OnZoomOut()
		{
			Scale -= 0.1;
		}
		private bool CanZoomOut()
		{
			return Scale > MinScale;
		}

		public double DocumentWidth
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Width; }
		}
		public double DocumentHeight
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Height; }
		}
		public bool IsPdfAllowed
		{
			get { return DocumentPaginator.PageCount > 0 && _reportProvider.PdfProvider != null && _reportProvider.PdfProvider.CanPrint; }
		}
		public int TotalPageNumber
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageCount; }
		}
		private int _pageNumber;
		public int PageNumber
		{
			get { return _pageNumber; }
			set
			{
				_pageNumber = value;
				OnPropertyChanged(() => PageNumber);
				OnPropertyChanged(() => CurrentPageNumber);
				CommandManager.InvalidateRequerySuggested();
			}
		}
		public int CurrentPageNumber
		{
			get { return PageNumber + 1; }
			set { PageNumber = value - 1; }
		}
		public double PageBorderThickness
		{
			get { return 2.0 / Scale; }
		}
		private double _scale;
		public double Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;
				OnPropertyChanged(() => Scale);
				OnPropertyChanged(() => PageBorderThickness);
			}
		}
		public double MinScale
		{
			get { return 0.1; }
		}
		public double MaxScale
		{
			get { return 5; }
		}

	}
}
