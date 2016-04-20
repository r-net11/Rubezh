using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Reports;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace ReportsModule.ViewModels
{
	public class ReportsViewModel : ViewPartViewModel
	{
		public ReportsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh, CanRefresh);
			FilterCommand = new RelayCommand(OnFilter, CanFilter);
			PrintReportCommand = new RelayCommand(OnPrintReport, CanPrintReport);
			PdfPrintReportCommand = new RelayCommand(OnPdfPrintReport, CanPdfPrintReport);
			Reports = new List<ReportViewModel>();
			SelectedReport = null;
		}

		private DocumentPaginator _documentPaginator;
		public DocumentPaginator DocumentPaginator
		{
			get { return _documentPaginator; }
			private set
			{
				_documentPaginator = value;
				OnPropertyChanged(() => DocumentWidth);
				OnPropertyChanged("DocumentHeight");
				OnPropertyChanged("DocumentPaginator");
			}
		}
		public double DocumentWidth
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Width; }
		}
		public double DocumentHeight
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Height; }
		}

		public List<ReportViewModel> Reports { get; private set; }

		ReportViewModel _selectedReport;
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
				OnPropertyChanged("SelectedReport");
				RefreshCommand.Execute();
			}
		}

		bool _inProgress;
		public bool InProgress
		{
			get { return _inProgress; }
			set
			{
				_inProgress = value;
				OnPropertyChanged("InProgress");
			}
		}

		bool _isSelectingReport;
		public bool IsSelectingReport
		{
			get { return _isSelectingReport; }
			set
			{
				_isSelectingReport = value;
				OnPropertyChanged("IsSelectingReport");
				if (IsSelectingReport)
					Reports.ForEach(report => report.OnPropertyChanged("IsEnabled"));
			}
		}

		public bool IsPdfAllowed
		{
			get
			{
#if DEBUG
				return true;
#endif
				return false;
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			if (SelectedReport != null)
				ApplicationService.BeginInvoke((Action)(() =>
					{
						using (new TimeCounter("Total: {0}", true, true))
						using (new WaitWrapper())
							try
							{
								LoadingService.Show("Идет построение отчета", "Идет построение отчета", 0);
								ApplicationService.DoEvents();
								InProgress = true;
								DocumentPaginator = SelectedReport.GenerateReport();
							}
							finally
							{
								LoadingService.Close();
								InProgress = false;
							}
					}));
			else
				DocumentPaginator = null;
		}
		bool CanRefresh()
		{
			return SelectedReport != null;
		}

		public RelayCommand FilterCommand { get; private set; }
		void OnFilter()
		{
			SelectedReport.Filter(RefreshCommand);
		}
		bool CanFilter()
		{
			return SelectedReport != null && SelectedReport.IsFilterable;
		}

		public RelayCommand PrintReportCommand { get; private set; }
		void OnPrintReport()
		{
			var printDialog = new PrintDialog();
			if (printDialog.ShowDialog() == true)
			{
				SelectedReport.PreparePrinting(printDialog.PrintTicket, DocumentPaginator.PageSize);
				XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
				if (writer != null)
					writer.WriteAsync(DocumentPaginator, printDialog.PrintTicket);
			}
		}
		bool CanPrintReport()
		{
			return DocumentPaginator != null && DocumentPaginator.PageCount > 0;
		}

		public RelayCommand PdfPrintReportCommand { get; private set; }
		void OnPdfPrintReport()
		{
			SelectedReport.PdfPrint();
		}
		bool CanPdfPrintReport()
		{
			return DocumentPaginator != null && DocumentPaginator.PageCount > 0 && SelectedReport != null && SelectedReport.CanPdfPrint;
		}

		public void AddReports(IEnumerable<IReportProvider> reportProviders)
		{
			foreach (var reportProvider in reportProviders)
				Reports.Add(new ReportViewModel(reportProvider));
		}

		public override void OnShow()
		{
			if (SelectedReport == null)
				SelectedReport = Reports.FirstOrDefault();
		}
	}
}