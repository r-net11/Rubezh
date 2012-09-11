using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace ReportsModule.ViewModels
{
	public class ReportsViewModel : ViewPartViewModel
	{
		public ReportsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh, CanRefresh);
			FilterCommand = new RelayCommand(OnFilter, CanFilter);
			PrintReportCommand = new RelayCommand(OnPrintReport, CanPrintReport);
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
				OnPropertyChanged("DocumentWidth");
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
				OnPropertyChanged("SelectedReport");
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
				OnPropertyChanged("InProgress");
			}
		}
		private bool _isSelectingReport;
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

		public RelayCommand RefreshCommand { get; private set; }
		private void OnRefresh()
		{
			if (SelectedReport != null)
				ApplicationService.BeginInvoke((Action)(() =>
					{
						DateTime dt = DateTime.Now;
						try
						{
							LoadingService.ShowProgress("Подождите...", "Идет построение отчета", 0);
							InProgress = true;
							DocumentPaginator = SelectedReport.GenerateReport();
						}
						finally
						{
							LoadingService.Close();
							InProgress = false;
						}
						Debug.WriteLine("Total: {0}", DateTime.Now - dt);
						Debug.WriteLine("Page Count: {0}", DocumentPaginator == null ? 0 : DocumentPaginator.PageCount);
					}));
			else
				DocumentPaginator = null;
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
				SelectedReport.PreparePrinting(printDialog.PrintTicket, DocumentPaginator.PageSize);
				XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
				if (writer != null)
					writer.WriteAsync(DocumentPaginator, printDialog.PrintTicket);
			}
		}
		private bool CanPrintReport()
		{
			return DocumentPaginator != null && DocumentPaginator.PageCount > 0;
		}

		public void AddReports(IEnumerable<IReportProvider> reportProviders)
		{
			foreach (var reportProvider in reportProviders)
				Reports.Add(new ReportViewModel(reportProvider));
		}
	}
}