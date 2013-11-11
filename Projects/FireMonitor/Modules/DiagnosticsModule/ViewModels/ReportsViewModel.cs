using System.Linq;
using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Windows;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using Infrastructure.Common.Reports;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Common.PDF;

namespace DiagnosticsModule.ViewModels
{
	public class ReportsViewModel : DialogViewModel
	{
		public ReportsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh, CanRefresh);
			FilterCommand = new RelayCommand(OnFilter, CanFilter);
			PrintReportCommand = new RelayCommand(OnPrintReport, CanPrintReport);
			PdfPrintReportCommand = new RelayCommand(OnPdfPrintReport, CanPdfPrintReport);
			Reports = new List<ReportViewModel>();
			SelectedReport = null;
			//
			LoadReportList();
		}
		private void LoadReportList()
		{
			Reports.Clear();
			foreach (var module in ApplicationService.Modules)
			{
				var reportProviderModule = module as IReportProviderModule;
				if (reportProviderModule != null)
					AddReports(reportProviderModule.GetReportProviders());
			}
			if (SelectedReport == null)
				SelectedReport = Reports.FirstOrDefault();
		}

		public List<ReportViewModel> Reports { get; private set; }
		private ReportViewModel _selectedReport;
		public ReportViewModel SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				if (SelectedReport != null)
					SelectedReport.IsActive = false;
				_selectedReport = value;
				if (SelectedReport != null)
					SelectedReport.IsActive = true;
				OnPropertyChanged(() => SelectedReport);
				RefreshCommand.Execute();
			}
		}
		private bool _inProgress;
		public bool InProgress
		{
			get { return _inProgress; }
			set
			{
				_inProgress = value;
				OnPropertyChanged(() => InProgress);
			}
		}
		private bool _isSelectingReport;
		public bool IsSelectingReport
		{
			get { return _isSelectingReport; }
			set
			{
				_isSelectingReport = value;
				OnPropertyChanged(() => IsSelectingReport);
				if (IsSelectingReport)
					Reports.ForEach(report => report.OnPropertyChanged(() => report.IsEnabled));
			}
		}
		private string _pdfPath;
		public string PdfPath
		{
			get { return _pdfPath; }
			set
			{
				_pdfPath = value;
				OnPropertyChanged(() => PdfPath);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			if (SelectedReport != null)
				ApplicationService.BeginInvoke((Action)(() =>
				{
					DateTime dt = DateTime.Now;
					try
					{
						LoadingService.Show("Идет построение отчета", 0);
						InProgress = true;
						var path = Path.ChangeExtension(AppDataFolderHelper.GetTempFileName(), ".pdf");
						SelectedReport.BuildReport(path);
						//PdfPath = @"d:\Rubezh\Projects\FireMonitor\bin\Debug\test.pdf";
						PdfPath = path;
					}
					finally
					{
						LoadingService.Close();
						InProgress = false;
					}
					Debug.WriteLine("Total: {0}", DateTime.Now - dt);
				}));
			else
				PdfPath = null;
		}
		private bool CanRefresh()
		{
			return SelectedReport != null;
		}

		public RelayCommand FilterCommand { get; private set; }
		private void OnFilter()
		{
			SelectedReport.Filter(RefreshCommand);
		}
		private bool CanFilter()
		{
			return SelectedReport != null && SelectedReport.IsFilterable;
		}

		public RelayCommand PrintReportCommand { get; private set; }
		private void OnPrintReport()
		{
			var printDialog = new PrintDialog();
			if (printDialog.ShowDialog() == true)
			{
				//SelectedReport.PreparePrinting(printDialog.PrintTicket, DocumentPaginator.PageSize);
				//XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
				//if (writer != null)
				//    writer.WriteAsync(DocumentPaginator, printDialog.PrintTicket);
			}
		}
		private bool CanPrintReport()
		{
			return PdfPath != null;
		}

		public RelayCommand PdfPrintReportCommand { get; private set; }
		private void OnPdfPrintReport()
		{
			var fileName = PDFHelper.ShowSavePdfDialog();
			if (!string.IsNullOrEmpty(fileName))
				File.Copy(PdfPath, fileName, true);
		}
		private bool CanPdfPrintReport()
		{
			return PdfPath != null;
		}

		public void AddReports(IEnumerable<IReportProvider> reportProviders)
		{
			foreach (var reportProvider in reportProviders)
				Reports.Add(new ReportViewModel(reportProvider));
		}
	}
}